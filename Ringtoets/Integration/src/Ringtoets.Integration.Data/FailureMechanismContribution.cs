using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Utils.Collections;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data
{
    public class FailureMechanismContribution : Observable
    {
        private readonly ICollection<FailureMechanismContributionItem> distribution = new List<FailureMechanismContributionItem>();
        private int norm;

        public FailureMechanismContribution(int norm)
        {
            distribution.Add(new FailureMechanismContributionItem(Piping.Data.Properties.Resources.PipingFailureMechanism_DisplayName, 0.24, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.GrassErosionFailureMechanism_DisplayName, 0.24, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.MacrostabilityInwardFailureMechanism_DisplayName, 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.OvertoppingFailureMechanism_DisplayName, 0.02, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.ClosingFailureMechanism_DisplayName, 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.FailingOfConstructionFailureMechanism_DisplayName, 0.02, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.StoneRevetmentFailureMechanism_DisplayName, 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.AsphaltRevetmentFailureMechanism_DisplayName, 0.03, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.GrassRevetmentFailureMechanism_DisplayName, 0.03, norm));
            distribution.Add(new FailureMechanismContributionItem(Resources.FailureMechanismContribution_FailureMechanismContribution_Other, 0.3, norm));
            Norm = norm;
        }

        public int Norm
        {
            get
            {
                return norm;
            }
            set
            {
                norm = value;
                distribution.ForEach(d => d.Norm = norm);
            }
        }

        public IEnumerable<FailureMechanismContributionItem> Distribution
        {
            get
            {
                return distribution;
            }
        }
    }
}