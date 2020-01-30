using System.Collections.Generic;

using Domain.Validators;

using Domain.KernelInterfaces;

namespace Domain.Users
{
    public class UserInfoValidator : IModelValidator
    {
        private readonly UserInfo model;
        private readonly IEnumerable<IValidator> validators;

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
