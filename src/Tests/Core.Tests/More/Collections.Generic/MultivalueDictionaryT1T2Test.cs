namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="MultivalueDictionary{TKey,TValue}"/>.
    /// </summary>
    public class MultivalueDictionaryT1T2Test
    {
        /// <summary>
        /// Since the values added to the dictionary can produce mutable items, this method is used to test for equivalency.
        /// </summary>
        /// <param name="obj">The item from the NotifyCollectionChangedEventArgs to evaluate.</param>
        /// <param name="key">The key of the item to verify.</param>
        /// <param name="values">The sequence of values to verify.</param>
        /// <returns>True if the item provided in the events arguments is equivalent to the added values; otherwise, false.</returns>
        private static bool AreEquivalent( object obj, string key, params string[] values )
        {
            var item = (KeyValuePair<string, ICollection<string>>) obj;

            return StringComparer.Ordinal.Equals( item.Key, key ) &&
                item.Value != null &&
                item.Value.Count == values.Length &&
                item.Value.SequenceEqual( values, StringComparer.Ordinal );
        }

        [Fact( DisplayName = "add should raise events" )]
        public void AddMethodRaisesEvents()
        {
            var propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            MultivalueDictionary<string, string> dictionary = new MultivalueDictionary<string, string>();
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // adding a new item should raise an event
            dictionary.Add( "key", "value" );
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.Equal( propertyArgs[0].PropertyName, "Count" );
            Assert.Equal( propertyArgs[1].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Add );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.Null( collectionArgs.OldItems );
            Assert.Equal( collectionArgs.OldStartingIndex, -1 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // adding an existing item will not raise an event
            dictionary.Add( "key", "value" );
            Assert.Equal( propertyArgs.Count(), 0 );
            Assert.Null( collectionArgs );

            // appending to an existing item will raise an event
            dictionary.AddRange( "key", new[] { "other value" } );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value", "value", "other value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );
        }

        [Fact( DisplayName = "add range should append items" )]
        public void AddRangeShouldAppendItems()
        {
            const int Key1 = 1;
            const int Key2 = 2;
            const string Value = "test value";
            const string OtherValue = "other test value";

            //Create the initial dictionary
            var dict = new MultivalueDictionary<int, string>();
            dict.Add( Key1, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[Key1].Count, 1 );

            //Add new item
            dict.AddRange( Key2, new[] { Value, Guid.NewGuid().ToString() } );
            Assert.Equal( dict.Count, 2 );
            Assert.Equal( dict[Key1].Count, 1 );
            Assert.Equal( dict[Key2].Count, 2 );

            //Add existing item with new value
            dict.Add( Key1, OtherValue );
            Assert.Equal( dict.Count, 2 );
            Assert.Equal( dict[Key1].Count, 2 );
        }

        [Fact( DisplayName = "add range should filter duplicates" )]
        public void AddRangeFiltersDuplicatesWhenNotAllowed()
        {
            const int Key = 1;
            const string Value = "test value";

            // create the intial test subject
            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( Key, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[Key].Count, 1 );

            // now AddRange with one existing value and one new value
            dict.AddRange( Key, new[] { Value, Guid.NewGuid().ToString() } );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[Key].Count, 2 );
        }

        [Fact( DisplayName = "add range should raise events" )]
        public void AddRangeMethodRaisesEvents()
        {
            IList<PropertyChangedEventArgs> propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            MultivalueDictionary<string, string> dictionary = new MultivalueDictionary<string, string>();
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // adding a new item should raise an event
            dictionary.AddRange( "key", new[] { "value", "other value" } );
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.Equal( propertyArgs[0].PropertyName, "Count" );
            Assert.Equal( propertyArgs[1].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Add );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value", "other value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.Null( collectionArgs.OldItems );
            Assert.Equal( collectionArgs.OldStartingIndex, -1 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // adding an existing item will not raise an event
            dictionary.Add( "key", "value" );
            Assert.Equal( propertyArgs.Count, 0 );
            Assert.Null( collectionArgs );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // appending to an existing item will raise an event
            dictionary.AddRange( "key", new[] { "yet another value" } );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value", "other value", "value", "yet another value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value", "other value", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );
        }

        [Fact( DisplayName = "duplicate items should be scoped to key" )]
        public void DuplicatesAreScopedToKey()
        {
            const string Value = "test value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 1 );

            dict.Add( 1, Guid.NewGuid().ToString() );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 2 );

            dict.Add( 2, Value );
            Assert.Equal( dict.Count, 2 );
            Assert.Equal( dict[2].Count, 1 );
        }

        [Fact( DisplayName = "add should allow duplicate items" )]
        public void CanAddDuplicatesItemsWhenAllowed()
        {
            const string Value = "test value";

            var dict = new MultivalueDictionary<int, string>();
            dict.Add( 1, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 1 );

            dict.Add( 1, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 2 );

            dict.Add( 1, Guid.NewGuid().ToString() );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 3 );

            dict.Add( 1, new[] { Guid.NewGuid().ToString() } );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 4 );

            dict.Add( 1, new[] { Value } );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 5 );

            dict.AddRange( 1, new[] { Guid.NewGuid().ToString() } );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 6 );

            dict.AddRange( 1, new[] { Value } );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 7 );
        }

        [Fact( DisplayName = "remove should remove key with last item" )]
        public void ShouldRemoveKeyWhenLastItemInValueIsRemoved()
        {
            const string Value = "test value";

            var dict = new MultivalueDictionary<int, string>();
            dict.Add( 1, Value );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict[1].Count, 1 );

            dict.Remove( 1, Value );
            Assert.Equal( dict.Count, 0 );
        }

        [Fact( DisplayName = "count values should return expected result" )]
        public void CountValuesShouldReturnCorrectNumber()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>();
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );
            dict.Add( 2, OtherValue );

            Assert.Equal( 2, dict.CountValues( 1 ) );
        }

        [Fact( DisplayName = "count all values should return expected result" )]
        public void CountAllValuesShouldReturnCorrectNumber()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>();
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );
            dict.Add( 2, OtherValue );

            Assert.Equal( dict.CountAllValues(), 3 );
        }

        [Fact( DisplayName = "set should replace values" )]
        public void SetShouldReplaceValues()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );

            Assert.Equal( dict.CountValues( 1 ), 2 );

            dict.Set( 1, Guid.NewGuid().ToString() );
            Assert.Equal( dict.CountValues( 1 ), 1 );
        }

        [Fact( DisplayName = "set should raise events" )]
        public void SetMethodRaisesEvents()
        {
            IList<PropertyChangedEventArgs> propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            MultivalueDictionary<string, string> dictionary = new MultivalueDictionary<string, string>();
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // replacing with a new item should raise an event
            dictionary.Set( "key", "value" );
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Add );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.Null( collectionArgs.OldItems );
            Assert.Equal( collectionArgs.OldStartingIndex, -1 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // replacing an existing item with an existing value should raise an event
            dictionary.Set( "key", "value" );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // replacing an existing item with a new value should raise an event
            dictionary.Set( "key", "other value" );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "other value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );
        }

        [Fact( DisplayName = "set range should replace values" )]
        public void SetRangeShouldReplaceValues()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );

            Assert.Equal( dict.CountValues( 1 ), 2 );

            dict.SetRange( 1, new[] { Guid.NewGuid().ToString() } );
            Assert.Equal( dict.CountValues( 1 ), 1 );
        }

        [Fact( DisplayName = "remove should remove values" )]
        public void RemoveShouldRemoveValues()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            dict.Add( 2, OtherValue );

            Assert.Equal( dict.Count, 2 );
            Assert.Equal( dict.CountValues( 1 ), 1 );
            Assert.Equal( dict.CountValues( 2 ), 1 );

            dict.Remove( 1 );
            Assert.Equal( dict.Count, 1 );
            Assert.Equal( dict.CountValues( 2 ), 1 );

            Assert.False( dict.Remove( 2, Value ) );
            Assert.True( dict.Remove( 2, OtherValue ) );
        }

        [Fact( DisplayName = "remove should raise events" )]
        public void RemoveMethodRaisesEvents()
        {
            IList<PropertyChangedEventArgs> propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            MultivalueDictionary<string, string> dictionary = new MultivalueDictionary<string, string>();
            dictionary.Add( "key", "value1" );
            dictionary.Add( "key", "value2" );
            dictionary.Add( "other key", "value" );
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // removing an nonexisting item should not raise an event
            dictionary.Remove( "made up key" );
            Assert.Equal( propertyArgs.Count, 0 );
            Assert.Null( collectionArgs );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // removing an existing item with a nonexisting value should not raise an event
            dictionary.Remove( "key", "made up value" );
            Assert.Equal( propertyArgs.Count, 0 );
            Assert.Null( collectionArgs );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // removing an entire existing collection by key should raise an event
            dictionary.Remove( "other key" );
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.Equal( propertyArgs[0].PropertyName, "Count" );
            Assert.Equal( propertyArgs[1].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Remove );
            Assert.Null( collectionArgs.NewItems );
            Assert.Equal( collectionArgs.NewStartingIndex, -1 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "other key", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 1 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // removing an existing item with an existing value should raise an event
            dictionary.Remove( "key", "value1" );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value2" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value1", "value2" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // removing an existing item with for the last existing value should raise an event
            dictionary.Remove( "key", "value2" );
            Assert.Equal( propertyArgs.Count, 3 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.Equal( propertyArgs[1].PropertyName, "Count" );
            Assert.Equal( propertyArgs[2].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Remove );
            Assert.Null( collectionArgs.NewItems );
            Assert.Equal( collectionArgs.NewStartingIndex, -1 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", new string[0] ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );
        }

        [Fact( DisplayName = "set range should filter duplicates" )]
        public void SetRangeShouldFilterDuplicatesWhenNotAllowed()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );

            Assert.Equal( dict.CountValues( 1 ), 2 );

            dict.SetRange( 1, new[] { Value, Value } );
            Assert.Equal( dict.CountValues( 1 ), 1 );
            Assert.Equal( dict[1].First(), Value );
        }

        [Fact( DisplayName = "set range should raise events" )]
        public void SetRangeMethodRaisesEvents()
        {
            IList<PropertyChangedEventArgs> propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            MultivalueDictionary<string, string> dictionary = new MultivalueDictionary<string, string>();
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // replacing with a new item should raise an event
            dictionary.SetRange( "key", new[] { "value" } );
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Add );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.Null( collectionArgs.OldItems );
            Assert.Equal( collectionArgs.OldStartingIndex, -1 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // replacing an existing item with an existing value should raise an event
            dictionary.Set( "key", "value" );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // replacing an existing item with a new value should raise an event
            dictionary.SetRange( "key", new[] { "other value" } );
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "key", "other value" ) );
            Assert.Equal( collectionArgs.NewStartingIndex, 0 );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "key", "value" ) );
            Assert.Equal( collectionArgs.OldStartingIndex, 0 );
        }

        [Fact( DisplayName = "contains should return exected result" )]
        public void ContainsShouldReturnCorrectValue()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );
            dict.Add( 1, Value );
            dict.Add( 1, OtherValue );

            Assert.True( dict.Contains( 1, Value ) );
            Assert.False( dict.Contains( 1, Guid.NewGuid().ToString() ) );
        }

        [Fact( DisplayName = "indexer should filter duplicates" )]
        public void IndexerFiltersDuplicatesWhenNotAllowed()
        {
            const string Value = "test value";
            const string OtherValue = "other value";

            var dict = new MultivalueDictionary<int, string>( () => new HashSet<string>() );

            dict[1] = new[] { Value, OtherValue, Value };

            Assert.Equal( dict.CountValues( 1 ), 2 );
        }

        [Fact( DisplayName = "indexer should raise events" )]
        public void SettingIndexerRaisesEvent()
        {
            IList<PropertyChangedEventArgs> propertyArgs = new List<PropertyChangedEventArgs>();
            NotifyCollectionChangedEventArgs collectionArgs = null;
            const string Value = "new value";
            var dictionary = new MultivalueDictionary<string, string>();
            dictionary.Add( "Key", string.Empty );
            dictionary.PropertyChanged += ( sender, e ) => propertyArgs.Add( e );
            dictionary.CollectionChanged += ( sender, e ) => collectionArgs = e;

            // indexing an existing key with a new value should raise events
            dictionary["Key"] = new[] { Value };
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "Key", Value ) );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "Key", string.Empty ) );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // perform same test but as IDictionary
            ( (IDictionary<string, ICollection<string>>) dictionary )["Key"] = new[] { Value };
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "Key", Value ) );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "Key", Value ) );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // indexing an existing key with an existing value should raise events
            dictionary["Key"] = new[] { Value };
            Assert.Equal( propertyArgs.Count, 1 );
            Assert.Equal( propertyArgs[0].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Replace );
            Assert.NotNull( collectionArgs.NewItems );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "Key", Value ) );
            Assert.NotNull( collectionArgs.OldItems );
            Assert.Equal( 1, collectionArgs.OldItems.Count );
            Assert.True( AreEquivalent( collectionArgs.OldItems[0], "Key", Value ) );

            // reset
            propertyArgs.Clear();
            collectionArgs = null;

            // indexing an new key with should raise events
            dictionary["Other Key"] = new[] { Value };
            Assert.Equal( propertyArgs.Count, 2 );
            Assert.Equal( propertyArgs[0].PropertyName, "Count" );
            Assert.Equal( propertyArgs[1].PropertyName, "Item[]" );
            Assert.NotNull( collectionArgs );
            Assert.Equal( collectionArgs.Action, NotifyCollectionChangedAction.Add );
            Assert.Equal( 1, collectionArgs.NewItems.Count );
            Assert.True( AreEquivalent( collectionArgs.NewItems[0], "Other Key", Value ) );
        }
    }
}
