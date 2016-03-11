namespace Ringtoets.Piping.Calculation.SubCalculator
{
    /// <summary>
    /// Class which wraps the <see cref="Deltares.WTIPiping.PiezoHeadCalculator"/>.
    /// </summary>
    public class PiezoHeadCalculator : IPiezoHeadCalculator {

        public double PhiPolder { private get; set; }

        public double RExit { private get; set; }

        public double HRiver { private get; set; }

        public double PhiExit { get; private set; }

        public void Calculate()
        {
            PhiExit = Deltares.WTIPiping.PiezoHeadCalculator.CalculatePhiExit(PhiPolder, RExit, HRiver);
        }
    }
}