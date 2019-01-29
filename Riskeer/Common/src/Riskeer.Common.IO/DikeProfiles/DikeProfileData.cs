// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Ringtoets.Common.Data.DikeProfiles;

namespace Riskeer.Common.IO.DikeProfiles
{
    /// <summary>
    /// Represents the data read from a *.prfl file read by <see cref="DikeProfileDataReader"/>.
    /// </summary>
    public class DikeProfileData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeProfileData"/> class.
        /// </summary>
        public DikeProfileData()
        {
            Orientation = double.NaN;
            DamHeight = double.NaN;
            DikeHeight = double.NaN;
            ForeshoreGeometry = new RoughnessPoint[0];
            DikeGeometry = new RoughnessPoint[0];
        }

        /// <summary>
        /// Gets or sets the identifier used for the dike profile data.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the orientation w.r.t. North of the dike profile.
        /// </summary>
        public double Orientation { get; set; }

        /// <summary>
        /// Gets or sets the descriptor value for the dam at the offshore side.
        /// </summary>
        public DamType DamType { get; set; }

        /// <summary>
        /// Gets or sets the height of the dam.
        /// </summary>
        public double DamHeight { get; set; }

        /// <summary>
        /// Gets or sets the descriptor value of how the profile can be characterized.
        /// </summary>
        public SheetPileType SheetPileType { get; set; }

        /// <summary>
        /// Gets or sets the height of the dike profile at the crest.
        /// </summary>
        public double DikeHeight { get; set; }

        /// <summary>
        /// Gets or sets the notes supplied in the file for this dike profile.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Gets or sets the foreshore geometry in local coordinates with their respective
        /// roughness values.
        /// </summary>
        public RoughnessPoint[] ForeshoreGeometry { get; set; }

        /// <summary>
        /// Gets or sets the dike geometry in local coordinates with their respective
        /// roughness values.
        /// </summary>
        public RoughnessPoint[] DikeGeometry { get; set; }
    }
}