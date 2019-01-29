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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.UITypeEditors;
using Riskeer.Common.Data.DikeProfiles;
using CoreCommonControlsResources = Core.Common.Controls.Properties.Resources;

namespace Riskeer.Common.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// foreshore profile from a collection.
    /// </summary>
    public class ForeshoreProfileEditor : SelectionEditor<IHasForeshoreProfileProperty, ForeshoreProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForeshoreProfileEditor"/>.
        /// </summary>
        public ForeshoreProfileEditor()
        {
            DisplayMember = nameof(ForeshoreProfile.Name);
            NullItem = new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties
            {
                Id = CoreCommonControlsResources.DisplayName_None,
                Name = CoreCommonControlsResources.DisplayName_None
            });
        }

        protected override IEnumerable<ForeshoreProfile> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableForeshoreProfiles();
        }

        protected override ForeshoreProfile GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).ForeshoreProfile;
        }
    }
}