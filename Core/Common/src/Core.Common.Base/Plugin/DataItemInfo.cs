// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Drawing;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that holds information for creating data objects.
    /// </summary>
    public class DataItemInfo
    {
        /// <summary>
        /// Constructs a new <see cref="DataItemInfo"/>.
        /// </summary>
        public DataItemInfo()
        {
            Name = "";
            Category = "";
        }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the data to create.
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// Gets or sets the name of the data to create.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the data to create.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the data to create.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets a method for determining whether or not the data item information is relevant for the proposed owner.
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Gets or sets a function for creating the data.
        /// The object parameter holds the proposed owner of the data to create.
        /// </summary>
        public Func<object, object> CreateData { get; set; }
    }

    /// <summary>
    /// Class that holds information for creating data objects.
    /// </summary>
    /// <typeparam name="TValue">The type of data to create.</typeparam>
    public class DataItemInfo<TValue>
    {
        /// <summary>
        /// Constructs a new <see cref="DataItemInfo{TValue}"/>.
        /// </summary>
        public DataItemInfo()
        {
            Name = "";
            Category = "";
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the data to create.
        /// </summary>
        public Type ValueType
        {
            get
            {
                return typeof(TValue);
            }
        }

        /// <summary>
        /// Gets or sets the name of the data to create.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the data to create.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the data to create.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets a method for determining whether or not the data item information is relevant for the proposed owner.
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Gets or sets a function for creating the data.
        /// The object parameter holds the proposed owner of the data to create.
        /// </summary>
        public Func<object, TValue> CreateData { get; set; }

        /// <summary>
        /// This operator converts a <see cref="DataItemInfo{TValue}"/> into a <see cref="DataItemInfo"/>.
        /// </summary>
        /// <param name="dataItemInfo">The <see cref="DataItemInfo{TValue}"/> to convert.</param>
        /// <returns>The converted <see cref="DataItemInfo"/>.</returns>
        public static implicit operator DataItemInfo(DataItemInfo<TValue> dataItemInfo)
        {
            return new DataItemInfo
            {
                ValueType = dataItemInfo.ValueType,
                Name = dataItemInfo.Name,
                Category = dataItemInfo.Category,
                Image = dataItemInfo.Image,
                AdditionalOwnerCheck = dataItemInfo.AdditionalOwnerCheck != null
                                           ? owner => dataItemInfo.AdditionalOwnerCheck(owner)
                                           : (Func<object, bool>) null,
                CreateData = dataItemInfo.CreateData != null
                                 ? owner => dataItemInfo.CreateData(owner)
                                 : (Func<object, object>) null
            };
        }
    }
}