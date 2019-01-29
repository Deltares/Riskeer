// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.ComponentModel;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// Base class for object properties with data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="Data"/>.</typeparam>
    public class ObjectProperties<T> : IObjectProperties
    {
        protected T data;

        public event EventHandler<EventArgs> RefreshRequired;

        [Browsable(false)]
        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (T) value;
            }
        }

        /// <summary>
        /// Method for raising <see cref="RefreshRequired"/>.
        /// </summary>
        protected void OnRefreshRequired()
        {
            RefreshRequired?.Invoke(this, new EventArgs());
        }
    }
}