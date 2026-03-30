using System.ComponentModel.DataAnnotations;

namespace Aiko.UI.Validations;

public class ValidateTestAttribute:ValidationAttribute
{
    //属性MaxAge和MinAge，将通过特性参数赋值
    public int MaxAge { get; }
    public int MinAge { get; }


    public ValidateTestAttribute(int maxAge, int minAge)
    {
        MaxAge = maxAge;
        MinAge = minAge;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        int age = (int)value;
        if (age >= MinAge && age <= MaxAge)
        {
            return ValidationResult.Success;
        }
        return new($"年龄最小{MinAge},最大{MaxAge}");
    }
}

