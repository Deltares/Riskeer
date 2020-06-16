using System;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test
{
    [TestFixture]
    public class WaternetCalculationExceptionTest :
        CustomExceptionDesignGuidelinesTestFixture<WaternetCalculationException, Exception> {}
}