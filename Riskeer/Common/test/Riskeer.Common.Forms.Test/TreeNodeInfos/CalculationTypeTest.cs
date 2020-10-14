using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.TreeNodeInfos;

namespace Riskeer.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class CalculationTypeTest : EnumValuesTestFixture<CalculationType, int>
    {
        protected override IDictionary<CalculationType, int> ExpectedValueForEnumValues =>
            new Dictionary<CalculationType, int>
            {
                {
                    CalculationType.SemiProbabilistic, 1
                },
                {
                    CalculationType.Probabilistic, 2
                },
                {
                    CalculationType.Hydraulic, 3
                }
            };
    }
}