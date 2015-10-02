using System;

namespace Wti.Data
{
    public class PipingOutput
    {
        public double HeaveFactorOfSafety { get; private set; }
        public double HeaveZValue { get; private set; }
        public double UpliftFactorOfSafety { get; private set; }
        public double UpliftZValue { get; private set; }
        public double SellmeijerFactorOfSafety { get; private set; }
        public double SellmeijerZValue { get; private set; }

        public PipingOutput(double upliftZValue, double upliftFactorOfSafety, double heaveZValue, double heaveFactorOfSafety, double sellmeijerZValue, double sellmeijerFactorOfSafety)
        {
            HeaveFactorOfSafety = heaveFactorOfSafety;
            HeaveZValue = heaveZValue;
            UpliftFactorOfSafety = upliftFactorOfSafety;
            UpliftZValue = upliftZValue;
            SellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            SellmeijerZValue = sellmeijerZValue;
        }
    }
}