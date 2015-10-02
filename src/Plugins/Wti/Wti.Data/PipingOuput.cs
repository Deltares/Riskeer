using System;

namespace Wti.Data
{
    public class PipingOuput
    {
        public double HeaveFactorOfSafety { get; private set; }
        public double HeaveZValue { get; private set; }
        public double UpliftFactorOfSafety { get; private set; }
        public double UpliftZValue { get; private set; }
        public double SellmeijerFactorOfSafety { get; private set; }
        public double SellmeijerZValue { get; private set; }

        public PipingOuput(double heaveFactorOfSafety, double heaveZValue, double upliftFactorOfSafety, double upliftZValue, double sellmeijerFactorOfSafety, double sellmeijerZValue)
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