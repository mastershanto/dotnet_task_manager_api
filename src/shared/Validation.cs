using System.ComponentModel.DataAnnotations;

namespace Shared;

public static class Validation
{
    public static IEnumerable<ValidationResult> Validate<T>(T model)
    {
        var context = new ValidationContext(model, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
