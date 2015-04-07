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

    internal sealed class DataPackagePropertySetViewAdapter : IDataPackagePropertySetView
    {
        private readonly DataPackagePropertySetView adapted;

        internal DataPackagePropertySetViewAdapter( DataPackagePropertySetView propertySetView )
        {
            Contract.Requires( propertySetView != null );
            this.adapted = propertySetView;
        }

        public Uri ApplicationListingUri
        {
            get
            {
                return this.adapted.ApplicationListingUri;
            }
        }

        public string ApplicationName
        {
            get
            {
                return this.adapted.ApplicationName;
            }
        }

        public Uri ContentSourceApplicationLink
        {
            get
            {
                return this.adapted.ContentSourceApplicationLink;
            }
        }

        public Uri ContentSourceWebLink
        {
            get
            {
                return this.adapted.ContentSourceWebLink;
            }
        }

        public string Description
        {
            get
            {
                return this.adapted.Description;
            }
        }

        public IReadOnlyList<string> FileTypes
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
        }

        public string PackageFamilyName
        {
            get
            {
                return this.adapted.PackageFamilyName;
            }
        }

        public IRandomAccessStreamReference Square30x30Logo
        {
            get
            {
                return this.adapted.Square30x30Logo;
            }
        }

        public IRandomAccessStreamReference Thumbnail
        {
            get
            {
                return this.adapted.Thumbnail;
            }
        }

        public string Title
        {
            get
            {
                return this.adapted.Title;
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
    }
}
