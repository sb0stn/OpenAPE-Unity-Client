using System;

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
        ///     Create a new preference term with the given key and value.
        /// </summary>
        /// <param name="key">The key that is used.</param>
        /// <param name="value">The value that is used.</param>
        public PreferenceTerm(string key, string value)
        {
            Key = key.Replace(CommonTermBaseUri, "");
            Uri = CommonTermBaseUri;
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
        public string Value { get; }

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
            return "{\"" + Key + "\" : \"" + Value + "\" (" + Type + ")}";
        }
    }
}