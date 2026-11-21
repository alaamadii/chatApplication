using Xunit;

namespace ChatApplication.Core.Tests;

public sealed class UsernameRulesTests
{
    [Theory]
    [InlineData("  Alaa  ", "Alaa")]
    [InlineData("علاء", "علاء")]
    [InlineData("Çağrı_19", "Çağrı_19")]
    [InlineData("A B-2", "A B-2")]
    [InlineData("abcdefghijklmnopqrstuvwx", "abcdefghijklmnopqrstuvwx")]
    public void Accepts_only_supported_names_and_trims_ordinary_outer_spaces(string input, string expected)
    {
        Assert.True(UsernameRules.TryNormalize(input, out var name));
        Assert.Equal(expected, name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abcdefghijklmnopqrstuvwxy")]
    [InlineData(" Server ")]
    [InlineData("sYsTeM")]
    [InlineData("\tAlaa")]
    [InlineData("Alaa\r")]
    [InlineData("Ali\u200f")]
    [InlineData("\u00a0Ali")]
    [InlineData("a\u0301")]
    [InlineData("😀")]
    [InlineData("ab\0")]
    [InlineData("A!")]
    public void Rejects_invalid_original_input_before_trimming(string? input)
    {
        Assert.False(UsernameRules.TryNormalize(input, out var name));
        Assert.Empty(name);
    }
}
