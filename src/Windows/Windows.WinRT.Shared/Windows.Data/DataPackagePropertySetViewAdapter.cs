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
            adapted = propertySetView;
        }

        public Uri ApplicationListingUri
        {
            get
            {
                return adapted.ApplicationListingUri;
            }
        }

        public string ApplicationName
        {
            get
            {
                return adapted.ApplicationName;
            }
        }

        public Uri ContentSourceApplicationLink
        {
            get
            {
                return adapted.ContentSourceApplicationLink;
            }
        }

        public Uri ContentSourceWebLink
        {
            get
            {
                return adapted.ContentSourceWebLink;
            }
        }

        public string Description
        {
            get
            {
                return adapted.Description;
            }
        }

        public IReadOnlyList<string> FileTypes
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
        }

        public string PackageFamilyName
        {
            get
            {
                return adapted.PackageFamilyName;
            }
        }

        public IRandomAccessStreamReference Square30x30Logo
        {
            get
            {
                return adapted.Square30x30Logo;
            }
        }

        public IRandomAccessStreamReference Thumbnail
        {
            get
            {
                return adapted.Thumbnail;
            }
        }

        public string Title
        {
            get
            {
                return adapted.Title;
            }
        }

        public bool ContainsKey( string key )
        {
            return adapted.ContainsKey( key );
        }

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
    }
}
