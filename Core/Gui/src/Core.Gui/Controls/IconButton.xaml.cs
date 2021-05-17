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
using System.Windows.Controls;

namespace Core.Gui.Controls
{
    /// <summary>
    /// Interaction logic for <see cref="IconButton"/>.
    /// </summary>
    public partial class IconButton : Button
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(IconButton), new PropertyMetadata(default(string)));

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(string), typeof(IconButton), new PropertyMetadata(default(string)));

        /// <summary>
        /// Identifies the <see cref="HeaderSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderSizeProperty = DependencyProperty.Register(
            nameof(HeaderSize), typeof(double), typeof(IconButton), new PropertyMetadata(default(double)));

        /// <summary>
        /// Identifies the <see cref="IconSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(double), typeof(IconButton), new PropertyMetadata(default(double)));

        /// <summary>
        /// Creates a new instance of <see cref="IconButton"/>.
        /// </summary>
        public IconButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get => (string) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the header.
        /// </summary>
        public double HeaderSize
        {
            get => (double) GetValue(HeaderSizeProperty);
            set => SetValue(HeaderSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        public double IconSize
        {
            get => (double) GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
    }
}