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
    class SlopeValidation:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            if (value == null)
            {
                return new ValidationResult(false, "Поле не может быть пустым");
            }
            var reg = new Regex(@"^([0-9]*[.])?[0-9]+$");
            if (!reg.IsMatch(value.ToString()))
            {
                return new ValidationResult
                    (false, "Только числа");
            }
            if (!float.TryParse(value.ToString().Replace(".",","),out var x))
            {
                return new ValidationResult
                    (false, "Только числа");
            }
            if (x<0||x>0.81)
            {
                return new ValidationResult
                    (false, "Только числа от 0 до 0.8");
            }
            return ValidationResult.ValidResult;
        }
    }
}
