// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;

namespace Core.Gui.Plugin
{
    /// <summary>
    /// Information for setting the application into a specific state.
    /// </summary>
    public class StateInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="StateInfo"/>.
        /// </summary>
        /// <param name="name">The name of the state.</param>
        /// <param name="symbol">The symbol of the state.</param>
        /// <param name="getRootData">The method for obtaining the root data of the state.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="getRootData"/> is <c>null</c>.</exception>
        public StateInfo(string name, string symbol, Func<IProject, object> getRootData)
        {
            Name = name;
            Symbol = symbol;
            GetRootData = getRootData ?? throw new ArgumentNullException(nameof(getRootData));
        }

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the symbol of the state.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the method for obtaining the root data of the state. Function arguments:
        /// <list type="number">
        ///     <item>The current <see cref="IProject"/> to get the state data from.</item>
        ///     <item>out - The root data object of the state.</item>
        /// </list>
        /// </summary>
        public Func<IProject, object> GetRootData { get; }
    }
}