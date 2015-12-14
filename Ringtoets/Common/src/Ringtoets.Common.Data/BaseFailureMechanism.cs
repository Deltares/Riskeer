using System;
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// This class is the base implementation for a failure mechanism. Classes which want
    /// to implement IFailureMechanism can and should most likely inherit from this class.
    /// </summary>
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
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentException(Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100, "value");
                }
                contribution = value;
            }
        }

        public string Name { get; protected set; }
    }
}