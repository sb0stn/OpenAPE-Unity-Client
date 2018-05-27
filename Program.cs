﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace OpenAPE
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // getting the right persona, this can be Olaf, Mary or Hannes.
            var persona = Persona.Olaf;

            // this class handles the communication with the server
            var client = new Client();

            // first we need to login to receive a token to authenticate with. This is valid for 1440s currently. 
            var loginSuccess = client.Login(persona.Username, persona.Password);

            if (!loginSuccess)
            {
                // handle error such as wrong credentials
                return;
            }

            // now we need to get a profile. For now, it suffices to retrieve the default profile.
            var profileSuccess = client.GetProfile(persona.Id, out var preferences);

            if (!profileSuccess)
            {
                // handle error such as expired token
                return;
            }

            // at this point we have successfully gotten the preferences.
            Console.WriteLine(preferences);
            Console.WriteLine(preferences.Get("auditoryOutLanguage"));

            // testing serialization
            var formatter = new BinaryFormatter();
            Stream outStream = new FileStream("preferences.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(outStream, preferences);
            outStream.Close();


            Stream inStream = new FileStream("preferences.bin", FileMode.Open, FileAccess.Read, FileShare.None);
            var results = (PreferenceTerms) formatter.Deserialize(inStream);
            Console.WriteLine(results);
            inStream.Close();
            File.Delete("preferences.bin");
        }
    }
}