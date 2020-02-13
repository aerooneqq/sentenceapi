using System.Security.Cryptography;
using System.Text;

using Application.Hash.Interfaces;


namespace Application.Hash
{
    public class HashService : IHashService
    {
        public string GetHash(byte[] bytes)
        {
            using MD5 md5Hash = MD5.Create();
            byte[] hash = md5Hash.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            { 
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}