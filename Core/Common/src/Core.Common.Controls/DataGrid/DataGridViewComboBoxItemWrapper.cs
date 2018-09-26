// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.Properties;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Wrapper for presenting <typeparamref name="T"/> items in a <see cref="DataGridViewComboBoxColumn"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to present in the <see cref="DataGridViewComboBoxColumn"/>.</typeparam>
    public class DataGridViewComboBoxItemWrapper<T>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DataGridViewComboBoxItemWrapper{T}"/>.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        public DataGridViewComboBoxItemWrapper(T wrappedObject)
        {
            WrappedObject = wrappedObject;
        }

        /// <summary>
        /// Gets the display name for the combobox item.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return WrappedObject == null ? Resources.DisplayName_None : WrappedObject.ToString();
            }
        }

        /// <summary>
        /// Gets a reference to the current wrapper instance.
        /// </summary>
        public DataGridViewComboBoxItemWrapper<T> This
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the wrapped object.
        /// </summary>
        public T WrappedObject { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DataGridViewComboBoxItemWrapper<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(WrappedObject);
        }

        public override string ToString()
        {
            return DisplayName;
        }

        private bool Equals(DataGridViewComboBoxItemWrapper<T> other)
        {
            return EqualityComparer<T>.Default.Equals(WrappedObject, other.WrappedObject);
        }
    }
}