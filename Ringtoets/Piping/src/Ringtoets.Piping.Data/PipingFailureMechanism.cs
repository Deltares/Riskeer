﻿using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : BaseFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
        {
            SectionDivisions = new InputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_SectionDevisions_DisplayName);
            SurfaceLines = new ObservableList<RingtoetsPipingSurfaceLine>();
            SoilProfiles = new ObservableList<PipingSoilProfile>();
            BoundaryConditions = new InputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_BoundaryConditions_DisplayName);
            var pipingCalculationGroup = new PipingCalculationGroup(Resources.PipingFailureMechanism_Calculations_DisplayName, false);
            pipingCalculationGroup.Children.Add(new PipingCalculation());
            CalculationsGroup = pipingCalculationGroup;
            AssessmentResult = new OutputPlaceholder(RingtoetsCommonDataResources.FailureMechanism_AssessmentResult_DisplayName);
            Name = Resources.PipingFailureMechanism_DisplayName;
        }

        /// <summary>
        /// Gets the subdivision of the assessment section for which the piping failure mechanism is calculating.
        /// </summary>
        public InputPlaceholder SectionDivisions { get; private set; }

        /// <summary>
        /// Gets the available <see cref="RingtoetsPipingSurfaceLine"/> within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> SurfaceLines { get; private set; }

        /// <summary>
        /// Gets the available profiles within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable<PipingSoilProfile> SoilProfiles { get; private set; }

        /// <summary>
        /// Gets the boundary conditions applying to the piping failure mechanism.
        /// </summary>
        public InputPlaceholder BoundaryConditions { get; private set; }

        /// <summary>
        /// Gets all available piping calculations.
        /// </summary>
        public PipingCalculationGroup CalculationsGroup { get; private set; }

        /// <summary>
        /// Gets the calculation results for this failure mechanism.
        /// </summary>
        public OutputPlaceholder AssessmentResult { get; private set; }
    }
}