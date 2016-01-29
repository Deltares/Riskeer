// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A piping calculation related object.
    /// </summary>
    public interface IPipingCalculationItem : IObservable
    {
        /// <summary>
        /// Gets the name of this calculation object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this calculation item contains piping calculation outputs.
        /// </summary>
        bool HasOutput { get; }
    }
}