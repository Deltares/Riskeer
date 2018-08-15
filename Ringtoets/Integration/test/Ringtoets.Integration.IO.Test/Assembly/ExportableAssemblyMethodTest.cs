using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAssemblyMethodTest : EnumValuesTestFixture<ExportableAssemblyMethod, int>
    {
        protected override IDictionary<ExportableAssemblyMethod, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<ExportableAssemblyMethod, int>
                {
                     {
                        ExportableAssemblyMethod.WBI0E1, 1
                    },
                    {
                        ExportableAssemblyMethod.WBI0E3, 2
                    },
                    {
                        ExportableAssemblyMethod.WBI0G1, 3
                    },
                    {
                        ExportableAssemblyMethod.WBI0G3, 4
                    },
                    {
                        ExportableAssemblyMethod.WBI0G4, 5
                    },
                    {
                        ExportableAssemblyMethod.WBI0G5, 6
                    },
                    {
                        ExportableAssemblyMethod.WBI0G6, 7
                    },
                    {
                        ExportableAssemblyMethod.WBI0T1, 8
                    },
                    {
                        ExportableAssemblyMethod.WBI0T3, 9
                    },
                    {
                        ExportableAssemblyMethod.WBI0T4, 10
                    },
                    {
                        ExportableAssemblyMethod.WBI0T5, 11
                    },
                    {
                        ExportableAssemblyMethod.WBI0T6, 12
                    },
                    {
                        ExportableAssemblyMethod.WBI0T7, 13
                    },
                    {
                        ExportableAssemblyMethod.WBI0A1, 14
                    },
                    {
                        ExportableAssemblyMethod.WBI1A1, 15
                    },
                    {
                        ExportableAssemblyMethod.WBI1B1, 16
                    },
                    {
                        ExportableAssemblyMethod.WBI2A1, 17
                    },
                    {
                        ExportableAssemblyMethod.WBI2B1, 18
                    },
                    {
                        ExportableAssemblyMethod.WBI2C1, 19
                    },
                    {
                        ExportableAssemblyMethod.WBI3A1, 20
                    },
                    {
                        ExportableAssemblyMethod.WBI3B1, 21
                    },
                    {
                        ExportableAssemblyMethod.WBI3C1, 22
                    }
                };
            }
        }
    }
}