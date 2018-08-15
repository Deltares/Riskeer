using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismTypeTest : EnumValuesTestFixture<ExportableFailureMechanismType, int>
    {
        protected override IDictionary<ExportableFailureMechanismType, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableFailureMechanismType, int>
                {
                    {
                        ExportableFailureMechanismType.STBI, 1
                    },
                    {
                        ExportableFailureMechanismType.STBU, 2
                    },
                    {
                        ExportableFailureMechanismType.STPH, 3
                    },
                    {
                        ExportableFailureMechanismType.STMI, 4
                    },
                    {
                        ExportableFailureMechanismType.AGK, 5
                    },
                    {
                        ExportableFailureMechanismType.AWO, 6
                    },
                    {
                        ExportableFailureMechanismType.GEBU, 7
                    },
                    {
                        ExportableFailureMechanismType.GABU, 8
                    },
                    {
                        ExportableFailureMechanismType.GEKB, 9
                    },
                    {
                        ExportableFailureMechanismType.GABI, 10
                    },
                    {
                        ExportableFailureMechanismType.ZST, 11
                    },
                    {
                        ExportableFailureMechanismType.DA, 12
                    },
                    {
                        ExportableFailureMechanismType.HTKW, 13
                    },
                    {
                        ExportableFailureMechanismType.BSKW, 14
                    },
                    {
                        ExportableFailureMechanismType.PKW, 15
                    },
                    {
                        ExportableFailureMechanismType.STKWp, 16
                    },
                    {
                        ExportableFailureMechanismType.STKWl, 17
                    },
                    {
                        ExportableFailureMechanismType.INN, 18
                    }
                };
            }
        }
    }
}