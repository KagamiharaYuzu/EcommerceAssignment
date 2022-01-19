using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace KLH60Manager.Shared
{
    public class CustomInputSelect<TVal> : InputSelect<TVal>
    {
        protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TVal result, [NotNullWhen(false)] out string validationErrorMessage)
        {
            if (typeof(TVal) == typeof(int))
            {
                if (int.TryParse(value, out int res))
                {
                    result = (TVal)(object)res;
                    validationErrorMessage = null;
                    return true;
                }
                else
                {
                    result = default;
                    validationErrorMessage = $"The selected value of: {value} is not a valid number.";
                    return false;
                }
            }
            else return base.TryParseValueFromString(value, out result, out validationErrorMessage);
        }
    }
}