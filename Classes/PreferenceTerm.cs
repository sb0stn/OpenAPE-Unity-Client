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
        private const string _CommonTermBaseURI = "http://registry.gpii.eu/common/";

        /// <summary>
        ///     Create a new preference term with the given key and value.
        /// </summary>
        /// <param name="key">The key that is used.</param>
        /// <param name="value">The value that is used.</param>
        public PreferenceTerm(string key, string value)
        {
            Key = key.Replace(_CommonTermBaseURI, "");
            Value = value;
        }

        /// <summary>
        ///     The key of this preference term.
        /// </summary>
        /// <remark>
        ///     Contains the term URI, e.g. "http://registry.gpii.eu/common/"
        /// </remark>
        public string Key { get; }

        /// <summary>
        ///     The value of this preference term.
        /// </summary>
        /// <remark>
        ///     Currently is always a string. This will change soon!
        /// </remark>
        public string Value { get; }

        /// <summary>
        ///     Returns a string representation of this preference term.
        /// </summary>
        /// <returns>A printable string.</returns>
        public override string ToString()
        {
            return "{\"" + Key + "\" : \"" + Value + "\"}";
        }
    }
}