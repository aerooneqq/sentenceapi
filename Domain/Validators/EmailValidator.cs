using Domain.KernelInterfaces;

using System.Net.Mail;


namespace Domain.Validators
{
    /// <summary>
    /// The standart rules of email address are implemented here
    /// </summary>
    public class EmailValidator : IValidator
    {
        private static string errorMessage = "Email address is not correct.";
        private static string emailNullError = "Email length must be greater than 0";

        private readonly string email;

        
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
                new MailAddress(email);

                return (true, string.Empty);
            }
            catch
            {
                return (false, errorMessage);
            }
        }
    }
}
