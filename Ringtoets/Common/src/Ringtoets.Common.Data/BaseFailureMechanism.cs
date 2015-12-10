using System;
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data
{

    public abstract class BaseFailureMechanism : Observable, IFailureMechanism
    {
        private double contribution;

        public double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException(Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100, "value");
                }
                contribution = value;
            }
        }

        public string Name { get; set; }
    }
}