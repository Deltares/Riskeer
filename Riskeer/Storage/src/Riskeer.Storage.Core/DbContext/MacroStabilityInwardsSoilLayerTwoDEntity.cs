// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Storage.Core.DbContext
{
    public class MacroStabilityInwardsSoilLayerTwoDEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MacroStabilityInwardsSoilLayerTwoDEntity()
        {
            MacroStabilityInwardsSoilLayerTwoDEntity1 = new HashSet<MacroStabilityInwardsSoilLayerTwoDEntity>();
            MacroStabilityInwardsSoilProfileTwoDEntities = new HashSet<MacroStabilityInwardsSoilProfileTwoDEntity>();
        }
    
        public long MacroStabilityInwardsSoilLayerTwoDEntityId { get; set; }
        public long? ParentMacroStabilityInwardsSoilLayerTwoDEntityId { get; set; }
        public byte IsAquifer { get; set; }
        public long? Color { get; set; }
        public string MaterialName { get; set; }
        public byte ShearStrengthModel { get; set; }
        public byte UsePop { get; set; }
        public double? AbovePhreaticLevelMean { get; set; }
        public double? AbovePhreaticLevelCoefficientOfVariation { get; set; }
        public double? AbovePhreaticLevelShift { get; set; }
        public double? BelowPhreaticLevelMean { get; set; }
        public double? BelowPhreaticLevelCoefficientOfVariation { get; set; }
        public double? BelowPhreaticLevelShift { get; set; }
        public double? CohesionMean { get; set; }
        public double? CohesionCoefficientOfVariation { get; set; }
        public double? FrictionAngleMean { get; set; }
        public double? FrictionAngleCoefficientOfVariation { get; set; }
        public double? ShearStrengthRatioMean { get; set; }
        public double? ShearStrengthRatioCoefficientOfVariation { get; set; }
        public double? StrengthIncreaseExponentMean { get; set; }
        public double? StrengthIncreaseExponentCoefficientOfVariation { get; set; }
        public double? PopMean { get; set; }
        public double? PopCoefficientOfVariation { get; set; }
        public string OuterRingXml { get; set; }
        public int Order { get; set; }
    
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsSoilLayerTwoDEntity> MacroStabilityInwardsSoilLayerTwoDEntity1 { get; set; }
        public virtual MacroStabilityInwardsSoilLayerTwoDEntity MacroStabilityInwardsSoilLayerTwoDEntity2 { get; set; }
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsSoilProfileTwoDEntity> MacroStabilityInwardsSoilProfileTwoDEntities { get; set; }
    }
}
