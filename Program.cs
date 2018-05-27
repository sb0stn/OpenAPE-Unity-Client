﻿using System;
using OpenAPE;
using System.Collections;

namespace OpenAPE_C_ {
    class Program {
        static void Main (string[] args) {
            // getting the right persona, this can be Olaf, Mary or Hannes.
            Persona persona = Persona.Olaf;

            // this class handles the communication with the server
            Client client = new Client ();

            // first we need to login to receive a token to authenticate with. This is valid for 1440s currently. 
            bool loginSuccess = client.Login (persona.Username, persona.Password);

            if (!loginSuccess) {
                // handle error such as wrong credentials
                return;
            }

            // now we need to get a profile. For now, it suffices to retrieve the default profile.
            PreferenceTerms preferences;
            bool profileSuccess = client.GetProfile (persona.Id, out preferences);

            if (!profileSuccess) {
                // handle error such as expired token
                return;
            }

            // at this point we have successfully gotten the preferences.
            Console.WriteLine (preferences);
            Console.WriteLine (preferences.Get("auditoryOutLanguage"));
        }
    }
}