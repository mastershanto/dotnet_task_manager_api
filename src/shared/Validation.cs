using System.ComponentModel.DataAnnotations;

namespace Shared;

public static class Validation
{
    public static IEnumerable<ValidationResult> Validate<T>(T model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var context = new ValidationContext(model, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    public static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<ValidationResult> validationResults)
    {
        return validationResults
            .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty), (result, memberName) => new
            {
                MemberName = string.IsNullOrWhiteSpace(memberName) ? "request" : memberName,
                ErrorMessage = result.ErrorMessage ?? "Validation error"
            })
            .GroupBy(x => x.MemberName)
            .ToDictionary(group => group.Key, group => group.Select(x => x.ErrorMessage).Distinct().ToArray());
    }
}
