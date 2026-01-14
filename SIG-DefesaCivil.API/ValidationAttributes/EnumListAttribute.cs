using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.ValidationAttributes
{
    public class EnumListAttribute : ValidationAttribute
    {
        public readonly Type _enumType;

        public EnumListAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            var stringList = value as List<string>;
            if (stringList == null) return ValidationResult.Success;

            var invalidValues = new List<string>();

            foreach (var item in stringList)
            {
                if (!Enum.IsDefined(_enumType, item))
                {
                    invalidValues.Add(item);
                }
            }

            if (invalidValues.Any())
            {
                var invalidString = string.Join(", ", invalidValues);
                return new ValidationResult($"Os seguintes valores são inválidos para o tipo {_enumType.Name}: {invalidString}.");
            }

            return ValidationResult.Success;
        }
    }
}