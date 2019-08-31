using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SentenceAPI.Validators
{
    public class EmailValidator : IValidator
    {
        private static string errorMessage = "Email address is not correct.";
        private static string emailNullError = "Email length must be greater than 0";

        private string email;

        public EmailValidator(string email)
        {
            this.email = email;
        }

        public (bool, string) Validate()
        {
            if (email == null)
            {
                return (false, emailNullError);
            }

            try
            {
                MailAddress mailAddress = new MailAddress(email);

                return (true, string.Empty);
            }
            catch (FormatException)
            {
                return (false, errorMessage);
            }
        }
    }
}
