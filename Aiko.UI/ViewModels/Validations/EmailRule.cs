using Aiko.UI.Validations;
using System.Text.RegularExpressions;

namespace Aiko.UI.ViewModels.Validations;

public class EmailRule<T> : IValidationRule<T>
{
    private readonly Regex _regex = new(@"^([w.-]+)@([w-]+)((.(w){2,3})+)$");
    public string ValidationMessage { get; set; }

    public bool Check(T value) => value is string str && _regex.IsMatch(str);
}

