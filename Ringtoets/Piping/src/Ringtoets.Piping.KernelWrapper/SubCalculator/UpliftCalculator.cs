using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="WTIUpliftCalculator"/>.
    /// </summary>
    public class UpliftCalculator : IUpliftCalculator
    {
        private readonly WTIUpliftCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftCalculator"/>
        /// </summary>
        public UpliftCalculator()
        {
            wrappedCalculator = new WTIUpliftCalculator();
        }

        public double EffectiveStress
        {
            set
            {
                wrappedCalculator.EffectiveStress = value;
            }
        }

        public double HExit
        {
            set
            {
                wrappedCalculator.HExit = value;
            }
        }

        public double HRiver
        {
            set
            {
                wrappedCalculator.HRiver = value;
            }
        }

        public double ModelFactorUplift
        {
            set
            {
                wrappedCalculator.ModelFactorUplift = value;
            }
        }

        public double PhiExit
        {
            set
            {
                wrappedCalculator.PhiExit = value;
            }
        }

        public double PhiPolder
        {
            set
            {
                wrappedCalculator.PhiPolder = value;
            }
        }

        public double RExit
        {
            set
            {
                wrappedCalculator.RExit = value;
            }
        }

        public double VolumetricWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumetricWeightOfWater = value;
            }
        }

        public double Zu
        {
            get
            {
                return wrappedCalculator.Zu;
            }
        }

        public double FoSu
        {
            get
            {
                return wrappedCalculator.FoSu;
            }
        }

        public void Calculate()
        {
            wrappedCalculator.Calculate();
        }

        public List<string> Validate()
        {
            return wrappedCalculator.Validate();
        }
    }
}