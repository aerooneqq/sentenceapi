using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Validators;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Users.Models
{
    public class UserInfoValidator : IModelValidator
    {
        private UserInfo model;
        private IEnumerable<IValidator> validators;

        public UserInfoValidator(UserInfo model)
        {
            this.model = model;
            validators = new IValidator[]
            {
                new EmailValidator(model.Email),
                new LoginValidator(model.Login),
                new PasswordValidator(model.Password)
            };
        }

        public (bool, IEnumerable<string>) Validate()
        {
            bool result = true;
            List<string> errorMessages = new List<string>();

            foreach (IValidator validator in validators)
            {
                (bool validationResult, string errorMsg) = validator.Validate();
                result &= validationResult;

                if (errorMsg.Length > 0)
                {
                    errorMessages.Add(errorMsg);
                }
            }

            return (result, errorMessages);
        }
    }
}
