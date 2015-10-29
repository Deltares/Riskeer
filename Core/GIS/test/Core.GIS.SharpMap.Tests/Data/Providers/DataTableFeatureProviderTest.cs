using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.SharpMap.Data.Providers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.GIS.SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class DataTableFeatureProviderTest
    {
        private readonly MockRepository mocks = new MockRepository();

        [Test]
        public void Contains()
        {
            var provider = new DataTableFeatureProvider("LINESTRING(20 20,40 40)");

            Assert.IsFalse(provider.Contains(mocks.Stub<IFeature>()));
            Assert.IsTrue(provider.Contains((IFeature) provider.Features[0]));
        }
    }
}