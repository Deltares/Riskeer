﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Integration.Plugin.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Integration.Plugin.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Als u de referentielijn vervangt zullen alle vakindelingen, berekende hydraulische randvoorwaarden en berekeningsresultaten worden verwijderd.
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        internal static string ReferenceLineImporter_ConfirmImport_Confirm_referenceline_import_which_clears_data_when_performed {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ConfirmImport_Confirm_referenceline_import_which_clears_dat" +
                        "a_when_performed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bevestigen.
        /// </summary>
        internal static string ReferenceLineImporter_ConfirmImport_DialogTitle {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ConfirmImport_DialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen referentielijn geïmporteerd..
        /// </summary>
        internal static string ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_imported {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_HandleCriticalFileReadError_Error_0_no_referenceline_import" +
                        "ed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geïmporteerde data toevoegen aan het traject..
        /// </summary>
        internal static string ReferenceLineImporter_ProgressText_Adding_imported_referenceline_to_assessmentsection {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ProgressText_Adding_imported_referenceline_to_assessmentsec" +
                        "tion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Referentielijn importeren afgebroken. Geen data ingelezen..
        /// </summary>
        internal static string ReferenceLineImporter_ProgressText_Import_cancelled_no_data_read {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ProgressText_Import_cancelled_no_data_read", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen referentielijn..
        /// </summary>
        internal static string ReferenceLineImporter_ProgressText_Reading_referenceline {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ProgressText_Reading_referenceline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verwijderen rekenresultaten en vakindelingen van faalmechanismen..
        /// </summary>
        internal static string ReferenceLineImporter_ProgressText_Removing_calculation_output_and_failure_mechanism_sections {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ProgressText_Removing_calculation_output_and_failure_mechan" +
                        "ism_sections", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verwijderen hydraulische randvoorwaarde uitvoer..
        /// </summary>
        internal static string ReferenceLineImporter_ProgressText_Removing_hydraulic_boundary_output {
            get {
                return ResourceManager.GetString("ReferenceLineImporter_ProgressText_Removing_hydraulic_boundary_output", resourceCulture);
            }
        }
    }
}
