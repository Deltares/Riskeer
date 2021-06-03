// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Style;

namespace Core.Common.Controls.Forms
{
    /// <summary>
    /// Custom tool strip renderer.
    /// </summary>
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is ToolStripButton btn)
            {
                if (btn.Checked || btn.Pressed || btn.Selected)
                {
                    var bounds = new Rectangle(Point.Empty, e.Item.Size);

                    e.Graphics.FillRectangle(new SolidBrush(ColorDefinitions.ButtonBackgroundColor), bounds);
                    e.Graphics.DrawRectangle(new Pen(ColorDefinitions.ButtonBorderColor), bounds);
                }

                btn.ForeColor = btn.Checked || btn.Pressed
                                    ? ColorDefinitions.ButtonActiveFrontColor
                                    : ColorDefinitions.ButtonInactiveFrontColor;

                return;
            }

            base.OnRenderButtonBackground(e);
        }
    }
}