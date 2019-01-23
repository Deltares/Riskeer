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

using System;
using System.Collections;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.ClosingStructures;
using Ringtoets.Storage.Core.Read.GrassCoverErosionInwards;
using Ringtoets.Storage.Core.Read.GrassCoverErosionOutwards;
using Ringtoets.Storage.Core.Read.HeightStructures;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;
using Ringtoets.Storage.Core.Read.Piping;
using Ringtoets.Storage.Core.Read.StabilityPointStructures;
using Ringtoets.Storage.Core.Read.StabilityStoneCover;
using Ringtoets.Storage.Core.Read.WaveImpactAsphaltCover;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="CalculationGroup"/> based on the
    /// <see cref="CalculationGroupEntity"/>.
    /// </summary>
    internal static class CalculationGroupEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <param name="generalPipingInput">The calculation input parameters for piping.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/>
        /// or <paramref name="generalPipingInput"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsPipingCalculationGroup(this CalculationGroupEntity entity, ReadConversionCollector collector,
                                                                      GeneralPipingInput generalPipingInput)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput));
                }

                var childCalculationEntity = childEntity as PipingCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector, generalPipingInput));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsMacroStabilityInwardsCalculationGroup(this CalculationGroupEntity entity,
                                                                                     ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsMacroStabilityInwardsCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as MacroStabilityInwardsCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsGrassCoverErosionInwardsCalculationGroup(this CalculationGroupEntity entity,
                                                                                        ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as GrassCoverErosionInwardsCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(this CalculationGroupEntity entity,
                                                                                                       ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as GrassCoverErosionOutwardsWaveConditionsCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsHeightStructuresCalculationGroup(this CalculationGroupEntity entity,
                                                                                ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsHeightStructuresCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as HeightStructuresCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsClosingStructuresCalculationGroup(this CalculationGroupEntity entity,
                                                                                 ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsClosingStructuresCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as ClosingStructuresCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsStabilityPointStructuresCalculationGroup(this CalculationGroupEntity entity,
                                                                                        ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsStabilityPointStructuresCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as StabilityPointStructuresCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(this CalculationGroupEntity entity,
                                                                                                 ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as StabilityStoneCoverWaveConditionsCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(this CalculationGroupEntity entity,
                                                                                                    ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var group = new CalculationGroup
            {
                Name = entity.Name
            };

            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector));
                }

                var childCalculationEntity = childEntity as WaveImpactAsphaltCoverWaveConditionsCalculationEntity;
                if (childCalculationEntity != null)
                {
                    group.Children.Add(childCalculationEntity.Read(collector));
                }
            }

            return group;
        }

        private static IEnumerable GetChildEntitiesInOrder(CalculationGroupEntity entity)
        {
            var sortedList = new SortedList();
            foreach (CalculationGroupEntity groupEntity in entity.CalculationGroupEntity1)
            {
                sortedList.Add(groupEntity.Order, groupEntity);
            }

            foreach (PipingCalculationEntity calculationEntity in entity.PipingCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (GrassCoverErosionInwardsCalculationEntity calculationEntity in entity.GrassCoverErosionInwardsCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (GrassCoverErosionOutwardsWaveConditionsCalculationEntity calculationEntity in entity.GrassCoverErosionOutwardsWaveConditionsCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (HeightStructuresCalculationEntity calculationEntity in entity.HeightStructuresCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (ClosingStructuresCalculationEntity calculationEntity in entity.ClosingStructuresCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (StabilityPointStructuresCalculationEntity calculationEntity in entity.StabilityPointStructuresCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (StabilityStoneCoverWaveConditionsCalculationEntity calculationEntity in entity.StabilityStoneCoverWaveConditionsCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (WaveImpactAsphaltCoverWaveConditionsCalculationEntity calculationEntity in entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            foreach (MacroStabilityInwardsCalculationEntity calculationEntity in entity.MacroStabilityInwardsCalculationEntities)
            {
                sortedList.Add(calculationEntity.Order, calculationEntity);
            }

            return sortedList.Values;
        }
    }
}