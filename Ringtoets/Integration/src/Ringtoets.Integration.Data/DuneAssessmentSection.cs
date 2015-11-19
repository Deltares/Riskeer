﻿using System.Collections.Generic;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The dune-based section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public class DuneAssessmentSection : AssessmentSectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuneAssessmentSection"/> class.
        /// </summary>
        public DuneAssessmentSection()
        {
            Name = Resources.DuneAssessmentSection_DisplayName;

            DuneErosionFailureMechanism = new FailureMechanismPlaceholder(Resources.DuneErosionFailureMechanism_DisplayName);
        }

        /// <summary>
        /// Gets the "Duin erosie" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder DuneErosionFailureMechanism { get; private set; }

        public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return DuneErosionFailureMechanism;
        }
    }
}