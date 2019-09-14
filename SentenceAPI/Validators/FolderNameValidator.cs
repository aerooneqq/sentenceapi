using SentenceAPI.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Validators
{
    public class FolderNameValidator : IValidator
    {
        #region Error messages
        private static readonly string lengthIsOutsideBorders = "The length of the folder name must be between 5 and 25";
        private static readonly string unacceptableSymbolInName = "The name must contain only English and Russian letters";
        private static readonly string firstCharIsNumber = "The folder name must start with a number";
        #endregion

        #region Folder name properties
        private static readonly int minLength = 5;
        private static readonly int maxLength = 25;
        private static readonly string englishLowerLetters = "qwertyuiopasdfghjklzxcvbnm";
        private static readonly string russianLowerLetters = "йцукенгшщзхъфывапролджэячсмитьбю";
        private static readonly string numbers = "1234567890";
        #endregion

        private string folderName;

        public FolderNameValidator(string folderName)
        {
            this.folderName = folderName;
        }

        public (bool result, string errorMessage) Validate()
        {
            if (folderName == null || folderName.Length > maxLength || folderName.Length < minLength)
            {
                return (false, lengthIsOutsideBorders);
            }

            folderName = folderName.ToLower();

            if (numbers.IndexOf(folderName[0]) > -1)
            {
                return (false, firstCharIsNumber);
            }

            for (int i = 0; i < folderName.Length; i++)
            {
                if (englishLowerLetters.IndexOf(folderName[i]) == -1 && 
                    russianLowerLetters.IndexOf(folderName[i]) == -1)
                {
                    return (false, unacceptableSymbolInName);
                }
            }

            return (true, string.Empty);
        }
    }
}
