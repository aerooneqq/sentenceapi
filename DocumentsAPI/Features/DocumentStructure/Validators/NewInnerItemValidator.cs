using Application.Documents.DocumentStructure.Models;

using Domain.KernelInterfaces;


namespace DocumentsAPI.Features.DocumentStructure.Validators
{
    public class NewInnerItemValidator : IValidator
    {
        private readonly NewInnerItem newInnerItem;


        public NewInnerItemValidator(NewInnerItem newInnerItem) => this.newInnerItem = newInnerItem;


        public (bool result, string errorMessage) Validate()
        {
            if (newInnerItem.Position < 0)
            {
                return (false, "Position can't be less than 0");
            }

            if (newInnerItem.Name.Length == 0)
            {
                return (false, "Name's length must be greater than 0");
            }

            return (true, string.Empty);
        }
    }
}