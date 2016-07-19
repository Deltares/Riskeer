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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Commands
{
    /// <summary>
    /// This class provides concrete implementation for <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionFromFileCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionFromFileCommandHandler));

        private readonly IWin32Window dialogParent;
        private IEnumerable<AssessmentSectionSettings> settings;
        private IEnumerable<ReferenceLineMeta> referenceLineMetas = new List<ReferenceLineMeta>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionFromFileCommandHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        public AssessmentSectionFromFileCommandHandler(IWin32Window dialogParent)
        {
            this.dialogParent = dialogParent;
        }

        /// <summary>
        /// Creates a new <see cref="IAssessmentSection"/>, based upon the in a dialog selected <see cref="ReferenceLineMeta"/>, which is derived from the shape file in <paramref name="folderpath"/>.
        /// </summary>
        /// <param name="folderpath">The path to the folder where a shape file should be read.</param>
        /// <returns></returns>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain any polylines.</item>
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="folderpath"/> points to an invalid directory.</item>
        /// <item>The path <paramref name="folderpath"/> does not contain any shape files.</item>
        /// </list></exception>
        public IAssessmentSection CreateAssessmentSectionFromFile(string folderpath)
        {
            ValidateAssessmentSectionSettings();
            ValidateReferenceLineMetas(folderpath);

            using (var dialog = CreateReferenceLineMetaSelectionDialogWithItems())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }
                var selectedItem = dialog.SelectedReferenceLineMeta;
                return selectedItem == null ? null : CreateAssessmentSection(selectedItem, dialog.SelectedLimitValue);
            }
        }

        private IAssessmentSection CreateAssessmentSection(ReferenceLineMeta selectedItem, int? selectedLimitValue)
        {
            IAssessmentSection assessmentSection;
            var settingOfSelectedAssessmentSection = settings.FirstOrDefault(s => s.AssessmentSectionId == selectedItem.AssessmentSectionId);
            if (settingOfSelectedAssessmentSection == null)
            {
                log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_No_settings_found_for_AssessmentSection);
                assessmentSection = CreateDikeAssessmentSection();
            }
            else
            {
                assessmentSection = (settingOfSelectedAssessmentSection.IsDune) ?
                                        CreateDuneAssessmentSection() :
                                        CreateDikeAssessmentSection(settingOfSelectedAssessmentSection);
            }

            assessmentSection.Id = selectedItem.AssessmentSectionId;
            assessmentSection.ReferenceLine = selectedItem.ReferenceLine;

            if (selectedLimitValue.HasValue)
            {
                assessmentSection.FailureMechanismContribution.Norm = selectedLimitValue.Value;
            }
            return assessmentSection;
        }

        private static void SetFailureMechanismsValueN(AssessmentSection assessmentSection, int n)
        {
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = n;
            assessmentSection.HeightStructures.GeneralInput.N = n;
        }

        private ReferenceLineMetaSelectionDialog CreateReferenceLineMetaSelectionDialogWithItems()
        {
            return new ReferenceLineMetaSelectionDialog(dialogParent, referenceLineMetas);
        }

        #region Create AssessmentSection

        private static AssessmentSection CreateDikeAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            return assessmentSection;
        }

        private static AssessmentSection CreateDikeAssessmentSection(AssessmentSectionSettings settings)
        {
            var assessmentSection = CreateDikeAssessmentSection();
            SetFailureMechanismsValueN(assessmentSection, settings.N);
            return assessmentSection;
        }

        private static IAssessmentSection CreateDuneAssessmentSection()
        {
            return new AssessmentSection(AssessmentSectionComposition.Dune);
        }

        #endregion

        #region Validators

        private void ValidateReferenceLineMetas(string folderpath)
        {
            var importer = new ReferenceLineMetaImporter(folderpath);
            referenceLineMetas = importer.GetReferenceLineMetas().ToArray();

            if (!referenceLineMetas.Any())
            {
                throw new CriticalFileValidationException(Resources.AssessmentSectionFromFileCommandHandler_ValidateReferenceLineMetas_No_referenceLines_in_file);
            }
        }

        private void ValidateAssessmentSectionSettings()
        {
            var reader = new AssessmentSectionSettingsReader();
            settings = reader.ReadSettings();
        }

        #endregion
    }
}