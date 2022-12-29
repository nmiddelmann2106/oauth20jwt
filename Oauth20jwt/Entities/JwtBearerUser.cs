namespace Oauth20jwt.Entities
{
    public class JwtBearerUser
    {
        public string Id { get; set; }
        public string Passwort { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string EMail { get; set; }
        public string AccessLevel { get; set; }
        public string Kundennummer { get; set; }
        public List<string> Institutionskennzeichen { get; set; }
    }  
}
