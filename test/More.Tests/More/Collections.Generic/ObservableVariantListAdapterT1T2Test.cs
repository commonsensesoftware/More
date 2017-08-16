namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Xunit;
    using static System.Collections.Specialized.NotifyCollectionChangedAction;

    public class ObservableVariantListAdapterT1T2Test
    {
        [Fact]
        public void clear_should_raise_events()
        {
            // arrange
            var list = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var action = default( NotifyCollectionChangedAction );

            list.Add( "test" );
            list.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            list.CollectionChanged += ( s, e ) => action = e.Action;

            // act
            list.Clear();

            // assert
            actualProperties.Should().Equal( expectedProperties );
            action.Should().Be( Reset );
        }

        [Fact]
        public void add_should_raise_events()
        {
            // arrange
            var list = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var args = default( NotifyCollectionChangedEventArgs );

            list.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            list.CollectionChanged += ( s, e ) => args = e;

            // act
            list.Add( "test" );

            // assert
            actualProperties.Should().Equal( expectedProperties );
            args.ShouldBeEquivalentTo(
                new
                {
                    Action = Add,
                    NewStartingIndex = 0,
                    OldStartingIndex = -1,
                    OldItems = default( IList ),
                    NewItems = new ArrayList() { "test" }
                } );
        }

        [Fact]
        public void remove_should_raise_events()
        {
            // arrange
            var list = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var args = default( NotifyCollectionChangedEventArgs );

            list.Add( "test" );
            list.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            list.CollectionChanged += ( s, e ) => args = e;

            // act
            list.Remove( "test" );

            // assert
            actualProperties.Should().Equal( expectedProperties );
            args.ShouldBeEquivalentTo(
                new
                {
                    Action = Remove,
                    OldStartingIndex = 0,
                    OldItems = new ArrayList() { "test" },
                    NewStartingIndex = -1,
                    NewItems = default( IList )
                } );
        }

        [Fact]
        public void indexer_should_raise_events()
        {
            // arrange
            var list = new ObservableVariantListAdapter<string, object>( new List<string>() );
            var expectedProperty = "Item[]";
            var actualProperty = default( string );
            var args = default( NotifyCollectionChangedEventArgs );

            list.Add( "test" );
            list.PropertyChanged += ( s, e ) => actualProperty = e.PropertyName;
            list.CollectionChanged += ( s, e ) => args = e;

            // act
            list[0] = "other test";

            // arrange
            actualProperty.Should().Be( expectedProperty );
            args.ShouldBeEquivalentTo(
                new
                {
                    Action = Replace,
                    OldStartingIndex = 0,
                    OldItems = new ArrayList() { "test" },
                    NewStartingIndex = 0,
                    NewItems = new ArrayList() { "other test" }
                } );
        }

        [Fact]
        public void adapter_should_bubble_events_from_source()
        {
            // arrange
            var collection = new ObservableCollection<string>();
            var list = new ObservableVariantListAdapter<string, object>( collection );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var args = default( NotifyCollectionChangedEventArgs );

            list.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            list.CollectionChanged += ( s, e ) => args = e;

            // act
            collection.Add( "test" );

            // assert
            actualProperties.Should().Equal( expectedProperties );
            args.ShouldBeEquivalentTo(
                new
                {
                    Action = Add,
                    OldStartingIndex = -1,
                    OldItems = default( IList ),
                    NewStartingIndex = 0,
                    NewItems = new ArrayList() { "test" }
                } );
        }
    }
}