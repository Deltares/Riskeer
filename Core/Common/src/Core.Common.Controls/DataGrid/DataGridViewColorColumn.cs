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
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// This class defines a column of a data grid view in which the cells
    /// show a color.
    /// </summary>
    public class DataGridViewColorColumn : DataGridViewColumn
    {
        /// <summary>
        /// Creates a new instance of <see cref="DataGridViewColorColumn"/>.
        /// </summary>
        public DataGridViewColorColumn() : base(new DataGridViewColorCell()) {}

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value is DataGridViewColorCell)
                {
                    base.CellTemplate = value;
                }
                else throw new ArgumentException($@"Given template must be of type {typeof(DataGridViewColorCell)}", nameof(value));
            }
        }
    }
}