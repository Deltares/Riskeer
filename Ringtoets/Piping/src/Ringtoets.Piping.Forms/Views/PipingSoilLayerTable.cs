// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using Core.Common.Controls.DataGrid;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    public class PipingSoilLayerTable : DataGridViewControl
    {
        public PipingSoilLayerTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="layers"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="layers">The collection of layers to show.</param>
        public void SetData(IEnumerable<PipingSoilLayer> layers)
        {
            SetDataSource(layers);
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(PipingSoilLayer.MaterialName), "Naam", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.Color), "Kleur", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.Top), "Topniveau [m+NAP]", true);
            AddCheckBoxColumn(nameof(PipingSoilLayer.IsAquifer), "Is aquifer", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.PermeabilityMean), "Doorlatendheid (verwachtingswaarde) [m/s]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.PermeabilityDeviation), "Doorlatendheid (standaardafwijking) [m/s]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.DiameterD70Mean), "d70 (verwachtingswaarde) [m]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.DiameterD70Deviation), "d70 (standaardafwijking) [m]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.BelowPhreaticLevelMean), "Verzadigd gewicht (verwachtingswaarde) [kn/m³]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.BelowPhreaticLevelDeviation), "Verzadigd gewicht (standaardafwijking) [kn/m³]", true);
            AddTextBoxColumn(nameof(PipingSoilLayer.BelowPhreaticLevelShift), "Verzadigd gewicht (verschuiving) [kn/m³]", true);
        }
    }
}