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

    sealed class DataPackagePropertySetViewAdapter : IDataPackagePropertySetView
    {
        readonly DataPackagePropertySetView adapted;

        internal DataPackagePropertySetViewAdapter( DataPackagePropertySetView propertySetView ) =>            adapted = propertySetView;

        public Uri ApplicationListingUri => adapted.ApplicationListingUri;

        public string ApplicationName => adapted.ApplicationName;

        public Uri ContentSourceApplicationLink => adapted.ContentSourceApplicationLink;

        public Uri ContentSourceWebLink => adapted.ContentSourceWebLink;

        public string Description => adapted.Description;

        public IReadOnlyList<string> FileTypes => adapted.FileTypes;

        public Color LogoBackgroundColor => adapted.LogoBackgroundColor;

        public string PackageFamilyName => adapted.PackageFamilyName;

        public IRandomAccessStreamReference Square30x30Logo => adapted.Square30x30Logo;

        public IRandomAccessStreamReference Thumbnail => adapted.Thumbnail;

        public string Title => adapted.Title;

        public bool ContainsKey( string key ) => adapted.ContainsKey( key );

        public IEnumerable<string> Keys => adapted.Keys;

        public bool TryGetValue( string key, out object value ) => adapted.TryGetValue( key, out value );

        public IEnumerable<object> Values => adapted.Values;

        public object this[string key] => adapted[key];

        public int Count => adapted.Count;

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