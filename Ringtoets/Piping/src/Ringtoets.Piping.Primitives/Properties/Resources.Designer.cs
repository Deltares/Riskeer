﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Piping.Primitives.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Piping.Primitives.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Slootbodem dijkzijde.
        /// </summary>
        public static string CharacteristicPoint_BottomDitchDikeSide {
            get {
                return ResourceManager.GetString("CharacteristicPoint_BottomDitchDikeSide", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Slootbodem polderzijde.
        /// </summary>
        public static string CharacteristicPoint_BottomDitchPolderSide {
            get {
                return ResourceManager.GetString("CharacteristicPoint_BottomDitchPolderSide", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Teen dijk binnenwaarts.
        /// </summary>
        public static string CharacteristicPoint_DikeToeAtPolder {
            get {
                return ResourceManager.GetString("CharacteristicPoint_DikeToeAtPolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Teen dijk buitenwaarts.
        /// </summary>
        public static string CharacteristicPoint_DikeToeAtRiver {
            get {
                return ResourceManager.GetString("CharacteristicPoint_DikeToeAtRiver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Insteek sloot dijkzijde.
        /// </summary>
        public static string CharacteristicPoint_DitchDikeSide {
            get {
                return ResourceManager.GetString("CharacteristicPoint_DitchDikeSide", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Insteek sloot polderzijde.
        /// </summary>
        public static string CharacteristicPoint_DitchPolderSide {
            get {
                return ResourceManager.GetString("CharacteristicPoint_DitchPolderSide", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen..
        /// </summary>
        public static string DesignVariable_GetDesignValue_Distribution_must_be_set {
            get {
                return ResourceManager.GetString("DesignVariable_GetDesignValue_Distribution_must_be_set", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Percentiel moet in het bereik [0, 1] liggen..
        /// </summary>
        public static string DesignVariable_Percentile_must_be_in_range {
            get {
                return ResourceManager.GetString("DesignVariable_Percentile_must_be_in_range", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen lagen gevonden voor de ondergrondschematisatie..
        /// </summary>
        public static string Error_Cannot_Construct_PipingSoilProfile_Without_Layers {
            get {
                return ResourceManager.GetString("Error_Cannot_Construct_PipingSoilProfile_Without_Layers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Eén of meerdere lagen hebben een top onder de bodem van de ondergrondschematisatie..
        /// </summary>
        public static string PipingSoilProfile_Layers_Layer_top_below_profile_bottom {
            get {
                return ResourceManager.GetString("PipingSoilProfile_Layers_Layer_top_below_profile_bottom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} De lokale coördinaat moet in het bereik [{1}, {2}] liggen..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_0_L_needs_to_be_in_1_2_range {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_0_L_needs_to_be_in_1_2_range", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een punt in de geometrie voor de profielschematisatie heeft geen waarde..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen hoogte bepalen op het punt met de lokale coördinaat {0}, omdat de profielschematisatie verticaal loopt op dat punt..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_verti" +
                        "cal_in_l", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De geometrie die opgegeven werd voor de profielschematisatie heeft geen waarde..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen hoogte bepalen..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_GetZAtL_Cannot_determine_height {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_GetZAtL_Cannot_determine_height", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De geometrie bevat geen punt op locatie {0} om als &apos;{1}&apos; in te stellen..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_ {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_poi" +
                        "nt_at_0_to_assign_as_characteristic_point_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De profielschematisatie heeft geen geometrie..
        /// </summary>
        public static string RingtoetsPipingSurfaceLine_SurfaceLine_has_no_Geometry {
            get {
                return ResourceManager.GetString("RingtoetsPipingSurfaceLine_SurfaceLine_has_no_Geometry", resourceCulture);
            }
        }
    }
}
