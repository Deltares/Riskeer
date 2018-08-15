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
                        ExportableFailureMechanismAssemblyMethod.WBI1A1, 1
                    },
                    {
                        ExportableFailureMechanismAssemblyMethod.WBI1B1, 2
                    }
                };
            }
        }
    }
}