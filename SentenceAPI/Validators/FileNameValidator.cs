using SentenceAPI.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Validators
{
    public class FileNameValidator : IValidator
    {
        #region Error messages
        private static readonly string firstCharIsNumMsg = "The first char can not be a number.";
        private static readonly string lengthIsOutsideBorders = "The file name's length must be between 5 and 25";
        private static readonly string unacceptedSymbolInFileName = "The file name must contain only English or Russuian letters";
        #endregion

        #region File name properties
        private static readonly string englishLowerLetters = "qwertyuiopasdfghjklzxcvbnm";
        private static readonly string russianLowerLetters = "йцукенгшщзхъфывапролджэячсмитьбю";
        private static readonly string specialSymbols = " _-";
        private static readonly string numbers = "0123456789";
        private static readonly int minLength = 5;
        private static readonly int maxLength = 25;
        #endregion

        private string fileName;

        public FileNameValidator(string fileName)
        {
            this.fileName = fileName;
        }

        public (bool result, string errorMessage) Validate()
        {
            if (fileName == null || fileName.Length < minLength || fileName.Length > maxLength)
            {
                return (false, lengthIsOutsideBorders);
            }

            fileName = fileName.ToLower();

            if (numbers.IndexOf(fileName[0]) > -1)
            {
                return (false, firstCharIsNumMsg);
            }

            for (int i = 0; i < fileName.Length; i++)
            {
                if (englishLowerLetters.IndexOf(fileName[i]) == -1 &&
                    russianLowerLetters.IndexOf(fileName[i]) == -1 &&
                    specialSymbols.IndexOf(fileName[i]) == -1)
                {   
                    return (false, unacceptedSymbolInFileName);
                }
            }

            return (true, string.Empty);
        }
    }
}
