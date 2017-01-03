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

using Core.Common.Base.Data;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Class that holds all the static dune erosion calculation input parameters.
    /// </summary>
    public class GeneralDuneErosionInput
    {
        private readonly RoundedDouble n;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralDuneErosionInput"/>.
        /// </summary>
        public GeneralDuneErosionInput()
        {
            n = new RoundedDouble(2, 2);
        }

        #region Length effect parameters

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        public RoundedDouble N
        {
            get
            {
                return n;
            }
        }

        #endregion
    }
}