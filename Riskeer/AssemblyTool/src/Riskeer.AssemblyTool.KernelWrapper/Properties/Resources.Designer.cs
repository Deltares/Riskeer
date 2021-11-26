﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.AssemblyTool.KernelWrapper.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.AssemblyTool.KernelWrapper.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Categoriebovengrens moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_CategoryLowerLimitOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_CategoryLowerLimitOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het specificeren van een toetsoordeel voor deze categorie is niet mogelijk..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_CategoryNotAllowed {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_CategoryNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Categorieondergrens moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_CategoryUpperLimitOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_CategoryUpperLimitOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ieder toetsspoor in de assemblage moet een vakindeling geïmporteerd hebben..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_CommonFailurePathSectionsInvalid {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_CommonFailurePathSectionsInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Alle (deel)vakken moeten minimaal een lengte hebben van 0.01 [m]..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_CommonFailurePathSectionsNotConsecutive {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_CommonFailurePathSectionsNotConsecutive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een lagere categorie moet als voldoende worden aangemerkt indien het vak aan een hogere categorie voldoet..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_DoesNotComplyAfterComply {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_DoesNotComplyAfterComply", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er ontbreekt invoer voor de assemblage rekenmodule waardoor de assemblage niet uitgevoerd kan worden..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_EmptyResultsList {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_EmptyResultsList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gezamenlijke lengte van alle deelvakken moet gelijk zijn aan de trajectlengte..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_FailurePathSectionLengthInvalid {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_FailurePathSectionLengthInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De lengte van een berekende deelvak kon niet goed worden bepaald..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_FailurePathSectionSectionStartEndInvalid {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_FailurePathSectionSectionStartEndInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Faalkansruimte moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_FailureProbabilityMarginOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_FailureProbabilityMarginOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De gespecificeerde kans moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_FailureProbabilityOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_FailureProbabilityOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een onverwachte fout opgetreden tijdens het assembleren..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_GenericErrorMessage {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_GenericErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De categoriegrenzen zijn niet aaneengesloten en spannen niet de volldige faalskansruimte..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_GetErrorMessage_InvalidCategoryLimits {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_GetErrorMessage_InvalidCategoryLimits", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De resultaten voor alle vakken moeten allen wel of geen kansspecificatie bevatten..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_InputNotTheSameType {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_InputNotTheSameType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lengte-effect factor moet minimaal 1 zijn..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_LengthEffectFactorOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_LengthEffectFactorOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De categoriebovengrens moet boven de categorieondergrens liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_LowerLimitIsAboveUpperLimit {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_LowerLimitIsAboveUpperLimit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ondergrens moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_LowerLimitOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_LowerLimitOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er moet een vakindeling zijn geïmporteerd..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_NoSectionsImported {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_NoSectionsImported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekende ondergrens per doorsnede is groter dan de ondergrens van het traject..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_PlowDsnAbovePlow {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_PlowDsnAbovePlow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De faalkans per vak moet groter of gelijk zijn aan de faalkans per doorsnede..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_ProfileProbabilityGreaterThanSectionProbability {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_ProfileProbabilityGreaterThanSectionProbability", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekende signaleringskans per doorsnede is groter dan de signaleringskans van het traject..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_PsigDsnAbovePsig {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_PsigDsnAbovePsig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De gespecificeerde resultaten voor een of meerdere toetssporen dekken niet de volledige lengte van het traject..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_RequestedPointOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_RequestedPointOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De trajectlengte moet groter zijn dan 0 [m]..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_SectionLengthOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_SectionLengthOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er zijn een of meerdere vakindelingen gevonden die geen categorie hebben..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_SectionsWithoutCategory {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_SectionsWithoutCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De signaleringskans moet kleiner zijn dan de ondergrens..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_SignalingLimitAboveLowerLimit {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_SignalingLimitAboveLowerLimit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Signaleringskans moet in het bereik [0,1] liggen..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_SignalingLimitOutOfRange {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_SignalingLimitOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een ongeldig resultaat gespecificeerd voor de gebruikte methode..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_TranslateAssessmentInvalidInput {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_TranslateAssessmentInvalidInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is ongeldige invoer gedefinieerd voor de gebruikte methode..
        /// </summary>
        internal static string AssemblyErrorMessageCreator_ValueMayNotBeNull {
            get {
                return ResourceManager.GetString("AssemblyErrorMessageCreator_ValueMayNotBeNull", resourceCulture);
            }
        }
    }
}
