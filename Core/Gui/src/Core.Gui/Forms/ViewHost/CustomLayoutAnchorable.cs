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

using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Gui.Forms.ViewHost
{
    /// <summary>
    /// Custom implementation of <see cref="LayoutAnchorable"/>.
    /// </summary>
    public class CustomLayoutAnchorable : LayoutAnchorable
    {
        /// <summary>
        /// The symbol <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            "Symbol", typeof(string), typeof(CustomLayoutAnchorable), new PropertyMetadata(default(string)));

        /// <summary>
        /// The font family <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily", typeof(FontFamily), typeof(CustomLayoutDocument), new PropertyMetadata(default(FontFamily)));

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string Symbol
        {
            get => (string) GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        /// <summary>
        /// Gets or sets the font family of the symbol.
        /// </summary>
        public FontFamily FontFamily
        {
            get => (FontFamily) GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
    }
}