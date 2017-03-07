// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Gui
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
            return GetSourceFileLocation(new FileFilterGenerator());
        }

        public string GetSourceFileLocation(FileFilterGenerator filter)
        {
            string filePath = null;
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = Resources.OpenFileDialog_Title,
                Filter = filter.Filter
            })
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }
            return filePath;
        }

        public string GetTargetFileLocation()
        {
            return GetTargetFileLocation(new FileFilterGenerator(), null);
        }

        public string GetTargetFileLocation(FileFilterGenerator filter, string suggestedFileName)
        {
            string filePath = null;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                Title = Resources.SaveFileDialog_Title,
                Filter = filter.Filter,
                FileName = suggestedFileName
            })
            {
                DialogResult result = dialog.ShowDialog();
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
    }
}