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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="FailureMechanismSection"/>.
    /// </summary>
    public partial class FailureMechanismSectionsView : CloseForFailureMechanismView
    {
        protected readonly IEnumerable<FailureMechanismSection> Sections;

        private readonly Observer failureMechanismObserver;

        private IEnumerable<FailureMechanismSection> currentSections;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsView"/>.
        /// </summary>
        /// <param name="sections">The sections to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public FailureMechanismSectionsView(IEnumerable<FailureMechanismSection> sections, IFailureMechanism failureMechanism)
            : base(failureMechanism)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            InitializeComponent();

            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionRow.Name),
                                                                         Resources.FailureMechanismSection_Name_DisplayName,
                                                                         true);
            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionRow.SectionStart),
                                                                         Resources.SectionStart_DisplayName,
                                                                         true);
            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionRow.SectionEnd),
                                                                         Resources.SectionEnd_DisplayName,
                                                                         true);
            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionRow.Length),
                                                                         Resources.FailureMechanismSection_Length_Rounded_DisplayName,
                                                                         true);

            failureMechanismObserver = new Observer(HandleFailureMechanismSectionsChange)
            {
                Observable = failureMechanism
            };

            Sections = sections;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            currentSections = Sections.ToArray();

            SetDataGridViewControlData();
        }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Sets the data in the failure mechanism sections data grid view control.
        /// </summary>
        protected virtual void SetDataGridViewControlData()
        {
            failureMechanismSectionsDataGridViewControl.SetDataSource(
                FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSections(
                    Sections,
                    CreateFailureMechanismSectionRow));
        }

        private static FailureMechanismSectionRow CreateFailureMechanismSectionRow(FailureMechanismSection section,
                                                                                   double sectionStart,
                                                                                   double sectionEnd)
        {
            return new FailureMechanismSectionRow(section, sectionStart, sectionEnd);
        }

        private void HandleFailureMechanismSectionsChange()
        {
            if (currentSections.SequenceEqual(Sections))
            {
                return;
            }

            currentSections = Sections.ToArray();

            SetDataGridViewControlData();
        }
    }
}