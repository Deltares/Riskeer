using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismGroupTest : EnumValuesTestFixture<ExportableFailureMechanismGroup, int>
    {
        protected override IDictionary<ExportableFailureMechanismGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableFailureMechanismGroup, int>
                {
                    {
                        ExportableFailureMechanismGroup.Group1, 1
                    },
                    {
                        ExportableFailureMechanismGroup.Group2, 2
                    },
                    {
                        ExportableFailureMechanismGroup.Group3, 3
                    },
                    {
                        ExportableFailureMechanismGroup.Group4, 4
                    }
                };
            }
        }
    }
}