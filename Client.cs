using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

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
    ///     The completion handler called when a response was reveived.
    /// </summary>
    /// <param name="response">The response text.</param>
    internal delegate void OnResponseReceived(string response);

    /// <summary>
    ///     The completion handler called when an error occured was reveived.
    /// </summary>
    /// <param name="message">The error message.</param>
    internal delegate void OnErrorReceived(string message);

    /// <summary>
    ///     The completion handler called when a rest call is completed.
    /// </summary>
    /// <param name="status">Whether or not the call succeeded.</param>
    /// <param name="result">The resulting object. May be null on error or when no result is needed.</param>
    public delegate void OnCompletion<T>(bool status, T result);

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
        ///     The user which was last logged in.
        /// </summary>
        private String _loggedInUser;

        /// <summary>
        ///     The latest response received.
        /// </summary>
        private UserContextResponse _userContextResponse;

        /// <summary>
        /// The Parent context that can execute Coroutines.
        /// </summary>
        private ICoroutineExecutor _parent;

        /// <summary>
        ///     Creates a new instance of the client.
        /// </summary>
        /// <remarks>
        ///     Currently trusts ALL certificates!
        /// </remarks>
        public Client(ICoroutineExecutor parent)
        {
            _parent = parent;

            // all Certificates are accepted TODO check if we can replace this
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        /// <summary>
        ///     Login with the given username and password.
        /// </summary>
        /// <remarks>
        ///     You will need to do this before loading any profiles.
        ///     Also please note, that the result of this operation is always null!
        /// </remarks>
        /// <param name="username">The username that is used.</param>
        /// <param name="password">The password that is used.</param>
        /// <param name="onCompletion">The completion handler returns whether the login succeeded.</param>
        public bool Login(string username, string password, OnCompletion<object> onCompletion)
        {
            if (username == null || username.Equals("") || password == null || password.Equals(""))
            {
                Debug.Log("Please supply all expected parameters");
                onCompletion(false, null);
                return false;
            }

            if ((_loggedInUser != null && username.Equals(_loggedInUser)) &&
                (_loginResponse != null && _loginResponse.IsValid))
            {
                Debug.Log("Login is still valid!");
                onCompletion(true, null);
                return true; // TODO remove
            }

            _parent.StartChildCoroutine(_LoginCoroutine(username, password, response =>
            {
                _loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response);
                _loggedInUser = username;
                onCompletion(true, null);
            }, message =>
            {
                Debug.Log("An error occured while logging in...");
                Debug.Log(message);
                onCompletion(false, null);
            }));

            return true; // TODO remove
        }


        /// <summary>
        ///     Retrieves the user profile with the given id.
        /// </summary>
        /// <remarks>
        ///     You have to be logged in with a valid token and the user needs to have access to the supplied profile. This means
        ///     the user owns it or it is public.
        /// </remarks>
        /// <param name="id">The profile's id</param>
        /// <param name="onCompletion">The completion handler returns whether the getting of the profile succeeded.
        /// The result is where the preferenceTerms are stored in. May be null on error.</param>
        internal bool GetProfile(string id, OnCompletion<PreferenceTerms> onCompletion)
        {
            if (id == null || id.Equals(""))
            {
                Debug.Log("Please supply all expected parameters");
                onCompletion(false, null);
                return false;
            }

            if (_loginResponse?.Token == null)
            {
                Debug.Log("You need to login first!");
                onCompletion(false, null);
                return false; // TODO remove
            }

            if (!_loginResponse.IsValid)
            {
                Debug.Log("Login has expired!");
                onCompletion(false, null);
                return false; // TODO remove
            }

            _parent.StartChildCoroutine(_GetProfileCoroutine(id, response =>
            {
                _userContextResponse = JsonConvert.DeserializeObject<UserContextResponse>(response);

                onCompletion(true, new PreferenceTerms(_userContextResponse.UserPreferences.PreferenceTerms));
            }, message =>
            {
                Debug.Log("An error occured while getting user profile...");
                Debug.Log("Are you sure you are logged in with the right user? " + _loggedInUser + " is logged in.");
                Debug.Log(message);
                onCompletion(false, null);
            }));

            return true; // TODO remove
        }

        /// <summary>
        ///     Updates the user profile with the given id.
        /// </summary>
        /// <param name="id">The profile's id</param>
        /// <param name="preferenceTerms">The updated preference terms to save.</param>
        /// <param name="onCompletion">The completion handler returns whether the update of the profile succeeded.</param>
        internal bool UpdateProfile(string id, PreferenceTerms preferenceTerms, OnCompletion<object> onCompletion)
        {
            if (_loginResponse?.Token == null)
            {
                Debug.Log("You need to login first!");
                onCompletion(false, null);
                return false; // TODO remove
            }

            if (!_loginResponse.IsValid)
            {
                Debug.Log("Login has expired!");
                onCompletion(false, null);
                return false; // TODO remove
            }

            _parent.StartChildCoroutine(_UpdateProfileCoroutine(id, preferenceTerms, response =>
            {
                onCompletion(true, null);
            }, message =>
            {
                Debug.Log("An error occured while updating user profile...");
                Debug.Log(message);
                onCompletion(false, null);
            }));

            return true; // TODO remove
        }

        
        
        /// <summary>
        ///     Handles the login as a coroutine.
        /// </summary>
        /// <param name="username">The username that is used.</param>
        /// <param name="password">The password that is used.</param>
        /// <param name="onResponseReceived">The handler to call on success.</param>
        /// <param name="onErrorReceived">The handler to call on error.</param>
        /// <returns>An enumerator.</returns>
        private IEnumerator _LoginCoroutine(string username, string password, OnResponseReceived onResponseReceived, OnErrorReceived onErrorReceived)
        {
            var form = new WWWForm();
            form.AddField("grant_type", "password");
            form.AddField("username", username);
            form.AddField("password", password);


            using (var req = UnityWebRequest.Post(BaseUrl + "token", form))
            {
                req.SetRequestHeader("grant_type", "application/x-www-form-urlencoded");
                req.downloadHandler = new DownloadHandlerBuffer();
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    onErrorReceived(req.error);
                }
                else
                {
                    onResponseReceived(req.downloadHandler.text);
                }
            }
        }

        /// <summary>
        ///     Handles the getting of a profile as a coroutine.
        /// </summary>
        /// <param name="id">The profile's id</param>
        /// <param name="onResponseReceived">The handler to call on success.</param>
        /// <param name="onErrorReceived">The handler to call on error.</param>
        /// <returns>An enumerator.</returns>
        private IEnumerator _GetProfileCoroutine(string id, OnResponseReceived onResponseReceived, OnErrorReceived onErrorReceived)
        {
            using (var req = UnityWebRequest.Get(BaseUrl + "api/user-contexts/" + id))
            {
                req.SetRequestHeader("content-type", "application/json");
                req.SetRequestHeader("authorization", _loginResponse.Token);
                req.downloadHandler = new DownloadHandlerBuffer();
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    onErrorReceived(req.error);
                }
                else
                {
                    onResponseReceived(req.downloadHandler.text);
                }
            }
        }

        /// <summary>
        ///     Handles the updating of a profile as a coroutine.
        /// </summary>
        /// <param name="id">The profile's id</param>
        /// <param name="preferenceTerms">The new and updated preference terms.</param>
        /// <param name="onResponseReceived">The handler to call on success.</param>
        /// <param name="onErrorReceived">The handler to call on error.</param>
        /// <returns>An enumerator.</returns>
        private IEnumerator _UpdateProfileCoroutine(string id, PreferenceTerms preferenceTerms, OnResponseReceived onResponseReceived, OnErrorReceived onErrorReceived)
        {
            using (var req = UnityWebRequest.Put(BaseUrl + "api/user-contexts/" + id, JsonConvert.SerializeObject(preferenceTerms)))
            {
                req.SetRequestHeader("content-type", "application/json");
                req.SetRequestHeader("authorization", _loginResponse.Token);
                req.downloadHandler = new DownloadHandlerBuffer();
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    onErrorReceived(req.error);
                }
                else
                {
                    onResponseReceived(req.downloadHandler.text);
                }
            }
        }
    }
}