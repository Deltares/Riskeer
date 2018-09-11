using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Common.Primitives.Test
{
    [TestFixture]
    public class ManualFailureMechanismSectionAssemblyCategoryGroupTest : EnumWithResourcesDisplayNameTestFixture<ManualFailureMechanismSectionAssemblyCategoryGroup>
    {
        protected override IDictionary<ManualFailureMechanismSectionAssemblyCategoryGroup, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ManualFailureMechanismSectionAssemblyCategoryGroup, int>
                {
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.None, 1
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, 2
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.Iv, 3
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.IIv, 4
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.Vv, 5
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv, 6
                    }
                };
            }
        }

        protected override IDictionary<ManualFailureMechanismSectionAssemblyCategoryGroup, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<ManualFailureMechanismSectionAssemblyCategoryGroup, string>
                {
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.None, "<selecteer>"
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, "NVT"
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.Iv, "Iv"
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.IIv, "IIv"
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.Vv, "Vv"
                    },
                    {
                        ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv, "VIIv"
                    }
                };
            }
        }
    }
}