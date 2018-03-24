using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class PanelAmountValidation : ValidationRule
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
            if (intValue < 2 || intValue > 31)
            {
                return new ValidationResult(false, "Числа от 2 до 30");
            }
            if (intValue % 2 == 1)
            {
                return new ValidationResult(false, "Только четные числа");
            }
            return ValidationResult.ValidResult;
        }
    }
}
