// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IFailureMechanism"/> for the properties panel
    /// to show a collection of <see cref="FailureMechanismSectionConfiguration"/>.
    /// </summary>
    public class FailureMechanismSectionConfigurationsProperties : ObjectProperties<IObservableEnumerable<FailureMechanismSectionConfiguration>>, IDisposable
    {
        private readonly Observer failureMechanismObserver;
        private readonly RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration> sectionConfigurationsObserver;

        private readonly IFailureMechanism failureMechanism;
        private readonly double b;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsProperties"/>.
        /// </summary>
        /// <param name="sectionConfigurations">The collection of failure mechanism section configurations
        /// to show the properties for.</param>
        /// <param name="failureMechanism">The failure mechanism the section configurations belong to.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfigurations"/> or
        /// <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismSectionConfigurationsProperties(IObservableEnumerable<FailureMechanismSectionConfiguration> sectionConfigurations, IFailureMechanism failureMechanism, double b)
        {
            if (sectionConfigurations == null)
            {
                throw new ArgumentNullException(nameof(sectionConfigurations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            failureMechanismObserver = new Observer(OnRefreshRequired)
            {
                Observable = failureMechanism
            };

            sectionConfigurationsObserver = new RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration>(
                OnRefreshRequired,
                sectionConfiguration => sectionConfiguration)
            {
                Observable = sectionConfigurations
            };

            data = sectionConfigurations;
            this.failureMechanism = failureMechanism;
            this.b = b;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ObservableCollectionWithSourcePath_SourcePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSections_SourcePath_Description))]
        public string SourcePath
        {
            get
            {
                return failureMechanism.FailureMechanismSectionSourcePath;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSections_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSections_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionConfigurationProperties[] SectionConfigurations
        {
            get
            {
                return FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations(
                    data,
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

        private FailureMechanismSectionConfigurationProperties CreateFailureMechanismSectionConfigurationProperties(
            FailureMechanismSectionConfiguration sectionConfiguration,
            double sectionStart,
            double sectionEnd)
        {
            return new FailureMechanismSectionConfigurationProperties(sectionConfiguration, sectionStart, sectionEnd, b);
        }
    }
}