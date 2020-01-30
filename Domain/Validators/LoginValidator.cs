using Domain.KernelInterfaces;

namespace Domain.Validators
{
    /// <summary>
    /// LOGIN CONDITIONS:
    /// 1) length > 8
    /// 2) MUST contain an english lower-case letter
    /// 3) MUST contain an english upper-case letter
    /// 4) MUST contain a number
    /// </summary>
    public class LoginValidator : IValidator
    {
        private static string englishLowerLetters = "qwertyuiopasdfghjklzxcvbnm";
        private static string englishHigherLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
        private static string numbers = "0123456789";

        private static string notEnoughLengthErrorMessage = "The length of the login must be greater than 8";
        private static string noCapitalLetterErrorMessage = "The login must contain the capital letters";
        private static string noLowerLetterErrorMesage = "The login must contain lower english letter";
        private static string noNumberInLoginErrorMessage = "The login must contain number";

        private readonly string login;

        public LoginValidator(string login)
        {
            this.login = login;
        }

        public (bool, string) Validate()
        {
            if (login == null || login.Length < 8)
            {
                return (false, notEnoughLengthErrorMessage);
            }

            bool capitalLetterInLogin = false;
            bool lowerLetterInLogin = false;
            bool numberInLogin = false;

            foreach (char letter in login)
            {
                if (englishLowerLetters.IndexOf(letter) > -1)
                {
                    lowerLetterInLogin = true;
                }

                if (englishHigherLetters.IndexOf(letter) > -1)
                {
                    capitalLetterInLogin = true;
                }

                if (numbers.IndexOf(letter) > -1)
                {
                    numberInLogin = true;
                }
            }

            if (!capitalLetterInLogin)
            {
                return (false, noCapitalLetterErrorMessage);
            }

            if (!lowerLetterInLogin)
            {
                return (false, noLowerLetterErrorMesage);
            }

            if (!numberInLogin)
            {
                return (false, noNumberInLoginErrorMessage);
            }

            return (true, string.Empty);
        }
    }
}
