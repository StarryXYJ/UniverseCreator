using System;
using JetBrains.Annotations;
using WorldCreator.Model.Time;
using Xunit;

namespace WorldCreator.Tests.Model.Time;

[TestSubject(typeof(CustomDateTime))]
public class CustomDateTimeTest
{
    [Fact]
    public void CreateTest()
    {
        var t1 = new CustomDateTime(0, 0, 0, GregorianCalendarRule.Instance);
        var t2 = new CustomDateTime(0, 13, 0, GregorianCalendarRule.Instance);
        var t3 = new CustomDateTime(0, 1, 32, GregorianCalendarRule.Instance);
        Assert.Skip("Pass");
    }
}