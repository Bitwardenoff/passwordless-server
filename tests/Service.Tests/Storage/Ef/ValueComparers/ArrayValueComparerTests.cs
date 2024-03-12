using Passwordless.Service.Storage.Ef.ValueComparers;

namespace Passwordless.Service.Tests.Storage.Ef.ValueComparers;

public class ArrayValueComparerTests
{
    [Fact]
    public void NullableArrayValueComparer_ComparingSameArrays_ReturnsExpectedResult()
    {
        // Arrange
        var sut = new NullableArrayValueComparer<string>();

        // Act
        var actual = sut.Equals(["a", "b"], ["a", "b"]);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NullableArrayValueComparer_ComparingDifferentArrays_ReturnsExpectedResult()
    {
        // Arrange
        var sut = new NullableArrayValueComparer<string>();

        // Act
        var actual = sut.Equals(["a", "b"], ["a", "c"]);

        // Assert
        Assert.False(actual);
    }
}