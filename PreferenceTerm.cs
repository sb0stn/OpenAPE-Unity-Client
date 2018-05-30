using System;
using UnityEngine;
using Newtonsoft.Json;

namespace OpenAPE
{
    /// <summary>
    ///     The PreferenceTerm class.
    ///     Contains a representation of a single preference term.
    /// </summary>
    [Serializable]
    public class PreferenceTerm
    {
        /// <summary>
        ///     The base URI for common terms.
        /// </summary>
        /// <remarks>
        ///     This is used to simplify getting a specific preference.
        /// </remarks>
        private const string CommonTermBaseUri = "http://registry.gpii.eu/common/";
        
        /// <summary>
        ///     The base URI for application terms.
        /// </summary>
        /// <remarks>
        ///     This is used to simplify getting a specific preference.
        /// </remarks>
        private const string ApplicationTermBaseUri = "http://registry.gpii.net/applications/de.hdm.sh18/";

        /// <summary>
        ///     Create a new preference term from serialization.
        /// </summary>
        /// <param name="key">The key that is used.</param>
        /// <param name="type">The type that is used.</param>
        /// <param name="value">The value that is used.</param>
        /// <param name="uri">The uri that is used.</param>
        [JsonConstructor]
        private PreferenceTerm(string key, string type, string value, string uri) {
            Key = key;
            Type = type;
            Value = value;
            Uri = uri;
        }

        /// <summary>
        ///     Create a new preference term with the given key and value.
        /// </summary>
        /// <param name="key">The key that is used.</param>
        /// <param name="value">The value that is used.</param>
        public PreferenceTerm(string key, string value)
        {
            if (key.Contains(CommonTermBaseUri))
            {
                Key = key.Replace(CommonTermBaseUri, "");
                Uri = CommonTermBaseUri;
            }
            else if (key.Contains(ApplicationTermBaseUri))
            {
                Key = key.Replace(ApplicationTermBaseUri, "");
                Uri = ApplicationTermBaseUri; 
            }
            else
            {
                Debug.Log("Unknown preference term URI encountered in key: " + key);
            }
            
            Value = value;
            Type = TypeValue(Key);
        }

        private static string TypeValue(string key)
        {
            switch (key)
            {
                case "language":
                case "auditoryOutLanguage":
                    return "string";

                case "highContrastEnabled":
                case "voiceControlEnabled":
                    return "bool";

                case "fontSize":
                case "speechRate":
                    return "short";

                case "pitch":
                case "volumeTTS":
                    return "double";

                default:
                    Console.WriteLine("Unknown key: " + key);
                    return "string";
            }
        }

        /// <summary>
        ///     The key of this preference term.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The Uri of this preference term.
        /// </summary>
        /// <remark>
        ///    E.g. "http://registry.gpii.eu/common/"
        /// </remark>
        public string Uri { get; }

        /// <summary>
        ///     The value of this preference term.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     The type of this preference term.
        /// </summary>
        /// <remarks>
        ///     Use this to parse the value. Unfortunately our .NET version does not support dynamic type.
        /// </remarks>
        public string Type { get; }


        /// <summary>
        ///     Returns a string representation of this preference term.
        /// </summary>
        /// <returns>A printable string.</returns>
        public override string ToString()
        {
            return "{\"" + Uri + Key + "\" : \"" + Value + "\" (" + Type + ")}";
        }
    }
}