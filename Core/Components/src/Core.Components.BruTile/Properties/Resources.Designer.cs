﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Core.Components.BruTile.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Core.Components.BruTile.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Een kritieke fout is opgetreden bij het aanmaken van de cache..
        /// </summary>
        internal static string PersistentCacheConfiguration_CreateTileCache_Critical_error_while_creating_tile_cache {
            get {
                return ResourceManager.GetString("PersistentCacheConfiguration_CreateTileCache_Critical_error_while_creating_tile_c" +
                        "ache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het pad naar bestandsmap &apos;{0}&apos; is niet geschikt om de kaart tegels in op te slaan..
        /// </summary>
        internal static string PersistentCacheConfiguration_Invalid_path_for_persistent_cache {
            get {
                return ResourceManager.GetString("PersistentCacheConfiguration_Invalid_path_for_persistent_cache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een databron is niet volgens het WMTS protocol aangeleverd..
        /// </summary>
        internal static string TileSourceFactory_GetWmtsTileSources_TileSource_without_WmtsTileSchema_error {
            get {
                return ResourceManager.GetString("TileSourceFactory_GetWmtsTileSources_TileSource_without_WmtsTileSchema_error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Niet in staat om de databronnen op te halen bij de WMTS URL &apos;{0}&apos;..
        /// </summary>
        internal static string TileSourceFactory_ParseWmtsTileSources_Cannot_connect_to_WMTS_0_ {
            get {
                return ResourceManager.GetString("TileSourceFactory_ParseWmtsTileSources_Cannot_connect_to_WMTS_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Niet in staat om de databron met naam &apos;{0}&apos; te kunnen vinden bij de WMTS URL &apos;{1}&apos;..
        /// </summary>
        internal static string WmtsLayerConfiguration_GetConfiguredTileSource_Cannot_find_LayerId_0_at_WmtsUrl_1_ {
            get {
                return ResourceManager.GetString("WmtsLayerConfiguration_GetConfiguredTileSource_Cannot_find_LayerId_0_at_WmtsUrl_1" +
                        "_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Afbeelding formaat moet opgegeven worden als MIME-type..
        /// </summary>
        internal static string WmtsLayerConfiguration_ValidateConfigurationParameters_PreferredFormat_must_be_mimetype {
            get {
                return ResourceManager.GetString("WmtsLayerConfiguration_ValidateConfigurationParameters_PreferredFormat_must_be_mi" +
                        "metype", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bron bevat geen WMTS schema..
        /// </summary>
        internal static string WmtsLayerConfiguration_ValidateTileSource_TileSource_must_have_WmtsTileSchema {
            get {
                return ResourceManager.GetString("WmtsLayerConfiguration_ValidateTileSource_TileSource_must_have_WmtsTileSchema", resourceCulture);
            }
        }
    }
}
