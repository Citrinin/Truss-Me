using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class IntegerValidation: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value == null)
            {
                return new ValidationResult(false, "Поле не может быть пустым");
            }
            var reg = new Regex(@"^([0-9]+)$");
            if (!reg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "Только целые числа");
            }
            return ValidationResult.ValidResult;
        }
    }
}
