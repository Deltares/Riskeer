﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

namespace Ringtoets.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Base class for exceedance probability calculation output.
    /// </summary>
    public abstract class ExceedanceProbabilityCalculationOutputBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExceedanceProbabilityCalculationOutputBase"/>.
        /// </summary>
        /// <param name="ringCombinMethod">The ring combine method used.</param>
        /// <param name="presentationSectionId">The presentation section id used.</param>
        /// <param name="mainMechanismId">The main mechanism id used.</param>
        /// <param name="mainMechanismCombinMethod">The main mechanism combine method used.</param>
        /// <param name="mechanismId">The mechanism id used.</param>
        /// <param name="sectionId">The section id used.</param>
        /// <param name="layerId">The layer id used.</param>
        /// <param name="alternativeId">The alternative id used.</param>
        protected ExceedanceProbabilityCalculationOutputBase(int ringCombinMethod, int presentationSectionId,
                                                             int mainMechanismId, int mainMechanismCombinMethod, int mechanismId,
                                                             int sectionId, int layerId, int alternativeId)
        {
            RingCombinMethod = ringCombinMethod;
            PresentationSectionId = presentationSectionId;
            MainMechanismId = mainMechanismId;
            MainMechanismCombinMethod = mainMechanismCombinMethod;
            MechanismId = mechanismId;
            SectionId = sectionId;
            LayerId = layerId;
            AlternativeId = alternativeId;
        }

        /// <summary>
        /// Gets the ring combine method.
        /// </summary>
        public int RingCombinMethod { get; private set; }

        /// <summary>
        /// Gets the presentation section id.
        /// </summary>
        public int PresentationSectionId { get; private set; }

        /// <summary>
        /// Gets the main mechanism id.
        /// </summary>
        public int MainMechanismId { get; private set; }

        /// <summary>
        /// Gets the main mechanism combine method.
        /// </summary>
        public int MainMechanismCombinMethod { get; private set; }

        /// <summary>
        /// Gets the mechanism id.
        /// </summary>
        public int MechanismId { get; private set; }

        /// <summary>
        /// Gets the section id.
        /// </summary>
        public int SectionId { get; private set; }

        /// <summary>
        /// Gets the layer id.
        /// </summary>
        public int LayerId { get; private set; }

        /// <summary>
        /// Gets the alternative id.
        /// </summary>
        public int AlternativeId { get; private set; }
    }
}