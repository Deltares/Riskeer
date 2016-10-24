// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.Collections.Generic;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of structure calculation input for properties panel.
    /// </summary>
    /// <typeparam name="TStructure">The type of structures at stake.</typeparam>
    /// <typeparam name="TStructureInput">The type of structures calculation input.</typeparam>
    /// <typeparam name ="TCalculation">The type of the calculation containing the structures calculation input.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
    public abstract class StructuresInputBaseProperties<TStructure, TStructureInput, TCalculation, TFailureMechanism> :
        ObjectProperties<InputContextBase<TStructureInput, TCalculation, TFailureMechanism>>,
        IHasHydraulicBoundaryLocationProperty,
        IHasStructureProperty<TStructure>,
        IHasForeshoreProfileProperty
        where TStructure : StructureBase
        where TStructureInput : StructuresInputBase<TStructure>
        where TCalculation : ICalculation
        where TFailureMechanism : IFailureMechanism
    {
        private readonly ConstructionProperties constructionProperties;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/> class.
        /// </summary>
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        protected StructuresInputBaseProperties(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException("constructionProperties");
            }

            this.constructionProperties = constructionProperties;
        }

        public ForeshoreProfile ForeshoreProfile { get; private set; }

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; private set; }

        public TStructure Structure { get; private set; }

        public IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TStructure> GetAvailableStructures()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.ForeshoreProfile"/>.
            /// </summary>
            public int ForeshoreProfilePropertyIndex { get; set; }
        }
    }
}