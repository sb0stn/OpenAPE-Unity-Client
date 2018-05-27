namespace OpenAPE
{
    public class Persona
    {
        public readonly string Username;
        public readonly string Password;
        public readonly string Id;

        public static readonly Persona Olaf = new Persona("sh18_olaf", "sh18demoolaf", "5ad9f7a438f55331474ccdf4");
        public static readonly Persona Mary = new Persona("sh18_mary", "sh18demomary", "5ad9f74c38f55331474ccdf0");
        public static readonly Persona Hannes = new Persona("sh18_hannes", "sh18demohannes", "5ad9f77138f55331474ccdf2");

        private Persona(string username, string password, string id)
        {
            Username = username;
            Password = password;
            Id = id;
        }
    }
}