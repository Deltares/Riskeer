﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RingtoetsEntities : DbContext, IRingtoetsEntities
    {
        public RingtoetsEntities()
            : base("name=RingtoetsEntities")
        {
        }
    
    	/// <summary>
        /// This method is called in a 'code first' approach when the model for a derived <see cref="DbContext"/> has been initialized,
        /// but before the model has been locked down and used to initialize the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder"/> that defines the model for the context being created.</param>
        /// <exception cref="UnintentionalCodeFirstException">Thrown because the <see cref="DbContext"/> is created in a 'code first' approach.</exception>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AssessmentSectionEntity> AssessmentSectionEntities { get; set; }
        public virtual DbSet<CalculationGroupEntity> CalculationGroupEntities { get; set; }
        public virtual DbSet<CharacteristicPointEntity> CharacteristicPointEntities { get; set; }
        public virtual DbSet<FailureMechanismEntity> FailureMechanismEntities { get; set; }
        public virtual DbSet<FailureMechanismSectionEntity> FailureMechanismSectionEntities { get; set; }
        public virtual DbSet<FailureMechanismSectionPointEntity> FailureMechanismSectionPointEntities { get; set; }
        public virtual DbSet<HydraulicLocationEntity> HydraulicLocationEntities { get; set; }
        public virtual DbSet<ProjectEntity> ProjectEntities { get; set; }
        public virtual DbSet<ReferenceLinePointEntity> ReferenceLinePointEntities { get; set; }
        public virtual DbSet<SoilLayerEntity> SoilLayerEntities { get; set; }
        public virtual DbSet<SoilProfileEntity> SoilProfileEntities { get; set; }
        public virtual DbSet<StochasticSoilModelEntity> StochasticSoilModelEntities { get; set; }
        public virtual DbSet<StochasticSoilProfileEntity> StochasticSoilProfileEntities { get; set; }
        public virtual DbSet<SurfaceLineEntity> SurfaceLineEntities { get; set; }
        public virtual DbSet<SurfaceLinePointEntity> SurfaceLinePointEntities { get; set; }
        public virtual DbSet<VersionEntity> VersionEntities { get; set; }
    }
}
