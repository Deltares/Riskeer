// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Windows.Forms;

namespace Core.Common.Controls.Forms
{
    /// <summary>
    /// Button that also responds to clicks when its containing form does not have input focus.
    /// </summary>
    public class EnhancedButton : Button
    {
        const uint WM_LBUTTONDOWN = 0x201;
        const uint WM_LBUTTONUP = 0x202;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP)
            {
                SimulateMouseClick(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void SimulateMouseClick(ref Message m)
        {
            m.Msg = (int) WM_LBUTTONDOWN;
            base.WndProc(ref m);

            m.Msg = (int) WM_LBUTTONUP;
            base.WndProc(ref m);
        }
    }
}