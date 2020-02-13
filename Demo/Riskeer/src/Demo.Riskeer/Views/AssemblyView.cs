// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.Gis.Data;
using Riskeer.Common.Data.AssessmentSection;

namespace Demo.Riskeer.Views
{
    public partial class AssemblyView : UserControl, IView
    {
        public AssemblyView(BackgroundData backgroundData)
        {
            InitializeComponent();

            riskeerMapControl.SetAllData(new MapDataCollection("test"), backgroundData);
        }

        public object Data { get; set; }

        private void ReadAssembly_Click(object sender, EventArgs e)
        {
            Console.WriteLine("click!");
        }
    }
}