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

    sealed class DataPackagePropertySetAdapter : IDataPackagePropertySet
    {
        readonly DataPackagePropertySet adapted;

        internal DataPackagePropertySetAdapter( DataPackagePropertySet propertySet ) => adapted = propertySet;

        public Uri ApplicationListingUri
        {
            get => adapted.ApplicationListingUri;
            set => adapted.ApplicationListingUri = value;
        }

        public string ApplicationName
        {
            get => adapted.ApplicationName;
            set => adapted.ApplicationName = value;
        }

        public Uri ContentSourceApplicationLink
        {
            get => adapted.ContentSourceApplicationLink;
            set => adapted.ContentSourceApplicationLink = value;
        }

        public Uri ContentSourceWebLink
        {
            get => adapted.ContentSourceWebLink;
            set => adapted.ContentSourceWebLink = value;
        }

        public string Description
        {
            get => adapted.Description;
            set => adapted.Description = value;
        }

        public IList<string> FileTypes => adapted.FileTypes;

        public Color LogoBackgroundColor
        {
            get => adapted.LogoBackgroundColor;
            set => adapted.LogoBackgroundColor = value;
        }

        public string PackageFamilyName
        {
            get => adapted.PackageFamilyName;
            set => adapted.PackageFamilyName = value;
        }

        public IRandomAccessStreamReference Square30x30Logo
        {
            get => adapted.Square30x30Logo;
            set => adapted.Square30x30Logo = value;
        }

        public IRandomAccessStreamReference Thumbnail
        {
            get => adapted.Thumbnail;
            set => adapted.Thumbnail = value;
        }

        public string Title
        {
            get => adapted.Title;
            set => adapted.Title = value;
        }

        public bool ContainsKey( string key ) => adapted.ContainsKey( key );

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement IDictionary<K,V>." )]
        public IEnumerable<string> Keys => adapted.Keys;

        public bool TryGetValue( string key, out object value ) => adapted.TryGetValue( key, out value );

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement IDictionary<K,V>." )]
        public IEnumerable<object> Values => adapted.Values;

        public object this[string key]
        {
            get => adapted[key];
            set => adapted[key] = value;
        }

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

        public void Add( string key, object value ) => adapted.Add( key, value );

        ICollection<string> IDictionary<string, object>.Keys => adapted.Keys;

        public bool Remove( string key ) => adapted.Remove( key );

        ICollection<object> IDictionary<string, object>.Values => adapted.Values;

        public void Add( KeyValuePair<string, object> item ) => adapted.Add( item );

        public void Clear() => adapted.Clear();

        public bool Contains( KeyValuePair<string, object> item ) => adapted.Contains( item );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by adapted object." )]
        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex ) => adapted.CopyTo( array, arrayIndex );

        public bool IsReadOnly => adapted.IsReadOnly;

        public bool Remove( KeyValuePair<string, object> item ) => adapted.Remove( item );
    }
}