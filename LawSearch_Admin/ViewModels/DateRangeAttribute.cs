using System.ComponentModel.DataAnnotations;

namespace LawSearch_Admin.ViewModels
{
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (FormDateModel)validationContext.ObjectInstance;

            if(model.fromDate > model.toDate)
            {
                return new ValidationResult("From date must be earlier than end date.");
            }
            return ValidationResult.Success;
        }
    }
}
