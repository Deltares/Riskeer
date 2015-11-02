﻿namespace Ringtoets.Piping.Data
{
    public class PipingSoilLayer
    {
        public PipingSoilLayer(double top)
        {
            Top = top;
        }

        public double Top { get; private set; }
        public bool IsAquifer { get; set; }

        public double? AbovePhreaticLevel { get; set; }
        public double? BelowPhreaticLevel { get; set; }
        public double? DryUnitWeight { get; set; }
    }
}