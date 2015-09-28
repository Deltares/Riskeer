using GeoAPI.Extensions.Feature;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class DataTableFeatureProviderTest
    {
        readonly MockRepository mocks = new MockRepository();
        [Test]
        public void Contains()
        {
            var provider = new DataTableFeatureProvider("LINESTRING(20 20,40 40)");

            Assert.IsFalse(provider.Contains(mocks.Stub<IFeature>()));
            Assert.IsTrue(provider.Contains((IFeature) provider.Features[0]));
        }
    }
}
