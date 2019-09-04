using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Validators
{
    /// <summary>
    /// PASSWORD RULES:
    /// 1) length >= 8
    /// 2) length <= 30
    /// 3) MUST contain only english letters (higher or lower case) and at least one number
    /// </summary>
    public class PasswordValidator : IValidator
    {
        private static int minPassLength = 8;
        private static int maxPassLength = 30;

        private static string englishLetters = "qwertyuiopasdfghjklzxcvbnm";
        private static string numbers = "0123456789";

        private static string notEnglishLetterInPassErrorMsg = "The password must contain only english letters";
        private static string noNumberInPasswordErrorMsg = "The password must contatin at least one number";
        private static string passwordNotLongEnoughErrorMsg =
            $"The length of the login must be greater than {minPassLength} and less than {maxPassLength}";

        private string password;

        public PasswordValidator(string password)
        {
            this.password = password?.ToLower();
        }

        public (bool, string) Validate()
        {
            if (password == null || password.Length < minPassLength || password.Length > maxPassLength)
            {
                return (false, passwordNotLongEnoughErrorMsg);
            }

            bool isNumberInPass = false;

            for (int i = 0; i < password.Length; i++)
            {
                char letter = password[i];

                if (englishLetters.IndexOf(letter) == -1 && numbers.IndexOf(letter) == -1)
                {
                    return (false, notEnglishLetterInPassErrorMsg);
                }

                if (numbers.IndexOf(letter) > -1)
                {
                    isNumberInPass = true;
                }
            }

            if (!isNumberInPass)
            {
                return (false, noNumberInPasswordErrorMsg);
            }

            return (true, string.Empty);
        }
    }
}
