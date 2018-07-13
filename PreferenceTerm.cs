using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenAPE
{
    public class TypeSwitch
    {
        Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();

        public TypeSwitch Case<T>(Action<T> action)
        {
            matches.Add(typeof(T), (x) => action((T) x));
            return this;
        }

        public void Switch(object x)
        {
            matches[x.GetType()](x);
        }
    }

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
        public string Value { set; get; }


        /// <summary>
        ///     The Type of this preference term.
        /// </summary>
        /// <returns>A Type, i.e. bool.</returns>
        public Type GetTypeOfValue()
        {
            switch (Key)
            {
                case "grayScale":
                    return typeof(bool);

                case "fontSize":
                    return typeof(int);

                case "volumeTTS":
                    return typeof(float);

                case "iconLocation":
                    return typeof(CircleButtonGroupManager.ViewType);
            }

            return typeof(string);
        }


        /// <summary>
        ///     The value of this preference term as a typed instance.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <remarks>You might want to use GetTypeOfValue() if you are not sure what type to expect.</remarks>
        /// <returns>The value converted to T.</returns>
        public T GetValueTyped<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                return (T) Convert.ChangeType(bool.Parse(Value), typeof(T));
            }

            if (typeof(T) == typeof(int))
            {
                return (T) Convert.ChangeType(int.Parse(Value), typeof(T));
            }

            if (typeof(T) == typeof(float))
            {
                return (T) Convert.ChangeType(float.Parse(Value), typeof(T));
            }

            if (typeof(T) == typeof(CircleButtonGroupManager.ViewType))
            {
                switch (Value)
                {
                    case "NORMAL":
                        return (T) Convert.ChangeType(CircleButtonGroupManager.ViewType.NORMAL, typeof(T));

                    case "RIGHT":
                        return (T) Convert.ChangeType(CircleButtonGroupManager.ViewType.RIGHT, typeof(T));

                    case "TOPRIGHT":
                        return (T) Convert.ChangeType(CircleButtonGroupManager.ViewType.TOPRIGHT, typeof(T));
                }
            }

            return (T) Convert.ChangeType(Value, typeof(T));
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