﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Components.Gis.Forms.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Core.Components.Gis.Forms.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap InformationIcon {
            get {
                object obj = ResourceManager.GetObject("InformationIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap MapsIcon {
            get {
                object obj = ResourceManager.GetObject("MapsIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Coördinatenstelsel.
        /// </summary>
        public static string WmtsCapability_MapLayer_CoordinateSystem {
            get {
                return ResourceManager.GetString("WmtsCapability_MapLayer_CoordinateSystem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Formaat.
        /// </summary>
        public static string WmtsCapability_MapLayer_Format {
            get {
                return ResourceManager.GetString("WmtsCapability_MapLayer_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kaartlaag.
        /// </summary>
        public static string WmtsCapability_MapLayer_Id {
            get {
                return ResourceManager.GetString("WmtsCapability_MapLayer_Id", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Titel.
        /// </summary>
        public static string WmtsCapability_MapLayer_Title {
            get {
                return ResourceManager.GetString("WmtsCapability_MapLayer_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WMTS locatie aanpassen.
        /// </summary>
        public static string WmtsConnectionDialog_Text_Edit {
            get {
                return ResourceManager.GetString("WmtsConnectionDialog_Text_Edit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URL naar &apos;capabilities&apos; van de webservice..
        /// </summary>
        public static string WmtsConnectionDialog_UrlErrorProvider_HelpText {
            get {
                return ResourceManager.GetString("WmtsConnectionDialog_UrlErrorProvider_HelpText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Web Map Tile Service (WMTS).
        /// </summary>
        public static string WmtsLocationControl_DisplayName {
            get {
                return ResourceManager.GetString("WmtsLocationControl_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gegevens ophalen van de locatie (URL) &apos;{0}&apos; is mislukt..
        /// </summary>
        public static string WmtsLocationControl_Unable_to_connect_to_0 {
            get {
                return ResourceManager.GetString("WmtsLocationControl_Unable_to_connect_to_0", resourceCulture);
            }
        }
    }
}
