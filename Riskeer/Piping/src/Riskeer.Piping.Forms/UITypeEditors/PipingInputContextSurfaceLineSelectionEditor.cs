﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Gui.UITypeEditors;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="PipingSurfaceLine"/> from a collection.
    /// </summary>
    /// <typeparam name="T">The type of object properties.</typeparam>
    public class PipingInputContextSurfaceLineSelectionEditor<T> : SelectionEditor<T, PipingSurfaceLine> where T : IHasSurfaceLineProperty
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContextSurfaceLineSelectionEditor{T}"/>.
        /// </summary>
        public PipingInputContextSurfaceLineSelectionEditor()
        {
            DisplayMember = nameof(PipingSurfaceLine.Name);
        }

        protected override IEnumerable<PipingSurfaceLine> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableSurfaceLines();
        }

        protected override PipingSurfaceLine GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).SurfaceLine;
        }
    }
}