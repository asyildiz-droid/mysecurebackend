using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MySecureBackend.WebApi.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)

        {

            byte[] salt = RandomNumberGenerator.GetBytes(16);


            string hash = Convert.ToBase64String(

                KeyDerivation.Pbkdf2(

                    password: password,

                    salt: salt,

                    prf: KeyDerivationPrf.HMACSHA256,

                    iterationCount: 10000,

                    numBytesRequested: 32));


            return Convert.ToBase64String(salt) + ":" + hash;

        }


        public bool VerifyPassword(string password, string storedHash)

        {

            var parts = storedHash.Split(':');

            var salt = Convert.FromBase64String(parts[0]);

            var hash = parts[1];


            string testHash = Convert.ToBase64String(

                KeyDerivation.Pbkdf2(

                    password,

                    salt,

                    KeyDerivationPrf.HMACSHA256,

                    10000,

                    32));


            return testHash == hash;

        }
    }
}
