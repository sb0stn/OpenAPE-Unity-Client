using System.Collections.Generic;
using System;

namespace OpenAPE {
    /// <summary>
    /// The PreferenceTerms class.
    /// Contains a representation of all preference terms in a profile.
    /// </summary>
    /// <remarks>
    /// Currently only common terms are supported and used.
    /// </remarks>
    class PreferenceTerms {
        /// <summary>
        /// The dictionary containing the preference terms.
        /// </summary>
        private List<PreferenceTerm> _preferences;


        /// <summary>
        /// Creates a preference terms list with the given dictionary.
        /// </summary>
        /// <param name="preferenceTermsDictionary">The dictionary that contains the key and value as returned from the server.</param>
        public PreferenceTerms(PreferenceTermsDictionary preferenceTermsDictionary) {
            _preferences = new List<PreferenceTerm>(preferenceTermsDictionary.Count);

            foreach(KeyValuePair<string, string> entry in preferenceTermsDictionary) {
                PreferenceTerm preferenceTerm = new PreferenceTerm( entry.Key, entry.Value);

                _preferences.Add(preferenceTerm);
            }


        }

        /// <summary>
        /// Gets a single preference term by its key.
        /// </summary>
        /// <param name="key">The key of the preference.</param>
        /// <returns>The preference term or null if it was not found.</returns>
        public PreferenceTerm Get(string key) {
            return _preferences.Find(preferenceTerm => preferenceTerm.Key == key);      
        }

        /// <summary>
        /// Returns a string representation of the preference terms.
        /// </summary>
        /// <returns>A printable string.</returns>
        public override string ToString() {
			// note: do not replace with string.Join(System.Environment.NewLine, _preferences) since Unity's .NET version
			// does not seem to support it yet.

			string retVal = "";

			foreach (PreferenceTerm preferenceTerm in _preferences) {
				retVal += preferenceTerm.ToString () + System.Environment.NewLine;
			}

			return retVal;
				
        }
    }
}