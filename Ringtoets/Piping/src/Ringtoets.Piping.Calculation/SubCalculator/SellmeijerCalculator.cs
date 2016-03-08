using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="Sellmeijer2011Calculator"/>.
    /// </summary>
    public class SellmeijerCalculator : ISellmeijerCalculator
    {
        private readonly Sellmeijer2011Calculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="SellmeijerCalculator"/>
        /// </summary>
        public SellmeijerCalculator()
        {
            wrappedCalculator = new Sellmeijer2011Calculator();
        }

        public double BeddingAngle
        {
            set
            {
                wrappedCalculator.BeddingAngle = value;
            }
        }

        public double D70
        {
            set
            {
                wrappedCalculator.D70 = value;
            }
        }

        public double D70Mean
        {
            set
            {
                wrappedCalculator.D70Mean = value;
            }
        }

        public double DAquifer
        {
            set
            {
                wrappedCalculator.DAquifer = value;
            }
        }

        public double DarcyPermeability
        {
            set
            {
                wrappedCalculator.DarcyPermeability = value;
            }
        }

        public double DTotal
        {
            set
            {
                wrappedCalculator.DTotal = value;
            }
        }

        public double GammaSubParticles
        {
            set
            {
                wrappedCalculator.GammaSubParticles = value;
            }
        }

        public double Gravity
        {
            set
            {
                wrappedCalculator.Gravity = value;
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

        public double KinematicViscosityWater
        {
            set
            {
                wrappedCalculator.KinematicViscosityWater = value;
            }
        }

        public double ModelFactorPiping
        {
            set
            {
                wrappedCalculator.ModelFactorPiping = value;
            }
        }

        public double Rc
        {
            set
            {
                wrappedCalculator.Rc = value;
            }
        }

        public double SeepageLength
        {
            set
            {
                wrappedCalculator.SeepageLength = value;
            }
        }

        public double VolumetricWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumetricWeightOfWater = value;
            }
        }

        public double WhitesDragCoefficient
        {
            set
            {
                wrappedCalculator.WhitesDragCoefficient = value;
            }
        }

        public double Zp
        {
            get
            {
                return wrappedCalculator.Zp;
            }
        }

        public double FoSp
        {
            get
            {
                return wrappedCalculator.FoSp;
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