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

using System.Linq;
using System.Windows.Forms.Integration;
using Core.Common.Controls.Views;
using Core.Gui.Forms.ViewHost;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Gui.TestUtil
{
    /// <summary>
    /// Contains routines for testing classes which are related to <see cref="AvalonDockViewHost"/>.
    /// </summary>
    public static class AvalonDockViewHostTestHelper
    {
        /// <summary>
        /// Returns whether <paramref name="title"/> is set for <paramref name="view"/>.
        /// </summary>
        /// <param name="avalonDockViewHost">The view host that owns the view.</param>
        /// <param name="view">The view to check the title for.</param>
        /// <param name="title">The title to check for.</param>
        /// <returns>Whether <paramref name="title"/> is set for <paramref name="view"/>.</returns>
        public static bool IsTitleSet(AvalonDockViewHost avalonDockViewHost, IView view, string title)
        {
            return avalonDockViewHost.DockingManager
                                     .Layout
                                     .Descendents()
                                     .OfType<LayoutContent>()
                                     .First(lc => ((WindowsFormsHost) lc.Content).Child == view).Title == title;
        }
    }
}