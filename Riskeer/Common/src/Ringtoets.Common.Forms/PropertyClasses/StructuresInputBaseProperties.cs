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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.UITypeEditors;

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
        private readonly Dictionary<string, int> propertyIndexLookup;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/> class.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected StructuresInputBaseProperties(
            InputContextBase<TStructureInput, TCalculation, TFailureMechanism> data,
            ConstructionProperties constructionProperties,
            IObservablePropertyChangeHandler propertyChangeHandler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            if (propertyChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(propertyChangeHandler));
            }

            PropertyChangeHandler = propertyChangeHandler;
            Data = data;
            propertyIndexLookup = new Dictionary<string, int>
            {
                {
                    nameof(Structure), constructionProperties.StructurePropertyIndex
                },
                {
                    nameof(StructureLocation), constructionProperties.StructureLocationPropertyIndex
                },
                {
                    nameof(StructureNormalOrientation), constructionProperties.StructureNormalOrientationPropertyIndex
                },
                {
                    nameof(FlowWidthAtBottomProtection), constructionProperties.FlowWidthAtBottomProtectionPropertyIndex
                },
                {
                    nameof(WidthFlowApertures), constructionProperties.WidthFlowAperturesPropertyIndex
                },
                {
                    nameof(StorageStructureArea), constructionProperties.StorageStructureAreaPropertyIndex
                },
                {
                    nameof(AllowedLevelIncreaseStorage), constructionProperties.AllowedLevelIncreaseStoragePropertyIndex
                },
                {
                    nameof(CriticalOvertoppingDischarge), constructionProperties.CriticalOvertoppingDischargePropertyIndex
                },
                {
                    nameof(FailureProbabilityStructureWithErosion), constructionProperties.FailureProbabilityStructureWithErosionPropertyIndex
                },
                {
                    nameof(ForeshoreProfile), constructionProperties.ForeshoreProfilePropertyIndex
                },
                {
                    nameof(UseBreakWater), constructionProperties.UseBreakWaterPropertyIndex
                },
                {
                    nameof(UseForeshore), constructionProperties.UseForeshorePropertyIndex
                },
                {
                    nameof(SelectedHydraulicBoundaryLocation), constructionProperties.HydraulicBoundaryLocationPropertyIndex
                },
                {
                    nameof(StormDuration), constructionProperties.StormDurationPropertyIndex
                }
            };
        }

        #region Output Settings

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OutputSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldIllustrationPointsBeCalculated
        {
            get
            {
                return data.WrappedData.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                data.WrappedData.ShouldIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        #endregion

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrderEvaluationMethod(string propertyName)
        {
            int propertyIndex;

            propertyIndexLookup.TryGetValue(propertyName, out propertyIndex);

            return propertyIndex;
        }

        [DynamicReadOnlyValidationMethod]
        public bool IsReadOnly(string property)
        {
            if (!HasStructure() && ShouldPropertyBeReadOnlyInAbsenseOfStructure(property))
            {
                return true;
            }

            return false;
        }

        public abstract IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles();

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referenceLocation = data.WrappedData.Structure?.Location;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AssessmentSection.HydraulicBoundaryDatabase.Locations, referenceLocation);
        }

        public abstract IEnumerable<TStructure> GetAvailableStructures();

        /// <summary>
        /// The change handler responsible for handling effects of a property change.
        /// </summary>
        protected IObservablePropertyChangeHandler PropertyChangeHandler { get; }

        /// <summary>
        /// The action to perform after setting the <see cref="Structure"/> property.
        /// </summary>
        protected abstract void AfterSettingStructure();

        protected bool HasStructure()
        {
            return data.WrappedData.Structure != null;
        }

        protected virtual bool ShouldPropertyBeReadOnlyInAbsenseOfStructure(string property)
        {
            if (nameof(StructureNormalOrientation).Equals(property))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.
        /// </summary>
        public class ConstructionProperties
        {
            #region Schematization

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.Structure"/>.
            /// </summary>
            public int StructurePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for the location of <see cref="StructuresInputBase{TStructure}.Structure"/>.
            /// </summary>
            public int StructureLocationPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.StructureNormalOrientation"/>.
            /// </summary>
            public int StructureNormalOrientationPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.FlowWidthAtBottomProtection"/>.
            /// </summary>
            public int FlowWidthAtBottomProtectionPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.WidthFlowApertures"/>.
            /// </summary>
            public int WidthFlowAperturesPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.StorageStructureArea"/>.
            /// </summary>
            public int StorageStructureAreaPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.AllowedLevelIncreaseStorage"/>.
            /// </summary>
            public int AllowedLevelIncreaseStoragePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.CriticalOvertoppingDischarge"/>.
            /// </summary>
            public int CriticalOvertoppingDischargePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.FailureProbabilityStructureWithErosion"/>.
            /// </summary>
            public int FailureProbabilityStructureWithErosionPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.ForeshoreProfile"/>.
            /// </summary>
            public int ForeshoreProfilePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.UseBreakWater"/>.
            /// </summary>
            public int UseBreakWaterPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.UseForeshore"/>.
            /// </summary>
            public int UseForeshorePropertyIndex { get; set; }

            #endregion

            #region Hydraulic data

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.HydraulicBoundaryLocation"/>.
            /// </summary>
            public int HydraulicBoundaryLocationPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.StormDuration"/>.
            /// </summary>
            public int StormDurationPropertyIndex { get; set; }

            #endregion
        }

        #region Schematization

        [DynamicPropertyOrder]
        [Editor(typeof(StructureEditor<StructureBase>), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_Description))]
        public TStructure Structure
        {
            get
            {
                return data.WrappedData.Structure;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() =>
                {
                    data.WrappedData.Structure = value;
                    AfterSettingStructure();
                }, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_Location_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_Location_Description))]
        public Point2D StructureLocation
        {
            get
            {
                return data.WrappedData.Structure == null
                           ? null
                           : new Point2D(
                               new RoundedDouble(0, data.WrappedData.Structure.Location.X),
                               new RoundedDouble(0, data.WrappedData.Structure.Location.Y));
            }
        }

        [DynamicReadOnly]
        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StructureNormalOrientation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StructureNormalOrientation_Description))]
        public virtual RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.WrappedData.StructureNormalOrientation;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.StructureNormalOrientation = value, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FlowWidthAtBottomProtection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FlowWidthAtBottomProtection_Description))]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.FlowWidthAtBottomProtection,
                    PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_WidthFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_WidthFlowApertures_Description))]
        public virtual NormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.WidthFlowApertures,
                    PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StorageStructureArea_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StorageStructureArea_Description))]
        public VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.StorageStructureArea,
                    PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_AllowedLevelIncreaseStorage_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_AllowedLevelIncreaseStorage_Description))]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.AllowedLevelIncreaseStorage,
                    PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_CriticalOvertoppingDischarge_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_CriticalOvertoppingDischarge_Description))]
        public VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    HasStructure()
                        ? VariationCoefficientDistributionPropertiesReadOnly.None
                        : VariationCoefficientDistributionPropertiesReadOnly.All,
                    data.WrappedData.CriticalOvertoppingDischarge,
                    PropertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [DynamicPropertyOrder]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureProbabilityStructureWithErosion_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureProbabilityStructureWithErosion_Description))]
        public double FailureProbabilityStructureWithErosion
        {
            get
            {
                return data.WrappedData.FailureProbabilityStructureWithErosion;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.FailureProbabilityStructureWithErosion = value, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ForeshoreProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ForeshoreProfile_Description))]
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return data.WrappedData.ForeshoreProfile;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ForeshoreProfile = value, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BreakWaterProperties_Description))]
        public UseBreakWaterProperties UseBreakWater
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null
                           ? new UseBreakWaterProperties()
                           : new UseBreakWaterProperties(data.WrappedData, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProperties_Description))]
        public UseForeshoreProperties UseForeshore
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData, PropertyChangeHandler);
            }
        }

        #endregion

        #region Hydraulic data

        [DynamicPropertyOrder]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryLocation_Description))]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                Point2D referenceLocation = data.WrappedData.Structure?.Location;
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation, referenceLocation)
                           : null;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation, PropertyChangeHandler);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StormDuration_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StormDuration_Description))]
        public VariationCoefficientLogNormalDistributionProperties StormDuration
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(
                    VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation,
                    data.WrappedData.StormDuration,
                    PropertyChangeHandler);
            }
        }

        #endregion
    }
}