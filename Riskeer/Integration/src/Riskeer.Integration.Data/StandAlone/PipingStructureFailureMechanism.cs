﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Piping Structure failure mechanism.
    /// </summary>
    public class PipingStructureFailureMechanism : FailureMechanismBase, IHasSectionResults<PipingStructureFailureMechanismSectionResultOld>
    {
        private const int numberOfDecimalPlacesN = 2;

        private static readonly Range<RoundedDouble> validityRangeN = new Range<RoundedDouble>(new RoundedDouble(numberOfDecimalPlacesN, 1),
                                                                                               new RoundedDouble(numberOfDecimalPlacesN, 20));

        private readonly ObservableList<PipingStructureFailureMechanismSectionResultOld> sectionResults;
        private RoundedDouble n;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingStructureFailureMechanism"/> class.
        /// </summary>
        public PipingStructureFailureMechanism()
            : base(Resources.PipingStructureFailureMechanism_DisplayName, Resources.PipingStructureFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<PipingStructureFailureMechanismSectionResultOld>();
            n = new RoundedDouble(numberOfDecimalPlacesN, 1.0);
        }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value of <see cref="N"/>
        /// is not in the range [1, 20].</exception>
        public RoundedDouble N
        {
            get
            {
                return n;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(n.NumberOfDecimalPlaces);
                if (!validityRangeN.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(RiskeerCommonDataResources.N_Value_should_be_in_Range_0_,
                                                                                       validityRangeN));
                }

                n = newValue;
            }
        }

        /// <summary>
        /// Gets whether the length effect should be applied in the section.
        /// </summary>
        public bool ApplyLengthEffectInSection => false;

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<PipingStructureFailureMechanismSectionResultOld> SectionResultsOld
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            base.AddSectionDependentData(section);
            sectionResults.Add(new PipingStructureFailureMechanismSectionResultOld(section));
        }

        protected override void ClearSectionDependentData()
        {
            sectionResults.Clear();
        }
    }
}