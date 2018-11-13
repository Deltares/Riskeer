// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Style;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// The map view for the assembly result per section for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>. 
    /// </summary>
    public partial class AssemblyResultPerSectionMapView : UserControl, IMapView
    {
        private readonly MapLineData assemblyResultLineData;
        private readonly MapLineData referenceLineMapData;
        private readonly MapPointData hydraulicBoundaryLocationsMapData;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultPerSectionMapView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssemblyResultPerSectionMapView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
            InitializeComponent();

            var mapDataCollection = new MapDataCollection("Assemblagekaart");
            assemblyResultLineData = new MapLineData("Gecombineerd vakoordeel",
                                                     new LineStyle
                                                     {
                                                         Width = 6,
                                                         DashStyle = LineDashStyle.Solid
                                                     });
            referenceLineMapData = RingtoetsMapDataFactory.CreateReferenceLineMapData();
            hydraulicBoundaryLocationsMapData = RingtoetsMapDataFactory.CreateHydraulicBoundaryLocationsMapData();

            mapDataCollection.Add(assemblyResultLineData);
            mapDataCollection.Add(referenceLineMapData);
            mapDataCollection.Add(hydraulicBoundaryLocationsMapData);

            SetAllMapDataFeatures();

            ringtoetsMapControl.SetAllData(mapDataCollection, assessmentSection.BackgroundData);
        }

        /// <summary>
        /// Gets the <see cref="Integration.Data.AssessmentSection"/> the view belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return ringtoetsMapControl.MapControl;
            }
        }

        private void SetAllMapDataFeatures()
        {
            SetReferenceLineMapData();
            SetHydraulicBoundaryLocationsMapData();
        }

        #region ReferenceLine MapData

        private void UpdateReferenceLineMapData()
        {
            SetReferenceLineMapData();
            referenceLineMapData.NotifyObservers();
        }

        private void SetReferenceLineMapData()
        {
            referenceLineMapData.Features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(AssessmentSection.ReferenceLine,
                                                                                                        AssessmentSection.Id,
                                                                                                        AssessmentSection.Name);
        }

        #endregion

        #region HydraulicBoundaryLocations MapData

        private void UpdateHydraulicBoundaryLocationsMapData()
        {
            SetHydraulicBoundaryLocationsMapData();
            hydraulicBoundaryLocationsMapData.NotifyObservers();
        }

        private void SetHydraulicBoundaryLocationsMapData()
        {
            hydraulicBoundaryLocationsMapData.Features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(AssessmentSection);
        }

        #endregion
    }
}