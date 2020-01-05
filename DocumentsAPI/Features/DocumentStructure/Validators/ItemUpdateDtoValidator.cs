using DocumentsAPI.Features.DocumentStructure.Models;
using DocumentsAPI.Models.DocumentStructureModels;

using SharedLibrary.KernelInterfaces;


namespace DocumentsAPI.Features.DocumentStructure.Validators
{
    public class ItemUpdateDtoValidator : IValidator
    {
        private readonly ItemUpdateDto itemUpdateDto;
        private readonly DocumentStructureModel documentStructure;
        private readonly IValidator newInnerItemValidator;


        public ItemUpdateDtoValidator(ItemUpdateDto itemUpdateDto, DocumentStructureModel documentStructure)
        {
            this.itemUpdateDto = itemUpdateDto;
            this.documentStructure = documentStructure;

            newInnerItemValidator = new NewInnerItemValidator(itemUpdateDto.NewInnerItem);
        }


        public (bool result, string errorMessage) Validate()
        {
            var validationResult = (result: true, errorMessage: string.Empty);

            if (itemUpdateDto.ParentDocumentStructureID != documentStructure.ID)
            {
                validationResult.result = false;
                validationResult.errorMessage += " The document structure ID doesn't equal itemUpdateDto ID.";
            }

            var newInnerValidatorResult = newInnerItemValidator.Validate();

            validationResult.result &= newInnerValidatorResult.result;
            validationResult.errorMessage += " " + newInnerValidatorResult.errorMessage;

            return validationResult;
        }
    }
}