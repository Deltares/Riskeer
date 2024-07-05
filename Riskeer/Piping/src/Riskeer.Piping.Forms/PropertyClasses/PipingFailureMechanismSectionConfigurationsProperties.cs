// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanism"/> for the properties panel
    /// to show a collection of <see cref="PipingFailureMechanismSectionConfiguration"/>.
    /// </summary>
    public class PipingFailureMechanismSectionConfigurationsProperties : ObjectProperties<PipingFailureMechanism>, IDisposable
    {
        private readonly Func<PipingFailureMechanismSectionConfiguration, IObservablePropertyChangeHandler> getPropertyChangeHandlerFunc;
        private readonly Observer failureMechanismObserver;
        private readonly RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration> sectionConfigurationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionConfigurationsProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the section configurations for.</param>
        /// <param name="getPropertyChangeHandlerFunc"></param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismSectionConfigurationsProperties(PipingFailureMechanism failureMechanism,
                                                                     Func<PipingFailureMechanismSectionConfiguration, IObservablePropertyChangeHandler> getPropertyChangeHandlerFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (getPropertyChangeHandlerFunc == null)
            {
                throw new ArgumentNullException(nameof(getPropertyChangeHandlerFunc));
            }

            this.getPropertyChangeHandlerFunc = getPropertyChangeHandlerFunc;

            data = failureMechanism;

            failureMechanismObserver = new Observer(OnRefreshRequired)
            {
                Observable = data
            };

            sectionConfigurationsObserver = new RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration>(
                OnRefreshRequired,
                sectionConfiguration => sectionConfiguration)
            {
                Observable = data.SectionConfigurations
            };
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ObservableCollectionWithSourcePath_SourcePath_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSections_SourcePath_Description))]
        public string SourcePath
        {
            get
            {
                return data.FailureMechanismSectionSourcePath;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSections_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSections_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public PipingFailureMechanismSectionConfigurationProperties[] SectionConfigurations
        {
            get
            {
                return FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations(
                    data.SectionConfigurations,
                    CreateFailureMechanismSectionConfigurationProperties);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                failureMechanismObserver.Dispose();
                sectionConfigurationsObserver.Dispose();
            }
        }

        private PipingFailureMechanismSectionConfigurationProperties CreateFailureMechanismSectionConfigurationProperties(
            PipingFailureMechanismSectionConfiguration sectionConfiguration,
            double sectionStart,
            double sectionEnd)
        {
            return new PipingFailureMechanismSectionConfigurationProperties(sectionConfiguration, sectionStart, sectionEnd,
                                                                            data.GeneralInput.B, getPropertyChangeHandlerFunc(sectionConfiguration));
        }
    }
}