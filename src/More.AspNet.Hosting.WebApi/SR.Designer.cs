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
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("More.SR", typeof(SR).Assembly);
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
        ///   Looks up a localized string similar to Begin commit changes..
        /// </summary>
        internal static string CommitBegin {
            get {
                return ResourceManager.GetString("CommitBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End commit changes..
        /// </summary>
        internal static string CommitEnd {
            get {
                return ResourceManager.GetString("CommitEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Begin query operation..
        /// </summary>
        internal static string RepositoryGetAsyncBegin {
            get {
                return ResourceManager.GetString("RepositoryGetAsyncBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End query operation..
        /// </summary>
        internal static string RepositoryGetAsyncEnd {
            get {
                return ResourceManager.GetString("RepositoryGetAsyncEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Begin save changes..
        /// </summary>
        internal static string RepositorySaveChangesAsyncBegin {
            get {
                return ResourceManager.GetString("RepositorySaveChangesAsyncBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End save changes..
        /// </summary>
        internal static string RepositorySaveChangesAsyncEnd {
            get {
                return ResourceManager.GetString("RepositorySaveChangesAsyncEnd", resourceCulture);
            }
        }
    }
}