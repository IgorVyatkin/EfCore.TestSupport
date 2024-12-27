// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestSupport;

public class TestTimeThings(ITestOutputHelper output)
{
    [Fact]
    public void TestNoSettings()
    {
        //SETUP
        var mock = new MockOutput();

        //ATTEMPT
        using (new TimeThings(mock))
        {
            Thread.Sleep(10);
        }

        //VERIFY
        mock.LastWriteLine.ShouldStartWith(" took ");
        mock.LastWriteLine.ShouldEndWith("ms.");
    }

    [Fact]
    public void TestMessage()
    {
        //SETUP
        var mock = new MockOutput();

        //ATTEMPT
        using (new TimeThings(mock, "This message"))
        {
            Thread.Sleep(10);
        }

        //VERIFY
        mock.LastWriteLine.ShouldStartWith("This message took ");
        mock.LastWriteLine.ShouldEndWith("ms.");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void TestTime(int milliseconds)
    {
        //SETUP
        var mock = new MockOutput();

        //ATTEMPT
        using (new TimeThings(mock))
        {
            Thread.Sleep(milliseconds);
        }

        //VERIFY
        output.WriteLine($"{milliseconds}: {mock.LastWriteLine}");
    }

    [Fact]
    public void TestMessageAndNumRuns()
    {
        //SETUP
        var mock = new MockOutput();

        //ATTEMPT
        using (new TimeThings(mock, "This message", 500))
        {
            Thread.Sleep(10);
        }

        //VERIFY
        mock.LastWriteLine.ShouldStartWith("500 x This message took ");
        mock.LastWriteLine.ShouldEndWith("us.");
        mock.LastWriteLine.ShouldContain(", ave. per run = ");
    }

    [Fact]
    public void TestTimeThingResultReturn()
    {
        //SETUP
        TimeThingResult result = null;

        //ATTEMPT
        using (new TimeThings(x => result = x, "This message", 10))
        {
            Thread.Sleep(10);
        }

        //VERIFY
        result.Message.ShouldEqual("This message");
        result.NumRuns.ShouldEqual(10);
        result.TotalTimeMilliseconds.ShouldBeInRange(10, 50);
    }

    [Fact]
    public void TestHowLongTimeThings()
    {
        //SETUP
        TimeThingResult result = null;

        //ATTEMPT
        using (new TimeThings(output, "TimeThings", 2))
        {
            using (new TimeThings(x => result = x))
            {

            }
        }

        //VERIFY
    }

    [Fact]
    public void TestTimeThingsMany()
    {
        //SETUP
        TimeThingResult result = null;
        //_output.WriteLine("warm up _output");

        //ATTEMPT
        using (new TimeThings(output, "TimeThings direct 1"))
        {
            Thread.Sleep(10);
        }
        using (new TimeThings(output, "TimeThings direct 2"))
        {
            Thread.Sleep(10);
        }
        using (new TimeThings(x => result = x, "TimeThings redirect 1"))
        {
            Thread.Sleep(10);
        }
        _output.WriteLine(result.ToString());
        using (new TimeThings(x => result = x, "TimeThings redirect 2"))
        {
            Thread.Sleep(10);
        }
        _output.WriteLine(result.ToString());

        //VERIFY
    }


    private class MockOutput : ITestOutputHelper
    {
        public string LastWriteLine { get; private set; }

        public void Write(string message)
        {
            throw new System.NotImplementedException();
        }

        public void Write(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public void WriteLine(string message)
        {
            LastWriteLine = message;
        }

        public void WriteLine(string format, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public string Output { get; }
    }
}