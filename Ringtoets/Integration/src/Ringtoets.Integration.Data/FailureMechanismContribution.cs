using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Utils.Collections;

namespace Ringtoets.Integration.Data
{
    public class FailureMechanismContribution : Observable
    {
        private readonly ICollection<FailureMechanismContributionItem> distribution = new List<FailureMechanismContributionItem>();
        private int norm;

        public FailureMechanismContribution(int norm)
        {
            distribution.Add(new FailureMechanismContributionItem("Dijken - Piping", 0.24, norm));
            distribution.Add(new FailureMechanismContributionItem("Dijken - Graserosie kruin en binnentalud", 0.24, norm));
            distribution.Add(new FailureMechanismContributionItem("Dijken - Macrostabiliteit binnenwaarts", 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem("Kunstwerken - Overslag en overloop", 0.02, norm));
            distribution.Add(new FailureMechanismContributionItem("Kunstwerken - Niet sluiten", 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem("Kunstwerken - Constructief falen", 0.02, norm));
            distribution.Add(new FailureMechanismContributionItem("Dijken - Steenbekledingen", 0.04, norm));
            distribution.Add(new FailureMechanismContributionItem("Dijken - Asfaltbekledingen", 0.03, norm));
            distribution.Add(new FailureMechanismContributionItem("Dijken - Grasbekledingen", 0.03, norm));
            distribution.Add(new FailureMechanismContributionItem("Overig", 0.3, norm));
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