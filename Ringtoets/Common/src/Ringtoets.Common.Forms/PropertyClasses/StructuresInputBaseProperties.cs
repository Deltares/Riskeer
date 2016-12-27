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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq.Expressions;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
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
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        protected StructuresInputBaseProperties(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException("constructionProperties");
            }

            propertyIndexLookup = new Dictionary<string, int>
            {
                {
                    GetMemberName(p => p.ModelFactorSuperCriticalFlow), constructionProperties.ModelFactorSuperCriticalFlowPropertyIndex
                },
                {
                    GetMemberName(p => p.Structure), constructionProperties.StructurePropertyIndex
                },
                {
                    GetMemberName(p => p.StructureLocation), constructionProperties.StructureLocationPropertyIndex
                },
                {
                    GetMemberName(p => p.StructureNormalOrientation), constructionProperties.StructureNormalOrientationPropertyIndex
                },
                {
                    GetMemberName(p => p.FlowWidthAtBottomProtection), constructionProperties.FlowWidthAtBottomProtectionPropertyIndex
                },
                {
                    GetMemberName(p => p.WidthFlowApertures), constructionProperties.WidthFlowAperturesPropertyIndex
                },
                {
                    GetMemberName(p => p.StorageStructureArea), constructionProperties.StorageStructureAreaPropertyIndex
                },
                {
                    GetMemberName(p => p.AllowedLevelIncreaseStorage), constructionProperties.AllowedLevelIncreaseStoragePropertyIndex
                },
                {
                    GetMemberName(p => p.CriticalOvertoppingDischarge), constructionProperties.CriticalOvertoppingDischargePropertyIndex
                },
                {
                    GetMemberName(p => p.FailureProbabilityStructureWithErosion), constructionProperties.FailureProbabilityStructureWithErosionPropertyIndex
                },
                {
                    GetMemberName(p => p.ForeshoreProfile), constructionProperties.ForeshoreProfilePropertyIndex
                },
                {
                    GetMemberName(p => p.UseBreakWater), constructionProperties.UseBreakWaterPropertyIndex
                },
                {
                    GetMemberName(p => p.UseForeshore), constructionProperties.UseForeshorePropertyIndex
                },
                {
                    GetMemberName(p => p.SelectedHydraulicBoundaryLocation), constructionProperties.HydraulicBoundaryLocationPropertyIndex
                },
                {
                    GetMemberName(p => p.StormDuration), constructionProperties.StormDurationPropertyIndex
                }
            };
        }

        #region Model factors

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ModelFactorSuperCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ModelFactorSuperCriticalFlow_Description")]
        public virtual NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.StandardDeviation, data.WrappedData)
                {
                    Data = data.WrappedData.ModelFactorSuperCriticalFlow
                };
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

        public abstract IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles();

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AvailableHydraulicBoundaryLocations, StructureLocation);
        }

        public abstract IEnumerable<TStructure> GetAvailableStructures();

        /// <summary>
        /// The action to perform after setting the <see cref="Structure"/> property.
        /// </summary>
        protected abstract void AfterSettingStructure();

        /// <summary>
        /// Sets a probability value to one of the properties of a wrapped data object.
        /// </summary>
        /// <typeparam name="T">The type of the wrapped data to set a probability value for.</typeparam>
        /// <param name="value">The probability value to set.</param>
        /// <param name="wrappedData">The wrapped data to set a probability value for.</param>
        /// <param name="setValueAction">The action that sets the probability value to a specific property of the wrapped data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> equals <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> cannot be parsed into a <c>double</c>.</exception>
        /// <remarks>After correctly setting the <paramref name="value"/> to the wrapped data, observers will be notified.</remarks>
        protected static void SetProbabilityValue<T>(string value,
                                                     T wrappedData,
                                                     Action<T, RoundedDouble> setValueAction)
            where T : IObservable
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", Resources.FailureProbability_Value_cannot_be_null);
            }
            try
            {
                setValueAction(wrappedData, (RoundedDouble) double.Parse(value));
            }
            catch (OverflowException)
            {
                throw new ArgumentException(Resources.FailureProbability_Value_too_large);
            }
            catch (FormatException)
            {
                throw new ArgumentException(Resources.FailureProbability_Could_not_parse_string_to_double_value);
            }
            wrappedData.NotifyObservers();
        }

        private static string GetMemberName(Expression<Func<StructuresInputBaseProperties<TStructure, TStructureInput, TCalculation, TFailureMechanism>, object>> expression)
        {
            return TypeUtils.GetMemberName(expression);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StructuresInputBaseProperties{TStructure, TStructureInput, TCalculation, TFailureMechanism}"/>.
        /// </summary>
        public class ConstructionProperties
        {
            #region Model factors

            /// <summary>
            /// Gets or sets the property index for <see cref="StructuresInputBase{TStructure}.ModelFactorSuperCriticalFlow"/>.
            /// </summary>
            public int ModelFactorSuperCriticalFlowPropertyIndex { get; set; }

            #endregion

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
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_Description")]
        public TStructure Structure
        {
            get
            {
                return data.WrappedData.Structure;
            }
            set
            {
                data.WrappedData.Structure = value;
                AfterSettingStructure();
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_Location_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_Location_Description")]
        public Point2D StructureLocation
        {
            get
            {
                return data.WrappedData.Structure == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.Structure.Location.X),
                               new RoundedDouble(0, data.WrappedData.Structure.Location.Y));
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_StructureNormalOrientation_Description")]
        public virtual RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.WrappedData.StructureNormalOrientation;
            }
            set
            {
                data.WrappedData.StructureNormalOrientation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FlowWidthAtBottomProtection_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FlowWidthAtBottomProtection_Description")]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.FlowWidthAtBottomProtection
                };
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_WidthFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_WidthFlowApertures_Description")]
        public virtual VariationCoefficientNormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.WidthFlowApertures
                };
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_StorageStructureArea_Description")]
        public VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.StorageStructureArea
                };
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_AllowedLevelIncreaseStorage_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_AllowedLevelIncreaseStorage_Description")]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.AllowedLevelIncreaseStorage
                };
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_CriticalOvertoppingDischarge_Description")]
        public VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.CriticalOvertoppingDischarge
                };
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FailureProbabilityStructureWithErosion_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FailureProbabilityStructureWithErosion_Description")]
        public string FailureProbabilityStructureWithErosion
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityStructureWithErosion);
            }
            set
            {
                SetProbabilityValue(value, data.WrappedData, (wrappedData, parsedValue) => wrappedData.FailureProbabilityStructureWithErosion = parsedValue);
            }
        }

        [DynamicPropertyOrder]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ForeshoreProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ForeshoreProfile_Description")]
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return data.WrappedData.ForeshoreProfile;
            }
            set
            {
                data.WrappedData.ForeshoreProfile = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterProperties_Description")]
        public UseBreakWaterProperties UseBreakWater
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null ?
                           new UseBreakWaterProperties() :
                           new UseBreakWaterProperties(data.WrappedData, data.Calculation);
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProperties_Description")]
        public UseForeshoreProperties UseForeshore
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData);
            }
        }

        #endregion

        #region Hydraulic data

        [DynamicPropertyOrder]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryLocation_Description")]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation, StructureLocation)
                           : null;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "Structure_StormDuration_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_StormDuration_Description")]
        public VariationCoefficientLogNormalDistributionProperties StormDuration
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, data.WrappedData)
                {
                    Data = data.WrappedData.StormDuration
                };
            }
        }

        #endregion
    }
}