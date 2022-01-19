using System;
using System.ComponentModel.DataAnnotations;

namespace StoreClassLibrary
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as string;
            return !string.IsNullOrEmpty(inputValue);
        }
    }
}