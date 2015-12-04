namespace Ringtoets.Integration.Data
{
    public class FailureMechanismContributionItem
    {
        public FailureMechanismContributionItem(string assessment, double probability, int norm)
        {
            Assessment = assessment;
            Probability = probability;
            Norm = norm;
        }

        public string Assessment { get; private set; }
        public double Probability { get; private set; }

        public double ProbabilityPerYear
        {
            get
            {
                return Norm / Probability;
            }
        }

        public double Norm { get; set; }
    }
}