using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAPE
{
    public class Personas
    {
        private static List<Persona> personas;


        public Personas(string json)
        {
            personas = JsonConvert.DeserializeObject<List<Persona>>(json);
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
        [JsonProperty("name")] public readonly string Name;

        [JsonProperty("user")] public readonly string Username;

        [JsonProperty("password")] public readonly string Password;

        [JsonProperty("id")] public readonly string Id;

        internal Persona()
        {
        }

        internal Persona(string name, string username, string password, string id)
        {
            Name = name;
            Username = username;
            Password = password;
            Id = id;
        }
    }
}