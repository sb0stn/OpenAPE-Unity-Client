using System.Xml;
using System.Collections.Generic;

namespace OpenAPE
{
    public class Personas
    {
        private static List<Persona> personas;


        public Personas(string xml)
        {         
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nodes = doc.DocumentElement.SelectNodes("/personas/persona");

            personas = new List<Persona>(nodes.Count);
            
            foreach (XmlNode node in nodes)
            {                
                var persona = new Persona(
                    node.SelectSingleNode("name").InnerText,
                    node.SelectSingleNode("password").InnerText,
                    node.SelectSingleNode("id").InnerText
                ); 

                personas.Add(persona);
            }
        }


        public Persona Get(int idx)
        {
            if (idx < personas.Count)
            {
                return personas[idx];
            }

            return null;
        }
        
        public List<Persona> GetAll()
        {
            return personas;
        }
    }
    
    
    public class Persona
    {
        public readonly string Id;
        public readonly string Password;
        public readonly string Username;

        internal Persona(string username, string password, string id)
        {
            Username = username;
            Password = password;
            Id = id;
        }
    }
}