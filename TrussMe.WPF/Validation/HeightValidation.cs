using System.Globalization;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class HeightValidation:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Поле не может быть пустым");
            }

            var stringValue = value.ToString();

            if (!int.TryParse(stringValue, out var intValue))
            {
                return new ValidationResult(false, "Только целые числа");
            }
            if (intValue < 300 || intValue > 5000)
            {
                return new ValidationResult(false, "Числа от 300 до 5000");
            }
            return ValidationResult.ValidResult;
        }
    }
}
