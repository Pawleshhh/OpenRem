using NUnit.Framework;
using System.Linq;

namespace OpenRem.Emulator.Test;

[TestFixture]
public class IntegrationTest
{
    [Test]
    public void ShouldHaveAtLeastOneRawFile()
    {
        var embeddedSample = new EmbeddedSample();
        Assert.GreaterOrEqual(embeddedSample.GetSamples().Count(), 1);
    }
}