using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismSectionAssemblyMethodTest : EnumValuesTestFixture<ExportableFailureMechanismSectionAssemblyMethod, int>
    {
        protected override IDictionary<ExportableFailureMechanismSectionAssemblyMethod, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableFailureMechanismSectionAssemblyMethod, int>
                {
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0E1, 1
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0E3, 2
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0G1, 3
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0G3, 4
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0G4, 5
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0G5, 6
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0G6, 7
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T1, 8
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T3, 9
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T4, 10
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T5, 11
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T6, 12
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0T7, 13
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI0A1, 14
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI1A1, 15
                    },
                    {
                        ExportableFailureMechanismSectionAssemblyMethod.WBI1B1, 16
                    }
                };
            }
        }
    }
}