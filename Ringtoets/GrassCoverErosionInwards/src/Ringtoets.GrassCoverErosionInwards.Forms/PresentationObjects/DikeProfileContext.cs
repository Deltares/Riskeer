// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// This is a presentation object for a <see cref="DikeProfile"/> instance.
    /// </summary>
    public class DikeProfileContext : WrappedObjectContextBase<DikeProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeProfileContext"/> class.
        /// </summary>
        /// <param name="dikeProfile">The dike profile to wrap.</param>
        /// <param name="dikeProfilesList">The observable list of dike profiles the context belongs to.</param>
        /// <exception cref="ArgumentNullException">When either <paramref name="dikeProfile"/>
        /// or <paramref name="dikeProfilesList"/> is null.</exception>
        public DikeProfileContext(DikeProfile dikeProfile, ObservableList<DikeProfile> dikeProfilesList)
            : base(dikeProfile)
        {
            if (dikeProfilesList == null)
            {
                throw new ArgumentNullException("dikeProfilesList");
            }
            DikeProfilesList = dikeProfilesList;
        }

        /// <summary>
        /// Gets the observable list of dike profiles.
        /// </summary>
        public ObservableList<DikeProfile> DikeProfilesList { get; private set; }
    }
}