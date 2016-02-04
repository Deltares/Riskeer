// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.Charting.Data;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// This class describes the command for opening a view for <see cref="ChartData"/> with some arbitrary data.
    /// </summary>
    public class OpenChartViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

        public OpenChartViewCommand(IDocumentViewController documentViewController)
        {
            this.documentViewController = documentViewController;
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute(params object[] arguments)
        {
            var line = new LineData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)    
            });
            var area = new AreaData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6),
                new Tuple<double, double>(1.6, 0.5),
                new Tuple<double, double>(0.0, 0.5),
                new Tuple<double, double>(0.0, 1.1)
            });
            var clearArea = new AreaData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.5, 0.5),    
                new Tuple<double, double>(0.5, 1.0),
                new Tuple<double, double>(1.0, 1.0),
                new Tuple<double, double>(0.5, 0.5)
            });
            var points = new PointData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(0.5, 1.6),  
                new Tuple<double, double>(1.0, 2.1)
            });
            documentViewController.DocumentViewsResolver.OpenViewForData(new ChartDataCollection(new List<ChartData> { area, clearArea, line, points }));
        }
    }
}