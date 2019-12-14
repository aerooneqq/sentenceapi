using System.Security.Cryptography;
using System.Text;

namespace SentenceAPI.Extensions
{
    public static class HashExtensions
    { 
        /// <summary>
        /// Computes the MD5 hash of a byte array, returns the string which represents the hash.
        /// <summary>
        public static string GetMD5Hash(this byte[] obj)
        { 
            using MD5 md5Hash = MD5.Create();
            byte[] hash = md5Hash.ComputeHash(obj);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            { 
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}