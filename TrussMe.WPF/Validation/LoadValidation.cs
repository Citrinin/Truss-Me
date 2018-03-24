using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class LoadValidation:ValidationRule
    {
        readonly Regex _regex = new Regex(@"^([0-9]*[.])?[0-9]+$");
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value == null)
            {
                return new ValidationResult(false, "Поле не может быть пустым");
            }

            if (!_regex.IsMatch(value.ToString()))
            {
                return new ValidationResult
                    (false, "Только положительные числа");
            }
            if (!float.TryParse(value.ToString().Replace(".", ","), out var x))
            {
                return new ValidationResult
                    (false, "Только положительные числа");
            }
            if (x < 0 || x > 1000)
            {
                return new ValidationResult
                    (false, "Только числа от 0 до 1000");
            }
            return ValidationResult.ValidResult;
        }
    }
}
