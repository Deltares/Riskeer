using System;

namespace Wti.Calculation.Piping
{
    public class PipingCalculationException : Exception
    {
        public PipingCalculationException(string message) : base(message)
        {
        }
    }
}