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
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// A <see cref="UserControl"/> which can be used to display a list of <see cref="DikeProfile"/>
    /// from which a selection can be made.
    /// </summary>
    public partial class GrassCoverErosionInwardsDikeProfileSelectionView : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsDikeProfileSelectionView"/>. The given
        /// <paramref name="dikeProfiles"/> is used to fill the datagrid.
        /// </summary>
        /// <param name="dikeProfiles">The dike profiles to present in the view.</param>
        public GrassCoverErosionInwardsDikeProfileSelectionView(IEnumerable<DikeProfile> dikeProfiles)
        {
            if (dikeProfiles == null)
            {
                throw new ArgumentNullException("dikeProfiles");
            }
            InitializeComponent();
            DikeProfileDataGrid.AutoGenerateColumns = false;
            DikeProfileDataGrid.DataSource = dikeProfiles.Select(sl => new DikeProfileSelectionRow(sl)).ToArray();
        }

        /// <summary>
        /// Gets the currently selected dike profiles from the data grid view.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="DikeProfile"/>
        /// which were selected in the view.</returns>
        public IEnumerable<DikeProfile> GetSelectedDikeProfiles()
        {
            return DikeProfiles.Where(sl => sl.Selected).Select(sl => sl.DikeProfile).ToArray();
        }

        private IEnumerable<DikeProfileSelectionRow> DikeProfiles
        {
            get
            {
                return (IEnumerable<DikeProfileSelectionRow>) DikeProfileDataGrid.DataSource;
            }
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            foreach (var item in DikeProfiles)
            {
                item.Selected = true;
            }
            DikeProfileDataGrid.Invalidate();
        }

        private void OnSelectNoneClick(object sender, EventArgs e)
        {
            foreach (var item in DikeProfiles)
            {
                item.Selected = false;
            }
            DikeProfileDataGrid.Invalidate();
        }

        private class DikeProfileSelectionRow
        {
            public DikeProfileSelectionRow(DikeProfile dikeProfile)
            {
                Selected = false;
                Name = dikeProfile.Name;
                DikeProfile = dikeProfile;
            }

            public bool Selected { get; set; }
            public string Name { get; private set; }
            public DikeProfile DikeProfile { get; private set; }
        }
    }
}