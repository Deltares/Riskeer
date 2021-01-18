// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Storage.Core.DbContext
{
    public class ProbabilisticPipingCalculationOutputEntity
    {
        public long ProbabilisticPipingCalculationOutputEntityId { get; set; }
        public long ProbabilisticPipingCalculationEntityId { get; set; }
        public long? ProfileSpecificGeneralResultFaultTreeIllustrationPointEntityId { get; set; }
        public long? SectionSpecificGeneralResultFaultTreeIllustrationPointEntityId { get; set; }
        public long? ProfileSpecificGeneralResultSubMechanismIllustrationPointEntityId { get; set; }
        public long? SectionSpecificGeneralResultSubMechanismIllustrationPointEntityId { get; set; }
        public int Order { get; set; }
        public double? ProfileSpecificReliability { get; set; }
        public double? SectionSpecificReliability { get; set; }

        public virtual GeneralResultFaultTreeIllustrationPointEntity GeneralResultFaultTreeIllustrationPointEntity { get; set; }
        public virtual GeneralResultFaultTreeIllustrationPointEntity GeneralResultFaultTreeIllustrationPointEntity1 { get; set; }
        public virtual GeneralResultSubMechanismIllustrationPointEntity GeneralResultSubMechanismIllustrationPointEntity { get; set; }
        public virtual GeneralResultSubMechanismIllustrationPointEntity GeneralResultSubMechanismIllustrationPointEntity1 { get; set; }
        public virtual ProbabilisticPipingCalculationEntity ProbabilisticPipingCalculationEntity { get; set; }
    }
}