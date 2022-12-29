using System.Security.Cryptography;
using System.Text;

namespace Oauth20jwt
{
    public class PasswortHelper
    {
        public string Encrypt(string password)
        {
            try
            {
                const string publickey = "12345678"; // ToDo
                const string secretkey = "87654321"; // ToDo
                var secretkeyByte = Encoding.UTF8.GetBytes(secretkey);
                var publickeybyte = Encoding.UTF8.GetBytes(publickey);
                var inputbyteArray = Encoding.UTF8.GetBytes(password);
                using var des = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public string Decrypt(string verschluesseltesPasswort)
        {
            try
            {
                const string publickey = "12345678"; // ToDo
                const string secretkey = "87654321"; // ToDo
                var privatekeyByte = Encoding.UTF8.GetBytes(secretkey);
                var publickeybyte = Encoding.UTF8.GetBytes(publickey);
                
                var inputbyteArray = Convert.FromBase64String(verschluesseltesPasswort.Replace(" ", "+"));
                using var des = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                var encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
    }
}
