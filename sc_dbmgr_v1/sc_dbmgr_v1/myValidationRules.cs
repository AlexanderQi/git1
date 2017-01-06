using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace sc_dbmgr_v1
{
    public class RequiredRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "不能为空值！");
            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "不能为空字符串！");

            return new ValidationResult(true, null);
        }
    }
}
