using System.ComponentModel.DataAnnotations;

namespace Sequence.AspNetCore
{
    internal sealed class EnumAttribute : ValidationAttribute
    {
        public EnumAttribute(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Type '{enumType}' is not an Enum.", nameof(enumType));
            }
        }

        public Type EnumType { get; }

        public override bool IsValid(object? value) => value != null && Enum.IsDefined(EnumType, value);
    }
}
