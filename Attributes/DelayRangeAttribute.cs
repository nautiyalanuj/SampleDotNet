using System.ComponentModel.DataAnnotations;

namespace FirstApplication.Attributes
{
    /// <summary>
    /// Custom validation attribute: the decorated integer must be within an inclusive range.
    /// With [ApiController], a failed check automatically returns a 400 response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class DelayRangeAttribute : ValidationAttribute
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public DelayRangeAttribute(int minimum, int maximum)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(minimum, maximum);

            _minimum = minimum;
            _maximum = maximum;
            ErrorMessage = "{0} must be between {1} and {2}.";
        }

        public override bool IsValid(object? value)
        {
            return value is int number && number >= _minimum && number <= _maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _minimum, _maximum);
        }
    }
}
