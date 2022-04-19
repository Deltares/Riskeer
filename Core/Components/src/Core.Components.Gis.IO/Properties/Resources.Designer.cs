﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core.Components.Gis.IO.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Core.Components.Gis.IO.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;!--
        ///Copyright (C) Stichting Deltares 2022. All rights reserved.
        ///
        ///This file is part of Riskeer.
        ///
        ///Riskeer is free software: you can redistribute it and/or modify
        ///it under the terms of the GNU Lesser General Public License as published by
        ///the Free Software Foundation, either version 3 of the License, or
        ///(at your option) any later version.
        ///
        ///This program is distributed in the hope that it will be useful,
        ///but WITHOUT ANY WARRANTY; without even the implied warr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string defaultWmtsConnectionInfo {
            get {
                return ResourceManager.GetString("defaultWmtsConnectionInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen kaartlaag geïmporteerd..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_HandleCriticalFileReadError_Error_0_no_maplayer_imported {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_HandleCriticalFileReadError_Error_0_no_maplayer_impor" +
                        "ted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kaartlaag toevoegen afgebroken. Geen gegevens gewijzigd..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_HandleUserCancelingImport_Import_canceled_No_data_changed {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_HandleUserCancelingImport_Import_canceled_No_data_cha" +
                        "nged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_Import_An_error_occurred_when_trying_to_read_the_file {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_Import_An_error_occurred_when_trying_to_read_the_file" +
                        "", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen geometrieën vinden in dit bestand..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_Import_File_does_not_contain_geometries {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_Import_File_does_not_contain_geometries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand of andere benodigde bestanden zijn niet gevonden..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_Import_File_does_not_exist_or_misses_needed_files {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_Import_File_does_not_exist_or_misses_needed_files", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De data in het shapebestand wordt niet ondersteund..
        /// </summary>
        internal static string FeatureBasedMapDataImporter_Import_ShapeFile_Contains_Unsupported_Data {
            get {
                return ResourceManager.GetString("FeatureBasedMapDataImporter_Import_ShapeFile_Contains_Unsupported_Data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen punten vinden in dit bestand..
        /// </summary>
        internal static string PointShapeFileReader_File_contains_geometries_not_points {
            get {
                return ResourceManager.GetString("PointShapeFileReader_File_contains_geometries_not_points", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen polygonen vinden in dit bestand..
        /// </summary>
        internal static string PointShapeFileReader_File_contains_geometries_not_polygons {
            get {
                return ResourceManager.GetString("PointShapeFileReader_File_contains_geometries_not_polygons", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Punten.
        /// </summary>
        internal static string PointShapeFileReader_ReadLine_Points {
            get {
                return ResourceManager.GetString("PointShapeFileReader_ReadLine_Points", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een feature mag maar één geometrie bevatten..
        /// </summary>
        internal static string PointShapeFileWriter_CreatePointFromMapFeature_A_feature_can_only_contain_one_geometry {
            get {
                return ResourceManager.GetString("PointShapeFileWriter_CreatePointFromMapFeature_A_feature_can_only_contain_one_geo" +
                        "metry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Polygoon.
        /// </summary>
        internal static string PolygonShapeFileReader_ReadLine_Polygon {
            get {
                return ResourceManager.GetString("PolygonShapeFileReader_ReadLine_Polygon", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon geen lijnen vinden in dit bestand..
        /// </summary>
        internal static string PolylineShapeFileReader_File_contains_geometries_not_line {
            get {
                return ResourceManager.GetString("PolylineShapeFileReader_File_contains_geometries_not_line", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lijn.
        /// </summary>
        internal static string PolylineShapeFileReader_ReadLine_Line {
            get {
                return ResourceManager.GetString("PolylineShapeFileReader_ReadLine_Line", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mapdata mag maar één feature bevatten..
        /// </summary>
        internal static string ShapeFileWriterBase_CopyToFeature_Mapdata_can_only_contain_one_feature {
            get {
                return ResourceManager.GetString("ShapeFileWriterBase_CopyToFeature_Mapdata_can_only_contain_one_feature", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het is niet mogelijk om WMTS connectie &apos;{0}&apos; aan te maken met URL &apos;{1}&apos;..
        /// </summary>
        internal static string WmtsConnectionInfoReader_Unable_To_Create_WmtsConnectionInfo {
            get {
                return ResourceManager.GetString("WmtsConnectionInfoReader_Unable_To_Create_WmtsConnectionInfo", resourceCulture);
            }
        }
    }
}
