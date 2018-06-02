// hijacking unity log when building outside of unity.
#if !(UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WII || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WEBGL || UNITY_FACEBOOK || UNITY_ADS || UNITY_ANALYTICS || UNITY_ASSERTIONS)
using System;
namespace UnityEngine {
    class Debug {
        public static void Log(string s) {
            Console.WriteLine(s);
        }
    }
}
#endif