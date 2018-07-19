using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace OpenAPE
{
    public class Personas
    {
        private static List<Persona> personas;


        public Personas(string json)
        {
            var result = JsonConvert.DeserializeObject<List<Persona>>(json);
            personas = result.Where( p => p.Enabled).ToList();
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

        [JsonProperty("enabled")] public readonly bool Enabled;

        internal Persona()
        {
        }

        internal Persona(string name, string username, string password, string id, bool enabled)
        {
            Name = name;
            Username = username;
            Password = password;
            Id = id;
            Enabled = enabled;
        }
    }
}