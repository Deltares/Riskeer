using System.Collections.Generic;

namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="Deltares.WTIPiping.HeaveCalculator"/>.
    /// </summary>
    public class HeaveCalculator : IHeaveCalculator
    {
        private readonly Deltares.WTIPiping.HeaveCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="HeaveCalculator"/>.
        /// </summary>
        public HeaveCalculator()
        {
            wrappedCalculator = new Deltares.WTIPiping.HeaveCalculator();
        }

        public double DTotal
        {
            set
            {
                wrappedCalculator.DTotal = value;
            }
        }

        public double HExit
        {
            set
            {
                wrappedCalculator.HExit = value;
            }
        }

        public double Ich
        {
            set
            {
                wrappedCalculator.Ich = value;
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

        public double Zh
        {
            get
            {
                return wrappedCalculator.Zh;
            }
        }

        public double FoSh
        {
            get
            {
                return wrappedCalculator.FoSh;
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