using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class SpanValidation:ValidationRule
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
            if (intValue < 5000 || intValue > 100000)
            {
                return new ValidationResult(false, "Числа от 5000 до 100000");
            }
            return ValidationResult.ValidResult;
        }
    }
}
