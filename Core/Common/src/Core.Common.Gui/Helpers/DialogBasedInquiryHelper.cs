// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using Core.Common.Util;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Gui.Helpers
{
    /// <summary>
    /// Class which inquires the user for application inputs based on dialogs.
    /// </summary>
    public class DialogBasedInquiryHelper : IInquiryHelper
    {
        private readonly IWin32Window dialogParent;

        /// <summary>
        /// Creates a new instance of <see cref="DialogBasedInquiryHelper"/>.
        /// </summary>
        /// <param name="dialogParent">The parent window to provide to the created
        /// dialogs.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public DialogBasedInquiryHelper(IWin32Window dialogParent)
        {
            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }

            this.dialogParent = dialogParent;
        }

        public string GetSourceFileLocation()
        {
            return GetSourceFileLocation(new FileFilterGenerator().Filter);
        }

        public string GetSourceFileLocation(string fileFilter)
        {
            string filePath = null;
            using (var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = Resources.OpenFileDialog_Title,
                Filter = fileFilter
            })
            {
                DialogResult result = dialog.ShowDialog(dialogParent);
                if (result == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }

            return filePath;
        }

        public string GetTargetFileLocation()
        {
            return GetTargetFileLocation(new FileFilterGenerator().Filter, null);
        }

        public string GetTargetFileLocation(string fileFilter, string suggestedFileName)
        {
            string filePath = null;
            using (var dialog = new SaveFileDialog
            {
                Title = Resources.SaveFileDialog_Title,
                Filter = fileFilter,
                FileName = suggestedFileName
            })
            {
                DialogResult result = dialog.ShowDialog(dialogParent);
                if (result == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }

            return filePath;
        }

        public bool InquireContinuation(string query)
        {
            DialogResult dialog = MessageBox.Show(
                dialogParent,
                query,
                CoreCommonBaseResources.Confirm,
                MessageBoxButtons.OKCancel);
            return dialog == DialogResult.OK;
        }

        public OptionalStepResult InquirePerformOptionalStep(string workflowDescription, string query)
        {
            DialogResult confirmation = MessageBox.Show(dialogParent,
                                                        query,
                                                        workflowDescription,
                                                        MessageBoxButtons.YesNoCancel);

            if (!Enum.IsDefined(typeof(DialogResult), confirmation))
            {
                throw new InvalidEnumArgumentException(nameof(confirmation),
                                                       (int) confirmation,
                                                       typeof(DialogResult));
            }

            switch (confirmation)
            {
                case DialogResult.Cancel:
                    return OptionalStepResult.Cancel;
                case DialogResult.Yes:
                    return OptionalStepResult.PerformOptionalStep;
                case DialogResult.No:
                    return OptionalStepResult.SkipOptionalStep;
                default:
                    throw new NotSupportedException("Dialogbox should only return the above values.");
            }
        }
    }
}