// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Core.Common.Controls.Views;
using Core.Gui;
using Core.Gui.Plugin;

namespace Riskeer.Common.Plugin
{
    /// <inheritdoc cref="ViewInfo{TData,TViewData,TView}"/>
    public class RiskeerViewInfo<TData, TViewData, TView> : ViewInfo<TData, TViewData, TView>
        where TView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiskeerViewInfo{TData, TViewData, TView}"/> class.
        /// </summary>
        /// <param name="getGuiFunc">The function for retrieving the <see cref="IGui"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="getGuiFunc"/> equals <c>null</c>.</exception>
        public RiskeerViewInfo(Func<IGui> getGuiFunc)
        {
            if (getGuiFunc == null)
            {
                throw new ArgumentNullException(nameof(getGuiFunc));
            }

            Symbol = getGuiFunc()?.ActiveStateInfo?.Symbol;
            FontFamily = getGuiFunc()?.ActiveStateInfo?.FontFamily;
        }
    }

    /// <inheritdoc cref="ViewInfo{TData, TViewData, TView}"/>
    public class RiskeerViewInfo<TData, TView>
        : RiskeerViewInfo<TData, TData, TView>
        where TView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiskeerViewInfo{TData, TView}"/> class.
        /// </summary>
        /// <param name="getGuiFunc">The function for retrieving the <see cref="IGui"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="getGuiFunc"/> equals <c>null</c>.</exception>
        public RiskeerViewInfo(Func<IGui> getGuiFunc) : base(getGuiFunc) {}
    }
}