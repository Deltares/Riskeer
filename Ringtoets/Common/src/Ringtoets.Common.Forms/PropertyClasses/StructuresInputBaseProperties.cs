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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
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

        #region Model factors

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "ModelFactorSuperCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ModelFactorSuperCriticalFlow_Description")]
        public NormalDistributionProperties ModelFactorSuperCriticalFlow
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

        public abstract IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles();

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        public abstract IEnumerable<TStructure> GetAvailableStructures();

        /// <summary>
        /// The action to perform after setting the <see cref="Structure"/> property.
        /// </summary>
        protected abstract void AfterSettingStructure();

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
        public Point2D HeightStructureLocation
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
        public RoundedDouble StructureNormalOrientation
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
        public VariationCoefficientNormalDistributionProperties WidthFlowApertures
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
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityStructureWithErosion_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityStructureWithErosion_Description")]
        public string FailureProbabilityStructureWithErosion
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityStructureWithErosion);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.FailureProbabilityStructureWithErosion_Value_cannot_be_null);
                }
                try
                {
                    data.WrappedData.FailureProbabilityStructureWithErosion = (RoundedDouble)double.Parse(value);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityStructureWithErosion_Value_too_large);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityStructureWithErosion_Could_not_parse_string_to_double_value);
                }
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProfile_Description")]
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
                           new UseBreakWaterProperties(null) :
                           new UseBreakWaterProperties(data.WrappedData);
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
        public HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "StormDuration_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StormDuration_Description")]
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