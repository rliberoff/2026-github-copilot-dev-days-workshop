using Sample;
using Xunit;

namespace Sample.Tests;

public sealed class GreeterTests
{
    [Fact]
    public void Greet_ReturnsExpectedMessage()
    {
        var greeter = new Greeter();

        var result = greeter.Greet("World");

        Assert.Equal("Hello, World!", result);
    }
}
