using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Oauth20jwt.Entities;
using Oauth20jwt.Exceptions;
using Styra.Omni.WebService.Helper;

namespace Oauth20jwt
{
    public class JwtTokenProvider
    {
        private PasswortHelper PasswortHelper { get; set; } = new PasswortHelper();

        private List<JwtBearerUser> UserList = new()
        {
            new JwtBearerUser()
            {
                Id = "musterkunde",
                Passwort = "ozaMgsSx7B+kkZRNrBLotxajeUbJ1bRfjGqF4dKJDzHY+O2sy423Vx+ZF5W+bt7WwXDDhG2qm0k=",
                //Passwort = "5a@7&Z&4Fs@X&NpelzSp42hi5Vk$r@n0CEGJK2eXdgrxOgtfhQ",// Für Kunde
                EMail = "info@musterkunde.de",
                Vorname = "Max",
                Nachname = "Mustermann",
                AccessLevel = "Customer",
                Institutionskennzeichen = new List<string>() { "123456789" },
                Kundennummer = "1234"
            },
            new JwtBearerUser
            {
                Id = "nina",
                Passwort = "0D5vjeUH+FeZLnhdEoSK+ro3ZnROnUZ3pCKkE6KTY/XzQLfVBhJQrtWTB/cX/8Qj5OgJSoWNNsA=",
                //Passwort = "s#%qSU64Z0pyu9J4@19K$j7bby^6FU4sOT%gnjvfDWaJScYIYe", // Für Kunde
                Vorname = "Nina",
                Nachname = "Middelmann",
                EMail = "info@middelmann.de",
                AccessLevel = "Owner",
                Institutionskennzeichen = new List<string>() { "987654321" },
                Kundennummer = "9876"
            },
            // ToDo: Anmeldung im Namen des Kunden vereinfachen um Situationen nachzustellen...
            new JwtBearerUser()
            {
                Id = "ninastestuser_123456789",
                Passwort = "qPD2lwrxbMk=",
                //Passwort = "1234567", // Für Kunde
                Vorname = "Nina",
                Nachname = "Middelmann",
                EMail = "info@middelmann.de",
                AccessLevel = "Owner",
                Institutionskennzeichen = new List<string>() { "123456789" },
                Kundennummer = "1234"
            }
        };

        private IEnumerable<Claim> GetUserClaims(JwtBearerUser user)
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Vorname + " " + user.Nachname),
                new Claim("Id", user.Id),
                new Claim("EMail", user.EMail),
                new Claim("AccessLevel", user.AccessLevel.ToUpper()),
                new Claim("Institutionskennzeichen", JsonConvert.SerializeObject(user.Institutionskennzeichen)),
                new Claim("Kundennummer", user.Kundennummer)
            };
            return claims;
        }

        public string GenerateToken(string userId, string password)
        {
            // ToDo User in DB speichern
            // ToDo Get UserDetails aus DB (hier nur vereinfachtes Verfahren, da DB noch nicht vorhanden)
            var user = UserList.SingleOrDefault(x => x.Id == userId);

            // ToDo Check User ob registriert in DB
            if (user == null)
                return null;

            // ToDo Passwort in DB hashen und speichern
            // ToDo Wenn es ein registrierter User ist, nachsehen ob Passwort in DB (hier nur vereinfachtes Verfahren, da DB noch nicht vorhanden)

            var verschluesseltesPasswort = PasswortHelper.Encrypt(password);

            if (verschluesseltesPasswort == user.Passwort)
            {
                //Provide the security key which was given in the JWToken configuration in Startup.cs
                // ToDo: SecurityKey auslagern..
                var key = Encoding.ASCII.GetBytes
                    ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                // Token wird erstellt 
                var jwToken = new JwtSecurityToken(
                    issuer: "http://localhost:45092/", // ToDo
                    audience: "http://localhost:45092/", // ToDo
                    claims: GetUserClaims(user),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                    //Using HS256 Algorithm to encrypt Token
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );
                var token = new JwtSecurityTokenHandler().WriteToken(jwToken);

                return token;
            }
            else
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) return false;

            SecurityToken securityToken;

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = "http://localhost:45092/", // ToDo
                ValidIssuer = "http://localhost:45092/", // ToDo
                ValidateLifetime = true,
                // ToDo SecurityKey auslagern..
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv")),
                RequireExpirationTime = false // ToDo vllt false?
            };

            try
            {
                var claims = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out securityToken);
            }
            catch (ArgumentException e)
            {
                throw new TokenStringNichtValideException(e.Message, e);
                // ToDo Hier ggf. Fehler an xxx senden.. wenn irgendjemand einfach versucht mit einem Token aus dem Netz drauf zuzugreifen...
            }

            return securityToken != null;

        }

        public ClaimsPrincipal GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) throw new TokenStringNichtValideException("Token nicht valide.");

            SecurityToken securityToken;

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = "http://localhost:45092/", // ToDo
                ValidIssuer = "http://localhost:45092/", // ToDo
                //IssuerSigningKeys = config.SigningKeys,   // ToDo 
                ValidateLifetime = false,  // ToDo 
                // ToDo SecurityKey auslagern..
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv")),
                RequireExpirationTime = false // ToDo
            };

            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out securityToken);
            }
            catch (ArgumentException e)
            {
                throw new TokenStringNichtValideException(e.Message, e);
            }
        }
    }
}
