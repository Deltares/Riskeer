﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Common.IO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Common.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Het bestand heeft een of meerdere multi-polylijnen, welke niet ondersteund worden..
        /// </summary>
        internal static string FailureMechanismSectionReader_File_has_unsupported_multiPolyline {
            get {
                return ResourceManager.GetString("FailureMechanismSectionReader_File_has_unsupported_multiPolyline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand heeft geen attribuut &apos;{0}&apos; welke vereist is om een vakindeling te importeren..
        /// </summary>
        internal static string FailureMechanismSectionReader_File_lacks_required_Attribute_0_ {
            get {
                return ResourceManager.GetString("FailureMechanismSectionReader_File_lacks_required_Attribute_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestand mag uitsluitend polylijnen bevatten..
        /// </summary>
        internal static string FailureMechanismSectionReader_OpenPolyLineShapeFile_File_can_only_have_polylines {
            get {
                return ResourceManager.GetString("FailureMechanismSectionReader_OpenPolyLineShapeFile_File_can_only_have_polylines", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestand bevat 1 multi-polylijn, welke niet ondersteund is..
        /// </summary>
        internal static string ReferenceLineReader_File_contains_unsupported_multi_polyline {
            get {
                return ResourceManager.GetString("ReferenceLineReader_File_contains_unsupported_multi_polyline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestand moet exact 1 gehele polylijn bevatten..
        /// </summary>
        internal static string ReferenceLineReader_File_must_contain_1_polyline {
            get {
                return ResourceManager.GetString("ReferenceLineReader_File_must_contain_1_polyline", resourceCulture);
            }
        }
    }
}
