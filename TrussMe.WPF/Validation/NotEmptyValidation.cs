using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TrussMe.WPF.Validation
{
    class NotEmptyValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || value.ToString().Length==0)
            {
                return new ValidationResult(false, "Поле не может быть пустым");
            }

            return ValidationResult.ValidResult;
        }
    }
}
