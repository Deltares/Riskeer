using System;
using System.Linq;
using DelftTools.Utils.Binding;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Binding
{
    [TestFixture]
    public class EnumBindingHelperTest
    {
        [Test]
        public void GetEnumValuesForEnumType()
        {
            var list = EnumBindingHelper.ToList<SampleEnum>();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(new[] { SampleEnum.Sinterklaas, SampleEnum.Kerstman },
                list.Select(kvp => kvp.Key).ToArray());
            Assert.AreEqual(new[] { "De goedheilig man", "De kerstman" },
                list.Select(kvp => kvp.Value).ToArray());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumValuesForNonEnumStructThrowsException()
        {
            //datetime is a struct but not an enum. Would be nice to get compile time checking 
            //but for now this is OK
            var list = EnumBindingHelper.ToList<DateTime>();
        }
    }
}