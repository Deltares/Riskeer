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

using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.DikeProfiles
{
    /// <summary>
    /// A collection of <see cref="DikeProfile"/>. The ids of the <see cref="DikeProfile"/>
    /// are unique within the collection.
    /// </summary>
    public class DikeProfileCollection : ObservableUniqueItemCollectionWithSourcePath<DikeProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DikeProfileCollection"/>.
        /// </summary>
        public DikeProfileCollection() : base(profile => profile.Id,
                                              Resources.DikeProfileCollection_TypeDescriptor,
                                              Resources.ProfileCollection_UniqueFeature_id_FeatureDescription) {}
    }
}