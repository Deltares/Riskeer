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

using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Core.Components.GraphSharp.Commands;
using GraphSharp.Controls;

namespace Core.Components.GraphSharp.Data
{
    /// <summary>
    /// Class describing the vertex to show in the <see cref="GraphLayout"/>.
    /// </summary>
    public class PointedTreeElementVertex : INotifyPropertyChanged
    {
        private ICommand selectedItemCommand;
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeElementVertex"/>.
        /// </summary>
        /// <param name="content">The content of the vertex.</param>
        /// <param name="fillColor">The fill color of the vertex.</param>
        /// <param name="lineColor">The line color of the vertex.</param>
        /// <param name="lineWidth">The line width of the vertex.</param>
        /// <param name="type">The type of the vertex.</param>
        /// <param name="isSelectable">Indicator whether the vertex is selectable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        public PointedTreeElementVertex(string content, Color fillColor, Color lineColor, int lineWidth,
                                        PointedTreeVertexType type, bool isSelectable)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Content = content;
            FillColor = new SolidColorBrush(fillColor);
            LineColor = new SolidColorBrush(lineColor);
            LineWidth = lineWidth;
            Type = type;
            IsSelectable = isSelectable;
        }

        /// <summary>
        /// Gets the content of the vertex.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the fill color of the vertex.
        /// </summary>
        public Brush FillColor { get; }

        /// <summary>
        /// Gets the line color of the vertex.
        /// </summary>
        public Brush LineColor { get; }

        /// <summary>
        /// Gets the line width of the vertex.
        /// </summary>
        public int LineWidth { get; }

        /// <summary>
        /// Gets the type of the vertex.
        /// </summary>
        public PointedTreeVertexType Type { get; }

        /// <summary>
        /// Gets whether the vertex is selectable.
        /// </summary>
        public bool IsSelectable { get; }

        /// <summary>
        /// Gets whether the vertex is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Gets the command for selecting the vertex.
        /// </summary>
        public ICommand VertexSelectedCommand
        {
            get
            {
                return selectedItemCommand ?? (selectedItemCommand = new VertexSelectedCommand(this));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}