using System;
using Newtonsoft.Json;
using UnityEngine;

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
        private const string ApplicationTermBaseUri = "http://registry.gpii.eu/sh2018ar/";

        /// <summary>
        ///     Create a new preference term from serialization.
        /// </summary>
        /// <param name="key">The key that is used.</param>
        /// <param name="value">The value that is used.</param>
        /// <param name="uri">The uri that is used.</param>
        [JsonConstructor]
        private PreferenceTerm(string key, string value, string uri)
        {
            Key = key;
            Value = ConvertValue(key, value);
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

            Value = ConvertValue(key, value);
          
        }

        /// <summary>
        ///     The key of this preference term.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The Uri of this preference term.
        /// </summary>
        /// <remark>
        ///     E.g. "http://registry.gpii.eu/common/"
        /// </remark>
        public string Uri { get; }

        /// <summary>
        ///     The value of this preference term.
        /// </summary>
        public dynamic Value { get; set; }


        private static dynamic ConvertValue(string key, string value)
        {
            try
            {

                switch (key)
                {
                    case "grayScale":
                        return Boolean.Parse(value);

                    case "fontSize":
                        return short.Parse(value);
                    
                    case "iconLocation":
                        switch (value)
                        {
                           case "NORMAL":
                               return CircleButtonGroupManager.ViewType.NORMAL;
                           
                           case "RIGHT":
                               return CircleButtonGroupManager.ViewType.RIGHT;
                           
                           case "TOPRIGHT":
                               return CircleButtonGroupManager.ViewType.TOPRIGHT;
                           
                           default:
                               Console.WriteLine("Unknown enum value: " + value);
                               return value;
                        }

                    default:
                        Console.WriteLine("Unknown key: " + key);
                        return value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not convert value: " + value);
                return value;
            }
        }


        /// <summary>
        ///     Returns a string representation of this preference term.
        /// </summary>
        /// <returns>A printable string.</returns>
        public override string ToString()
        {
            return "{\"" + Uri + Key + "\" : \"" + Value + "}";
        }
    }
}