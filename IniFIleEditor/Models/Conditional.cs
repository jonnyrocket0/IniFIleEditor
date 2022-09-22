using System;

namespace IniFIleEditor.Models
{
    public class Conditional
    {
        public Conditional(string conditionalString, object conditionalValue)
        {
            ConditionalType = (ConditionalType) Enum.Parse(typeof(ConditionalType), conditionalString, true);
            ConditionalValue = conditionalValue;
        }

        public ConditionalType ConditionalType { get; set; }
        public object? ConditionalValue { get; set; }
    }
}
