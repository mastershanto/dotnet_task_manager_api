using TodoApi.Services;
using Xunit;

namespace TodoApi.UnitTests;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new PasswordHasher();

    [Fact]
    public void Hash_And_Verify_Success()
    {
        var password = "P@ssw0rd!";
        var hash = _hasher.HashPassword(password);
        Assert.True(_hasher.VerifyPassword(password, hash));
    }

    [Fact]
    public void Verify_Fails_For_Wrong_Password()
    {
        var hash = _hasher.HashPassword("correct");
        Assert.False(_hasher.VerifyPassword("incorrect", hash));
    }

    [Fact]
    public void Hash_Format_Is_V1()
    {
        var hash = _hasher.HashPassword("abc123");
        Assert.StartsWith("v1$", hash);
        Assert.Equal(4, hash.Split('$', StringSplitOptions.RemoveEmptyEntries).Length);
    }
}
