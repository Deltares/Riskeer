using System;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Contribution
{
    /// <summary>
    /// This class represents a failure mechanism which has no representative within Ringtoets but 
    /// contributes to the overall verdict nonetheless.
    /// </summary>
    public class OtherFailureMechanism : BaseFailureMechanism
    {
        /// <summary>
        /// Creates a new instance of <see cref="OtherFailureMechanism"/>.
        /// </summary>
        public OtherFailureMechanism()
        {
            Name = Resources.OtherFailureMechanism_DisplayName;
        }
    }
}