﻿using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real piezometric head at exit sub calculator of piping.
    /// </summary>
    public class PiezoHeadCalculatorStub : IPiezoHeadCalculator
    {
        public double PhiPolder { get; set; }
        public double RExit { get; set; }
        public double HRiver { get; set; }
        public double PhiExit { get; private set; }

        public void Calculate() {}
    }
}