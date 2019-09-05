using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;

namespace DataAccessLayer.Hashes
{
    public static class HashExtensions
    {
        public static string GetMD5Hash(this string str)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] strBytes = Encoding.UTF8.GetBytes(str);
                byte[] md5HashBytes = md5Hash.ComputeHash(strBytes);

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < md5HashBytes.Length; i++)
                {
                    stringBuilder.Append(md5HashBytes[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}
