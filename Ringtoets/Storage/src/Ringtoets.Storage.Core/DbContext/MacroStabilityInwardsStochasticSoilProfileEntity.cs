// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Ringtoets.Storage.Core.DbContext
{
    public class MacroStabilityInwardsStochasticSoilProfileEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MacroStabilityInwardsStochasticSoilProfileEntity()
        {
            MacroStabilityInwardsCalculationEntities = new HashSet<MacroStabilityInwardsCalculationEntity>();
        }

        public long MacroStabilityInwardsStochasticSoilProfileEntityId { get; set; }
        public long StochasticSoilModelEntityId { get; set; }
        public long? MacroStabilityInwardsSoilProfileOneDEntityId { get; set; }
        public long? MacroStabilityInwardsSoilProfileTwoDEntityId { get; set; }
        public double Probability { get; set; }
        public int Order { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsCalculationEntity> MacroStabilityInwardsCalculationEntities { get; set; }

        public virtual MacroStabilityInwardsSoilProfileOneDEntity MacroStabilityInwardsSoilProfileOneDEntity { get; set; }
        public virtual MacroStabilityInwardsSoilProfileTwoDEntity MacroStabilityInwardsSoilProfileTwoDEntity { get; set; }
        public virtual StochasticSoilModelEntity StochasticSoilModelEntity { get; set; }
    }
}