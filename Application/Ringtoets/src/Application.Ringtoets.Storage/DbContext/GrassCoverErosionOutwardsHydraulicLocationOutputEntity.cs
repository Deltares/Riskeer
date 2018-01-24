// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application.Ringtoets.Storage.DbContext
{
    public class GrassCoverErosionOutwardsHydraulicLocationOutputEntity
    {
        public long GrassCoverErosionOutwardsHydraulicLocationOutputEntityId { get; set; }
        public long GrassCoverErosionOutwardsHydraulicLocationEntityId { get; set; }
        public long? GeneralResultSubMechanismIllustrationPointEntityId { get; set; }
        public virtual GrassCoverErosionOutwardsHydraulicLocationEntity GrassCoverErosionOutwardsHydraulicLocationEntity { get; set; }
        public byte HydraulicLocationOutputType { get; set; }
        public double? Result { get; set; }
        public double? TargetProbability { get; set; }
        public double? TargetReliability { get; set; }
        public double? CalculatedProbability { get; set; }
        public double? CalculatedReliability { get; set; }
        public byte CalculationConvergence { get; set; }

        public virtual GeneralResultSubMechanismIllustrationPointEntity GeneralResultSubMechanismIllustrationPointEntity { get; set; }
    }
}