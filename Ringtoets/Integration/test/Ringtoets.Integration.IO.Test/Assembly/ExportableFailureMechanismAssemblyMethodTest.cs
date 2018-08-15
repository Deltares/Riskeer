using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismAssemblyMethodTest : EnumValuesTestFixture<ExportableFailureMechanismAssemblyMethod, int>
    {
        protected override IDictionary<ExportableFailureMechanismAssemblyMethod, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableFailureMechanismAssemblyMethod, int>
                {
                    {
                        ExportableFailureMechanismAssemblyMethod.WBI2A1, 1
                    },
                    {
                        ExportableFailureMechanismAssemblyMethod.WBI2B1, 2
                    },
                    {
                        ExportableFailureMechanismAssemblyMethod.WBI2C1, 3
                    }
                };
            }
        }
    }
}