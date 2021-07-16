// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application.Riskeer.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Riskeer.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Riskeer versie {0} wordt gestart door {1}....
        /// </summary>
        internal static string App_Starting_Riskeer_version_0_by_user_0 {
            get {
                return ResourceManager.GetString("App_Starting_Riskeer_version_0_by_user_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Onbekende kritieke fout opgetreden..
        /// </summary>
        internal static string App_Unhandled_exception {
            get {
                return ResourceManager.GetString("App_Unhandled_exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De Helpdesk Water is gehuisvest bij de Rijkswaterstaat Water, Verkeer en Leefomgeving (RWS WVL). De helpdesk richt zich op het ontsluiten van kennis die aanwezig is binnen de werkvelden waterbeleid en waterbeheer in de breedste zin.
        ///
        ///Hierbij wordt intensief samengewerkt met diverse organisatieonderdelen zowel binnen Rijkswaterstaat als daar buiten. Op deze manier kan de helpdesk uw vragen snel en efficiënt beantwoorden, waarbij optimaal gebruik wordt gemaakt van de beschikbare kennis..
        /// </summary>
        internal static string HelpdeskWater_Description {
            get {
                return ResourceManager.GetString("HelpdeskWater_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Helpdesk Water.
        /// </summary>
        internal static string HelpdeskWater_DisplayName {
            get {
                return ResourceManager.GetString("HelpdeskWater_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon Riskeer {
            get {
                object obj = ResourceManager.GetObject("Riskeer", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
