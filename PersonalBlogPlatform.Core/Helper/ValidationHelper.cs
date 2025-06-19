using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.Helper
{
    /// <summary>
    /// Model Validation Class , contain an extinction method "Model Validation".
    /// </summary>
    public class ValidationHelper
    {
        public static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
            }
        }
    }
}
