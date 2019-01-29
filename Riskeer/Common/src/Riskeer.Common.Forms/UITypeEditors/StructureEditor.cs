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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Gui.UITypeEditors;
using Ringtoets.Common.Data;

namespace Riskeer.Common.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// structure from a collection.
    /// </summary>
    /// <typeparam name="T">The type of structures at stake.</typeparam>
    public class StructureEditor<T> : SelectionEditor<IHasStructureProperty<T>, T> where T : StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructureEditor{T}"/>.
        /// </summary>
        public StructureEditor()
        {
            DisplayMember = nameof(StructureBase.Name);
        }

        protected override IEnumerable<T> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableStructures();
        }

        protected override T GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).Structure;
        }
    }
}