﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Spotkick.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Spotkick.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;.
        /// </summary>
        internal static string DbConnectionString {
            get {
                return ResourceManager.GetString("DbConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string SongkickApiKey {
            get {
                return ResourceManager.GetString("SongkickApiKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://api.songkick.com.
        /// </summary>
        internal static string SongkickApiUrl {
            get {
                return ResourceManager.GetString("SongkickApiUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://accounts.spotify.com/api.
        /// </summary>
        internal static string SpotifyAccountsApiUrl {
            get {
                return ResourceManager.GetString("SpotifyAccountsApiUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://api.spotify.com.
        /// </summary>
        internal static string SpotifyApiUrl {
            get {
                return ResourceManager.GetString("SpotifyApiUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://accounts.spotify.com/authorize.
        /// </summary>
        internal static string SpotifyAuthorizeUrl {
            get {
                return ResourceManager.GetString("SpotifyAuthorizeUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string SpotifyClientId {
            get {
                return ResourceManager.GetString("SpotifyClientId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string SpotifyClientSecret {
            get {
                return ResourceManager.GetString("SpotifyClientSecret", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://localhost:6254/Spotkick/Callback.
        /// </summary>
        internal static string SpotifyRedirectUrl {
            get {
                return ResourceManager.GetString("SpotifyRedirectUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to user-read-email user-follow-read playlist-modify-private.
        /// </summary>
        internal static string SpotifyScope {
            get {
                return ResourceManager.GetString("SpotifyScope", resourceCulture);
            }
        }
    }
}