using NUnit.Framework;
using SharpMap.Data;
using SharpTestsEx;

namespace SharpMap.Tests.Data
{
    [TestFixture]
    public class FeatureDataRowAttributeAccessorTest
    {
        [Test]
        public void KeepColumnNamesInSync()
        {
            var table = new FeatureDataTable();
            var row = table.NewRow();
            table.Rows.Add(row);

            var accessor = new FeatureDataRowAttributeAccessor(row);

            // now add column and check if it is available via accessor
            table.Columns.Add("Name", typeof (string));

            accessor.Count
                .Should().Be.EqualTo(1);
        }
    }
}