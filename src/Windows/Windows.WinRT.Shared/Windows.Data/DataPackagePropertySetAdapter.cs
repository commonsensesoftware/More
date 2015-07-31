namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation.Metadata;
    using global::Windows.Storage.Streams;
    using global::Windows.UI;

    internal sealed class DataPackagePropertySetAdapter : IDataPackagePropertySet
    {
        private readonly DataPackagePropertySet adapted;

        internal DataPackagePropertySetAdapter( DataPackagePropertySet propertySet )
        {
            Contract.Requires( propertySet != null );
            adapted = propertySet;
        }

        public Uri ApplicationListingUri
        {
            get
            {
                return adapted.ApplicationListingUri;
            }
            set
            {
                adapted.ApplicationListingUri = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                return adapted.ApplicationName;
            }
            set
            {
                adapted.ApplicationName = value;
            }
        }

        public Uri ContentSourceApplicationLink
        {
            get
            {
                return adapted.ContentSourceApplicationLink;
            }
            set
            {
                adapted.ContentSourceApplicationLink = value;
            }
        }

        public Uri ContentSourceWebLink
        {
            get
            {
                return adapted.ContentSourceWebLink;
            }
            set
            {
                adapted.ContentSourceWebLink = value;
            }
        }

        public string Description
        {
            get
            {
                return adapted.Description;
            }
            set
            {
                adapted.Description = value;
            }
        }

        public IList<string> FileTypes
        {
            get
            {
                return adapted.FileTypes;
            }
        }

        public Color LogoBackgroundColor
        {
            get
            {
                return adapted.LogoBackgroundColor;
            }
            set
            {
                adapted.LogoBackgroundColor = value;
            }
        }

        public string PackageFamilyName
        {
            get
            {
                return adapted.PackageFamilyName;
            }
            set
            {
                adapted.PackageFamilyName = value;
            }
        }

        public IRandomAccessStreamReference Square30x30Logo
        {
            get
            {
                return adapted.Square30x30Logo;
            }
            set
            {
                adapted.Square30x30Logo = value;
            }
        }

        public IRandomAccessStreamReference Thumbnail
        {
            get
            {
                return adapted.Thumbnail;
            }
            set
            {
                adapted.Thumbnail = value;
            }
        }

        public string Title
        {
            get
            {
                return adapted.Title;
            }
            set
            {
                adapted.Title = value;
            }
        }

        public bool ContainsKey( string key )
        {
            return adapted.ContainsKey( key );
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement IDictionary<K,V>." )]
        public IEnumerable<string> Keys
        {
            get
            {
                return adapted.Keys;
            }
        }

        public bool TryGetValue( string key, out object value )
        {
            return adapted.TryGetValue( key, out value );
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement IDictionary<K,V>." )]
        public IEnumerable<object> Values
        {
            get
            {
                return adapted.Values;
            }
        }

        public object this[string key]
        {
            get
            {
                return adapted[key];
            }
            set
            {
                adapted[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return adapted.Count;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, object>> @this = adapted;
            return @this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, object>> @this = adapted;
            return @this.GetEnumerator();
        }

        public void Add( string key, object value )
        {
            adapted.Add( key, value );
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return adapted.Keys;
            }
        }

        public bool Remove( string key )
        {
            return adapted.Remove( key );
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return adapted.Values;
            }
        }

        public void Add( KeyValuePair<string, object> item )
        {
            adapted.Add( item );
        }

        public void Clear()
        {
            adapted.Clear();
        }

        public bool Contains( KeyValuePair<string, object> item )
        {
            return adapted.Contains( item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by adapted object." )]
        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            adapted.CopyTo( array, arrayIndex );
        }

        public bool IsReadOnly
        {
            get
            {
                return adapted.IsReadOnly;
            }
        }

        public bool Remove( KeyValuePair<string, object> item )
        {
            return adapted.Remove( item );
        }
    }
}
