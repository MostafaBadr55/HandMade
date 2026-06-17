using System.ComponentModel.DataAnnotations;

namespace HandMade.CustomAttributes
{
    public class AllowedValuesAttribute<TEnum> : ValidationAttribute
    where TEnum : struct, Enum
    {
        private readonly TEnum[] _allowed;

        public AllowedValuesAttribute(params object[] allowed)
        {
            _allowed = allowed.Cast<TEnum>().ToArray();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
        {
            if (value is TEnum enumValue && _allowed.Contains(enumValue))
                return ValidationResult.Success;

            var allowedNames = string.Join(", ", _allowed.Select(v => v.ToString()));
            return new ValidationResult(
                $"Value must be one of: {allowedNames}.",
                [ctx.MemberName!]);
        }
    }
}
