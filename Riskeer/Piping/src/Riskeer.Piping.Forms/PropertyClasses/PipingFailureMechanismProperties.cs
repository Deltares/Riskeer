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

using System;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Piping.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanism"/> properties panel.
    /// </summary>
    public class PipingFailureMechanismProperties : ObjectProperties<PipingFailureMechanism>
    {
        private readonly Dictionary<string, int> propertyIndexLookup;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="constructionProperties">The property values required to create an instance of <see cref="PipingFailureMechanismProperties"/>.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismProperties(PipingFailureMechanism data,
                                                ConstructionProperties constructionProperties,
                                                IAssessmentSection assessmentSection)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Data = data;
            AssessmentSection = assessmentSection;

            propertyIndexLookup = new Dictionary<string, int>
            {
                {
                    nameof(Name), constructionProperties.NamePropertyIndex
                },
                {
                    nameof(Code), constructionProperties.CodePropertyIndex
                },
                {
                    nameof(Group), constructionProperties.GroupPropertyIndex
                },
                {
                    nameof(Contribution), constructionProperties.ContributionPropertyIndex
                },
                {
                    nameof(A), constructionProperties.APropertyIndex
                },
                {
                    nameof(B), constructionProperties.BPropertyIndex
                },
                {
                    nameof(SectionLength), constructionProperties.SectionLengthPropertyIndex
                },
                {
                    nameof(N), constructionProperties.NPropertyIndex
                }
            };
        }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrderEvaluationMethod(string propertyName)
        {
            propertyIndexLookup.TryGetValue(propertyName, out int propertyIndex);

            return propertyIndex;
        }

        protected IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="PipingFailureMechanismProperties"/>.
        /// </summary>
        public class ConstructionProperties
        {
            #region General

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.Name"/>.
            /// </summary>
            public int NamePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.Code"/>.
            /// </summary>
            public int CodePropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.Group"/>.
            /// </summary>
            public int GroupPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.Contribution"/>.
            /// </summary>
            public int ContributionPropertyIndex { get; set; }

            #endregion

            # region Length effect parameters

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.A"/>.
            /// </summary>
            public int APropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.B"/>.
            /// </summary>
            public int BPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.SectionLength"/>.
            /// </summary>
            public int SectionLengthPropertyIndex { get; set; }

            /// <summary>
            /// Gets or sets the property index for <see cref="PipingFailureMechanismProperties.N"/>.
            /// </summary>
            public int NPropertyIndex { get; set; }

            # endregion
        }

        #region General

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_Description))]
        public int Group
        {
            get
            {
                return data.Group;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_Description))]
        public virtual double Contribution
        {
            get
            {
                return data.Contribution;
            }
        }

        #endregion

        #region Length effect parameters

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_Description))]
        public virtual double A
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.A;
            }
            set
            {
                data.PipingProbabilityAssessmentInput.A = value;
                data.NotifyObservers();
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_Description))]
        public virtual double B
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.B;
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_Description))]
        public virtual RoundedDouble SectionLength
        {
            get
            {
                return new RoundedDouble(2, AssessmentSection.ReferenceLine.Length);
            }
        }

        [DynamicPropertyOrder]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_Description))]
        public virtual RoundedDouble N
        {
            get
            {
                PipingProbabilityAssessmentInput probabilityAssessmentInput = data.PipingProbabilityAssessmentInput;
                return new RoundedDouble(2, probabilityAssessmentInput.GetN(AssessmentSection.ReferenceLine.Length));
            }
        }

        #endregion
    }
}