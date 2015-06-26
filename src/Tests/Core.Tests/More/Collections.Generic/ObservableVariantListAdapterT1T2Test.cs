namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObservableVariantListAdapter{TFrom,TTo}"/>.
    /// </summary>
    public partial class ObservableVariantListAdapterT1T2Test
    {
        [Fact( DisplayName = "clear should raise events" )]
        public void ClearShouldRaiseCorrectEvents()
        {
            // arrange
            var target = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expected = new[] { "Count", "Item[]" };
            var actual = new List<string>();
            var action = default ( NotifyCollectionChangedAction );

            target.Add( "test" );
            target.PropertyChanged += ( s, e ) => actual.Add( e.PropertyName );
            target.CollectionChanged += ( s, e ) => action = e.Action;

            // act
            target.Clear();

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
            Assert.Equal( NotifyCollectionChangedAction.Reset, action );
        }

        [Fact( DisplayName = "add should raise events" )]
        public void AddShouldRaiseCorrectEvents()
        {
            // arrange
            var target = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expected = new[] { "Count", "Item[]" };
            var actual = new List<string>();
            NotifyCollectionChangedEventArgs args = null;

            target.PropertyChanged += ( s, e ) => actual.Add( e.PropertyName );
            target.CollectionChanged += ( s, e ) => args = e;

            // act
            target.Add( "test" );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
            Assert.Equal( NotifyCollectionChangedAction.Add, args.Action );
            Assert.Equal( 0, args.NewStartingIndex );
            Assert.NotNull( args.NewItems );
            Assert.Equal( 1, args.NewItems.Count );
            Assert.Equal( "test", args.NewItems[0] );
        }

        [Fact( DisplayName = "remove should raise events" )]
        public void RemoveShouldRaiseCorrectEvents()
        {
            // arrange
            var target = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expected = new[] { "Count", "Item[]" };
            var actual = new List<string>();
            NotifyCollectionChangedEventArgs args = null;

            target.Add( "test" );
            target.PropertyChanged += ( s, e ) => actual.Add( e.PropertyName );
            target.CollectionChanged += ( s, e ) => args = e;

            // act
            target.Remove( "test" );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
            Assert.Equal( NotifyCollectionChangedAction.Remove, args.Action );
            Assert.Equal( 0, args.OldStartingIndex );
            Assert.NotNull( args.OldItems );
            Assert.Equal( 1, args.OldItems.Count );
            Assert.Equal( "test", args.OldItems[0] );
        }

        [Fact( DisplayName = "indexer should raise events" )]
        public void IndexerShouldRaiseCorrectEvents()
        {
            // arrange
            var target = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expected = "Item[]";
            string actual = null;
            NotifyCollectionChangedEventArgs args = null;

            target.Add( "test" );
            target.PropertyChanged += ( s, e ) => actual = e.PropertyName;
            target.CollectionChanged += ( s, e ) => args = e;

            // act
            target[0] = "other test";

            // arrange
            Assert.Equal( expected, actual );
            Assert.Equal( NotifyCollectionChangedAction.Replace, args.Action );
            Assert.Equal( 0, args.NewStartingIndex );
            Assert.Equal( 0, args.OldStartingIndex );
            Assert.NotNull( args.NewItems );
            Assert.NotNull( args.OldItems );
            Assert.Equal( 1, args.NewItems.Count );
            Assert.Equal( 1, args.OldItems.Count );
            Assert.Equal( "test", args.OldItems[0] );
            Assert.Equal( "other test", args.NewItems[0] );
        }

        [Fact( DisplayName = "adapter should bubble events from source" )]
        public void AdapterShouldBubbleEventsFromSource()
        {
            // arrange
            var collection = new ObservableCollection<string>();
            var target = new ObservableVariantListAdapter<string, object>( collection );
            var expected = new[] { "Count", "Item[]" };
            var actual = new List<string>();
            NotifyCollectionChangedEventArgs args = null;

            target.PropertyChanged += ( s, e ) => actual.Add( e.PropertyName );
            target.CollectionChanged += ( s, e ) => args = e;

            // act
            collection.Add( "test" );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
            Assert.Equal( NotifyCollectionChangedAction.Add, args.Action );
            Assert.Equal( 0, args.NewStartingIndex );
            Assert.NotNull( args.NewItems );
            Assert.Equal( 1, args.NewItems.Count );
            Assert.Equal( "test", args.NewItems[0] );
        }
    }
}
