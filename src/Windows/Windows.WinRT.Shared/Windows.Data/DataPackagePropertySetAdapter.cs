namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
            this.adapted = propertySet;
        }

        public Uri ApplicationListingUri
        {
            get
            {
                return this.adapted.ApplicationListingUri;
            }
            set
            {
                this.adapted.ApplicationListingUri = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                return this.adapted.ApplicationName;
            }
            set
            {
                this.adapted.ApplicationName = value;
            }
        }

        public Uri ContentSourceApplicationLink
        {
            get
            {
                return this.adapted.ContentSourceApplicationLink;
            }
            set
            {
                this.adapted.ContentSourceApplicationLink = value;
            }
        }

        public Uri ContentSourceWebLink
        {
            get
            {
                return this.adapted.ContentSourceWebLink;
            }
            set
            {
                this.adapted.ContentSourceWebLink = value;
            }
        }

        public string Description
        {
            get
            {
                return this.adapted.Description;
            }
            set
            {
                this.adapted.Description = value;
            }
        }

        public IList<string> FileTypes
        {
            get
            {
                return this.adapted.FileTypes;
            }
        }

        public Color LogoBackgroundColor
        {
            get
            {
                return this.adapted.LogoBackgroundColor;
            }
            set
            {
                this.adapted.LogoBackgroundColor = value;
            }
        }

        public string PackageFamilyName
        {
            get
            {
                return this.adapted.PackageFamilyName;
            }
            set
            {
                this.adapted.PackageFamilyName = value;
            }
        }

        public IRandomAccessStreamReference Square30x30Logo
        {
            get
            {
                return this.adapted.Square30x30Logo;
            }
            set
            {
                this.adapted.Square30x30Logo = value;
            }
        }

        public IRandomAccessStreamReference Thumbnail
        {
            get
            {
                return this.adapted.Thumbnail;
            }
            set
            {
                this.adapted.Thumbnail = value;
            }
        }

        public string Title
        {
            get
            {
                return this.adapted.Title;
            }
            set
            {
                this.adapted.Title = value;
            }
        }

        public bool ContainsKey( string key )
        {
            return this.adapted.ContainsKey( key );
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this.adapted.Keys;
            }
        }

        public bool TryGetValue( string key, out object value )
        {
            return this.adapted.TryGetValue( key, out value );
        }

        public IEnumerable<object> Values
        {
            get
            {
                return this.adapted.Values;
            }
        }

        public object this[string key]
        {
            get
            {
                return this.adapted[key];
            }
            set
            {
                this.adapted[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.adapted.Count;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, object>> @this = this.adapted;
            return @this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, object>> @this = this.adapted;
            return @this.GetEnumerator();
        }

        public void Add( string key, object value )
        {
            this.adapted.Add( key, value );
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return this.adapted.Keys;
            }
        }

        public bool Remove( string key )
        {
            return this.adapted.Remove( key );
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return this.adapted.Values;
            }
        }

        public void Add( KeyValuePair<string, object> item )
        {
            this.adapted.Add( item );
        }

        public void Clear()
        {
            this.adapted.Clear();
        }

        public bool Contains( KeyValuePair<string, object> item )
        {
            return this.adapted.Contains( item );
        }

        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            this.adapted.CopyTo( array, arrayIndex );
        }

        public bool IsReadOnly
        {
            get
            {
                return this.adapted.IsReadOnly;
            }
        }

        public bool Remove( KeyValuePair<string, object> item )
        {
            return this.adapted.Remove( item );
        }
    }
}
