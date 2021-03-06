﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace More {
    using System;
    using System.Reflection;
    
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
    internal class ExceptionMessage {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessage() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("More.ExceptionMessage", typeof(ExceptionMessage).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to The activity &apos;{0}&apos; has already been registered and cannot be registered more than once..
        /// </summary>
        internal static string ActivityCannotBeRegisteredMoreThanOnce {
            get {
                return ResourceManager.GetString("ActivityCannotBeRegisteredMoreThanOnce", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type name &apos;{0}&apos; is ambiguous..
        /// </summary>
        internal static string AmbiguousTypeName {
            get {
                return ResourceManager.GetString("AmbiguousTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unspecified exception occurred..
        /// </summary>
        internal static string HostException {
            get {
                return ResourceManager.GetString("HostException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified activity identifier is invalid. The value must be a globally unique identifier (GUID)..
        /// </summary>
        internal static string InvalidActivityAttributeId {
            get {
                return ResourceManager.GetString("InvalidActivityAttributeId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; is not a valid shell view because it does not implement the &apos;{1}&apos; interface..
        /// </summary>
        internal static string InvalidShellView {
            get {
                return ResourceManager.GetString("InvalidShellView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The activity &apos;{0}&apos; of type &apos;{1}&apos; indicates that it depends on an activity of type &apos;{2}&apos;, but a matching activity could not be found..
        /// </summary>
        internal static string MissingDependentActivity {
            get {
                return ResourceManager.GetString("MissingDependentActivity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The host was configured to execute an activity of type &apos;{0}&apos;, but the activity could not be resolved..
        /// </summary>
        internal static string MissingStartupActivity {
            get {
                return ResourceManager.GetString("MissingStartupActivity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple matches for activity &apos;{0}&apos; were found.  Each activity must have exactly one export..
        /// </summary>
        internal static string MultipleStartupActivitiesExported {
            get {
                return ResourceManager.GetString("MultipleStartupActivitiesExported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The host was unable to find a shell view that matches the type &apos;{0}&apos;..
        /// </summary>
        internal static string NoCandidateShellView {
            get {
                return ResourceManager.GetString("NoCandidateShellView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred loading the shell view.  See the inner exception for details..
        /// </summary>
        internal static string ShellViewLoadException {
            get {
                return ResourceManager.GetString("ShellViewLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The host was unable to create a shell view of type &apos;{0}&apos;..
        /// </summary>
        internal static string ShellViewResolutionFailed {
            get {
                return ResourceManager.GetString("ShellViewResolutionFailed", resourceCulture);
            }
        }
    }
}
