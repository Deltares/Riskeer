using Core.Common.Utils.Attributes;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class CalculationConvergenceTest
    {
        [Test]
        public void DisplayName_Always_ReturnExpectedValues()
        {
            // Assert
            Assert.AreEqual("", GetDisplayName(CalculationConvergence.NotCalculated));
            Assert.AreEqual("Nee", GetDisplayName(CalculationConvergence.CalculatedNotConverged));
            Assert.AreEqual("Ja", GetDisplayName(CalculationConvergence.CalculatedConverged));
        }

        private string GetDisplayName(CalculationConvergence value)
        {
            var type = typeof(CalculationConvergence);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(ResourcesDisplayNameAttribute), false);
            return ((ResourcesDisplayNameAttribute) attributes[0]).DisplayName;
        }
    }
}