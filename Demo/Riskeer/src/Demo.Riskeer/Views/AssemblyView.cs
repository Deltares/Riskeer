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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Gui;
using Core.Common.Util;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Riskeer.AssemblyTool.IO;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Common.Data.AssessmentSection;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerIntegrationPluginResources = Riskeer.Integration.Plugin.Properties.Resources;

namespace Demo.Riskeer.Views
{
    public partial class AssemblyView : UserControl, IMapView
    {
        private readonly IInquiryHelper inquiryHelper;
        private readonly MapDataCollection mapDataCollection;

        public AssemblyView(BackgroundData backgroundData, IInquiryHelper inquiryHelper)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.inquiryHelper = inquiryHelper;

            InitializeComponent();

            mapDataCollection = new MapDataCollection("test");
            riskeerMapControl.SetAllData(mapDataCollection, backgroundData);
        }

        public object Data { get; set; }

        public IMapControl Map
        {
            get
            {
                return riskeerMapControl.MapControl;
            }
        }

        private void ReadAssembly_Click(object sender, EventArgs e)
        {
            var fileFilterGenerator = new FileFilterGenerator(RiskeerIntegrationPluginResources.AssemblyResult_file_filter_Extension,
                                                              RiskeerCommonFormsResources.AssemblyResult_DisplayName);

            string filePath = inquiryHelper.GetSourceFileLocation(fileFilterGenerator.Filter);

            if (filePath == null)
            {
                return;
            }

            using (var reader = new SerializableAssemblyReader(filePath))
            {
                SerializableAssembly data = reader.Read();

                DisplayDataOnMap(data);
            }
        }

        private void DisplayDataOnMap(SerializableAssembly data)
        {
            mapDataCollection.Clear();

            SerializableFailureMechanismSection[] failureMechanismSections = data.FeatureMembers.OfType<SerializableFailureMechanismSection>().ToArray();

            var failureMechanismSectionFeatures = new List<MapFeature>();

            foreach (SerializableFeatureMember serializableFeatureMember in data.FeatureMembers)
            {
                var serializableAssessmentSection = serializableFeatureMember as SerializableAssessmentSection;
                if (serializableAssessmentSection != null)
                {
                    Point2D[] geometry = GetGeometry(serializableAssessmentSection.ReferenceLineGeometry);

                    mapDataCollection.Add(new MapLineData("Referentie lijn", new LineStyle
                                          {
                                              Color = Color.Blue,
                                              DashStyle = LineDashStyle.DashDot,
                                              Width = 5
                                          })
                                          {
                                              Features = new[]
                                              {
                                                  new MapFeature(new[]
                                                  {
                                                      new MapGeometry(new[]
                                                      {
                                                          geometry
                                                      })
                                                  })
                                              }
                                          });
                }

                var failureMechanismSectionAssembly = serializableFeatureMember as SerializableCombinedFailureMechanismSectionAssembly;
                if (failureMechanismSectionAssembly != null)
                {
                    SerializableFailureMechanismSection section = failureMechanismSections.Single(s => s.Id == failureMechanismSectionAssembly.FailureMechanismSectionId);

                    var feature = new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            GetGeometry(section.Geometry)
                        })
                    });
                    feature.MetaData.Add("Assembly", failureMechanismSectionAssembly.CombinedSectionResult.CategoryGroup);
                    failureMechanismSectionFeatures.Add(feature);
                }
            }

            mapDataCollection.Add(new MapLineData("Assemblage resultaat", new LineStyle(), CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme())
            {
                Features = failureMechanismSectionFeatures
            });

            mapDataCollection.NotifyObservers();
        }

        private static MapTheme<LineCategoryTheme> CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
        {
            return new MapTheme<LineCategoryTheme>("Assembly", new[]
            {
                CreateCategoryTheme(Color.FromArgb(255, 0, 255, 0), SerializableFailureMechanismSectionCategoryGroup.Iv),
                CreateCategoryTheme(Color.FromArgb(255, 118, 147, 60), SerializableFailureMechanismSectionCategoryGroup.IIv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 255, 0), SerializableFailureMechanismSectionCategoryGroup.IIIv),
                CreateCategoryTheme(Color.FromArgb(255, 204, 192, 218), SerializableFailureMechanismSectionCategoryGroup.IVv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 153, 0), SerializableFailureMechanismSectionCategoryGroup.Vv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 0, 0), SerializableFailureMechanismSectionCategoryGroup.VIv),
                CreateCategoryTheme(Color.FromArgb(255, 255, 255, 255), SerializableFailureMechanismSectionCategoryGroup.VIIv),
                CreateCategoryTheme(Color.FromArgb(0, 0, 0, 0), SerializableFailureMechanismSectionCategoryGroup.NotApplicable)
            });
        }

        private static LineCategoryTheme CreateCategoryTheme(Color color, SerializableFailureMechanismSectionCategoryGroup categoryGroup)
        {
            var lineStyle = new LineStyle
            {
                Color = color,
                DashStyle = LineDashStyle.Solid,
                Width = 6
            };

            return new LineCategoryTheme(CreateCriterion(categoryGroup), lineStyle);
        }

        private static ValueCriterion CreateCriterion(SerializableFailureMechanismSectionCategoryGroup category)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<SerializableFailureMechanismSectionCategoryGroup>(category).DisplayName);
        }

        private static Point2D[] GetGeometry(SerializableLine line)
        {
            string[] coordinates = line.LineString.Geometry.Split(' ');

            var geometry = new List<Point2D>();
            for (var i = 0; i < coordinates.Length - 1; i += 2)
            {
                var xCoordinate = Convert.ToDouble(coordinates[i], CultureInfo.InvariantCulture);
                var yCoordinate = Convert.ToDouble(coordinates[i + 1], CultureInfo.InvariantCulture);
                geometry.Add(new Point2D(xCoordinate, yCoordinate));
            }

            return geometry.ToArray();
        }
    }
}