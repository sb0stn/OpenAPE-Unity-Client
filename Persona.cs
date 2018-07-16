using System.Xml;
using System.Collections.Generic;

namespace OpenAPE
{
    public class Persona
    {
        public readonly string Id;
        public readonly string Password;
        public readonly string Username;

        private static List<Persona> personas;


        public static void InitWithConfig(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);

            foreach (XmlNode node in doc.DocumentElement.SelectNodes("/personas/persona"))
            {
                var persona = new Persona(
                    node.SelectSingleNode("/name").InnerText,
                    node.SelectSingleNode("/password").InnerText,
                    node.SelectSingleNode("/id").InnerText
                    ); 

                    personas.Add(persona);
            }
        }

        public static List<Persona> GetPersonas()
        {
            return personas;
        }

        private Persona(string username, string password, string id)
        {
            Username = username;
            Password = password;
            Id = id;
        }
    }
}