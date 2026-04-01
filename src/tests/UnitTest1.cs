using App.Features.User.Domain;
using Shared;

namespace Api.Tests;

public class ValidationAndResultTests
{
    [Fact]
    public void Validate_ReturnsErrors_ForInvalidModel()
    {
        var invalid = new UserModel
        {
            Name = string.Empty,
            Email = "invalid-email"
        };

        var results = Validation.Validate(invalid).ToArray();

        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserModel.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserModel.Email)));
    }

    [Fact]
    public void ToErrorDictionary_UsesRequestFallback_WhenMemberNameMissing()
    {
        var validationResults = new[]
        {
            new System.ComponentModel.DataAnnotations.ValidationResult("General error")
        };

        var map = Validation.ToErrorDictionary(validationResults);

        Assert.True(map.ContainsKey("request"));
        Assert.Contains("General error", map["request"]);
    }

    [Fact]
    public void Result_Failure_SetsFailureState()
    {
        var result = Result<int>.Failure("boom");

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(default, result.Value);
        Assert.Contains("boom", result.Errors);
    }
}
