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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IFailureMechanism"/> for the properties panel
    /// to show a collection of <see cref="FailureMechanismSection"/>.
    /// </summary>
    public class FailureMechanismSectionsProperties : ObjectProperties<IFailureMechanism>, IDisposable
    {
        private readonly Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the section properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismSectionsProperties(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            failureMechanismObserver = new Observer(OnRefreshRequired)
            {
                Observable = failureMechanism
            };

            data = failureMechanism;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ObservableCollectionWithSourcePath_SourcePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSections_SourcePath_Description))]
        public string SourcePath
        {
            get
            {
                return data.FailureMechanismSectionSourcePath;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSections_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSections_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionProperties[] Sections
        {
            get
            {
                return FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(
                    data.Sections,
                    CreateFailureMechanismSectionProperties);
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
            }
        }

        private static FailureMechanismSectionProperties CreateFailureMechanismSectionProperties(FailureMechanismSection section,
                                                                                                 double sectionStart,
                                                                                                 double sectionEnd)
        {
            return new FailureMechanismSectionProperties(section, sectionStart, sectionEnd);
        }
    }
}