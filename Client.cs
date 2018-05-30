using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using RestSharp;
using UnityEngine;

namespace OpenAPE
{   
    /// <summary>
    ///     The LoginResponse class.
    ///     Contains a representation of the response that is received from the server on login.
    /// </summary>
    internal class LoginResponse
    {
        /// <summary>
        ///     The exact time the reponse was received.
        /// </summary>
        /// <remarks>
        ///     Used to calculate the expiration of the token
        /// </remarks>
        private readonly DateTime _created = DateTime.Now;

        /// <summary>
        ///     The token that was received.
        /// </summary>
        [JsonProperty("access_token")]
        internal string Token { get; set; }

        /// <summary>
        ///     The expiration of the token.
        /// </summary>
        /// <remarks>
        ///     In seconds since token creation. Use isValid to check for expiration.
        /// </remarks>
        [JsonProperty("expires_in")]
        internal int Expiration { get; set; }

        /// <summary>
        ///     Whether the token is still valid.
        /// </summary>
        /// <remarks>
        ///     Only checks whether the token should still be valid according to the expiration date.
        ///     The server might still reject it for some other reason.
        /// </remarks>
        internal bool IsValid => (DateTime.Now - _created).TotalSeconds < Expiration;
    }

    /// <summary>
    ///     The UserContextResponse class.
    ///     Contains a representation of the response that is received from the server on getting a user context.
    /// </summary>
    internal class UserContextResponse
    {
        /// <summary>
        ///     The default preferences in this profile.
        /// </summary>
        /// <remarks>
        ///     The others are currently ignored if present.
        /// </remarks>
        [JsonProperty("default")]
        internal UserPreferences UserPreferences { get; set; }
    }

    /// <summary>
    ///     The UserPreferences class.
    ///     Contains the data of a specific preference set.
    /// </summary>
    internal class UserPreferences
    {
        /// <summary>
        ///     The human-readable name of this preference set.
        /// </summary>
        [JsonProperty("name")]
        internal string Name { get; set; }

        /// <summary>
        ///     The list of preferences of this preference set.
        /// </summary>
        [JsonProperty("preferences")]
        internal PreferenceTermsDictionary PreferenceTerms { get; set; }
    }


    /// <summary>
    ///     The PreferenceTerms class.
    ///     Contains a list of preferences.
    /// </summary>
    /// <inheritdoc cref="Dictionary{TKey,TValue}" />
    public class PreferenceTermsDictionary : Dictionary<string, string>
    {
    }

    /// <summary>
    ///     The main Client class.
    ///     Is used to communicate with the OpenAPE server.
    /// </summary>
    public class Client
    {
        /// <summary>
        ///     The base url of the server.
        /// </summary>
        private const string BaseUrl = "https://openape.gpii.eu/";
        
        /// <summary>
        ///     The latest response received.
        /// </summary>
        private LoginResponse _loginResponse;

        /// <summary>
        ///     The latest response received.
        /// </summary>
        private UserContextResponse _userContextResponse;
        

        /// <summary>
        ///      Creates a new instance of the client.
        /// </summary>
        /// <remarks>
        ///     Currently trusts ALL certificates!
        /// </remarks>
        public Client()
        {
            // all Certificates are accepted TODO check if we can replace this
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; ;   
        }

        /// <summary>
        ///     Login with the given username and password.
        /// </summary>
        /// <remarks>
        ///     You will need to do this before loading any profiles.
        /// </remarks>
        /// <param name="username">The username that is used.</param>
        /// <param name="password">The password that is used.</param>
        /// <returns>true, if the login succeeded, else false.</returns>
        public bool Login(string username, string password)
        {
            if (_loginResponse != null && _loginResponse.IsValid)
            {
                Debug.Log("Login is still valid!");
                return true;
            }

            var client = new RestClient(BaseUrl + "token");

            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                "grant_type=password&username=" + username + "&password=" + password, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (response.ErrorException != null)
            {
                Debug.Log("An error occured while logging in...");
                return false;
            }

            _loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

            return true;
        }

        /// <summary>
        ///     Retrieves the user profile with the given id.
        /// </summary>
        /// <remarks>
        ///     You have to be logged in with a valid token and the user needs to have access to the supplied profile. This means
        ///     the user owns it or it is public.
        /// </remarks>
        /// <param name="id">The profile's id</param>
        /// <param name="preferenceTerms">The object where the preferenceTerms are stored in. May be null on error.</param>
        /// <returns>true, if the retrieval succeeded, else false.</returns>
        internal bool GetProfile(string id, out PreferenceTerms preferenceTerms)
        {
            if (_loginResponse.Token == null)
            {
                Debug.Log("You need to login first!");
                preferenceTerms = null;
                return false;
            }

            if (!_loginResponse.IsValid)
            {
                Debug.Log("Login has expired!");
                preferenceTerms = null;
                return false;
            }

            var client = new RestClient(BaseUrl + "api/user-contexts/" + id);

            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", _loginResponse.Token);

            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                Debug.Log("An error occured while getting user profile...");
                preferenceTerms = null;
                return false;
            }

            _userContextResponse = JsonConvert.DeserializeObject<UserContextResponse>(response.Content);
            preferenceTerms = new PreferenceTerms(_userContextResponse.UserPreferences.PreferenceTerms);

            return true;
        }

        internal bool UpdateProfile(string id, PreferenceTerms preferenceTerms)
        {
            if (_loginResponse.Token == null)
            {
                Debug.Log("You need to login first!");
                return false;
            }

            if (!_loginResponse.IsValid)
            {
                Debug.Log("Login has expired!");
                return false;
            }
            
            var client = new RestClient(BaseUrl + "api/user-contexts/" + id);

            var request = new RestRequest(Method.PUT);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", _loginResponse.Token);
            
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                Debug.Log("An error occured while updating user profile...");
                return false;
            }

            return true;
        }
    }
}