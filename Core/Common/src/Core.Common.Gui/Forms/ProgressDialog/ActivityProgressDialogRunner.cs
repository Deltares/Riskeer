// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base.Service;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    /// <summary>
    /// Helper methods for running a sequence of activities and observe their progress in a dialog.
    /// </summary>
    public static class ActivityProgressDialogRunner
    {
        /// <summary>
        /// Runs a given activity while showing progress in a dialog.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which the progress dialog should be shown on top.</param>
        /// <param name="activity">The activity to be executed.</param>
        public static void Run(IWin32Window dialogParent, Activity activity)
        {
            Run(dialogParent, new[]
            {
                activity
            });
        }

        /// <summary>
        /// Runs a sequence of activities of type <typeparamref name="TActivity"/> while showing progress in a dialog.
        /// </summary>
        /// <typeparam name="TActivity">The activity type.</typeparam>
        /// <param name="dialogParent">The dialog parent for which the progress dialog should be shown on top.</param>
        /// <param name="activities">The activities to be executed.</param>
        public static void Run<TActivity>(IWin32Window dialogParent, IEnumerable<TActivity> activities)
            where TActivity : Activity
        {
            using (var activityProgressDialog = new ActivityProgressDialog(dialogParent, activities))
            {
                activityProgressDialog.ShowDialog();
            }
        }
    }
}