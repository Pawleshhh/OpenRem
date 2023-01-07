using NUnit.Framework;

namespace OpenRem.Common.Test;

[TestFixture]
public class IntegrationTest
{
    [Test]
    public void Hardcoded_AssemblyNames_AreCorrect()
    {
        Assert.AreEqual("OpenRem.Engine", AppDomainHelper.EngineAssemblyName);
        Assert.AreEqual("OpenRem.Engine.Interface", AppDomainHelper.EngineInterfaceAssemblyName);
    }
}