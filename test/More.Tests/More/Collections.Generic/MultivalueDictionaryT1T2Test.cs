namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;
    using static System.Collections.Specialized.NotifyCollectionChangedAction;

    public class MultivalueDictionaryT1T2Test
    {
        [Fact]
        public void add_should_raise_insert_events()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>();

            dictionary.MonitorEvents();

            // act
            dictionary.Add( "key", "value" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Add )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems == null );
        }

        [Fact]
        public void add_existing_value_should_not_raise_insert_events()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>( () => new HashSet<string>() ) { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary.Add( "key", "value" );

            // assert
            dictionary.ShouldNotRaise( "PropertyChanged" );
            dictionary.ShouldNotRaise( "CollectionChanged" );
        }

        [Fact]
        public void add_should_raise_insert_events_after_append()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>() { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary.Add( "key", "other value" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void add_range_should_append_items()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>();

            // act
            dictionary.AddRange( 1, new[] { "one", "two" } );

            // assert
            dictionary[1].Should().Equal( new[] { "one", "two" } );
        }

        [Theory]
        [InlineData( new[] { "test" }, new[] { "test" } )]
        [InlineData( new[] { "test", "other test" }, new[] { "test", "other test" } )]
        public void add_range_should_filter_duplicates( string[] values, string[] expected )
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() ) { { 1, "test" } };

            // act
            dictionary.AddRange( 1, values );

            // assert
            dictionary[1].Should().Equal( expected );
        }

        [Fact]
        public void add_range_should_raise_insert_events()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>();

            dictionary.MonitorEvents();

            // act
            dictionary.AddRange( "key", new[] { "value", "other value" } );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Add )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems == null );
        }

        [Fact]
        public void add_range_should_raise_insert_events_after_append()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>() { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary.AddRange( "key", new[] { "other value" } );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void duplicate_items_should_be_scoped_to_key()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 2, "test" }
            };

            // act
            dictionary.Add( 1, "test" );
            dictionary.Add( 1, "other test" );
            dictionary.Add( 2, "test" );

            // assert
            dictionary[1].Should().Equal( new[] { "test", "other test" } );
            dictionary[2].Should().Equal( new[] { "test" } );
        }

        [Fact]
        public void add_should_allow_duplicate_items()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>() { { 1, "value" } };

            // act
            dictionary.Add( 1, "value" );

            // assert
            dictionary[1].Should().Equal( new[] { "value", "value" } );
        }

        [Fact]
        public void add_range_should_allow_duplicate_items()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>() { { 1, "value" } };

            // act
            dictionary.AddRange( 1, new[] { "value", "value" } );

            // assert
            dictionary[1].Should().Equal( new[] { "value", "value", "value" } );
        }

        [Fact]
        public void remove_should_remove_key_with_last_item()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>() { { 1, "test" } };

            // act
            dictionary.Remove( 1, "test" );

            // assert
            dictionary.Should().BeEmpty();
        }

        [Fact]
        public void count_values_should_return_expected_result()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>()
            {
                { 1, "test" },
                { 1, "other test" },
                { 2, "test" }
            };

            // act
            var count = dictionary.CountValues( 1 );

            // assert
            count.Should().Be( 2 );
        }

        [Fact]
        public void count_all_values_should_return_expected_result()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>()
            {
                { 1, "test" },
                { 1, "other test" },
                { 2, "test" }
            };

            // act
            var count = dictionary.CountAllValues();

            // assert
            count.Should().Be( 3 );
        }

        [Fact]
        public void set_should_replace_values()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 1, "other test" },
            };

            // act
            dictionary.Set( 1, "another test" );

            // assert
            dictionary[1].Should().Equal( new[] { "another test" } );
        }

        [Fact]
        public void set_should_raise_events_when_item_is_added()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>();

            dictionary.MonitorEvents();

            // act
            dictionary.Set( "key", "value" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Add )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems == null );
        }

        [Theory]
        [InlineData( "value" )]
        [InlineData( "other value" )]
        public void set_should_raise_events_when_new_or_existing_item_is_replaced( string newValue )
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>() { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary.Set( "key", newValue );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void set_range_should_replace_values()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 1, "other test" }
            };

            // act
            dictionary.SetRange( 1, new[] { "another test" } );

            // assert
            dictionary[1].Should().Equal( new[] { "another test" } );
        }

        [Fact]
        public void remove_should_remove_key()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 2, "other test" }
            };

            // act
            var removed = dictionary.Remove( 1 );

            // assert
            removed.Should().BeTrue();
            dictionary.Should().HaveCount( 1 );
        }

        [Theory]
        [InlineData( "test", 1, true )]
        [InlineData( "other test", 2, false )]
        public void remove_should_remove_values( string value, int count, bool expected )
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 2, "other test" }
            };

            // act
            var removed = dictionary.Remove( 1, value );

            // assert
            removed.Should().Be( expected );
            dictionary.Should().HaveCount( count );
        }

        [Fact]
        public void remove_should_not_raise_event_when_key_does_not_exist()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>()
            {
                { "key", "value" },
                { "key", "other value" },
                { "other key", "value" }
            };

            dictionary.MonitorEvents();

            // act
            dictionary.Remove( "made up key" );

            // assert
            dictionary.ShouldNotRaise( "PropertyChanged" );
            dictionary.ShouldNotRaise( "CollectionChanged" );
        }

        [Fact]
        public void remove_should_not_raise_event_when_nonexistent_value_is_removed_from_key()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>()
            {
                { "key", "value" },
                { "key", "other value" },
                { "other key", "value" }
            };

            dictionary.MonitorEvents();

            // act
            dictionary.Remove( "key", "made up key" );

            // assert
            dictionary.ShouldNotRaise( "PropertyChanged" );
            dictionary.ShouldNotRaise( "CollectionChanged" );
        }

        [Fact]
        public void remove_should_raise_event_when_key_is_removed()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>()
            {
                { "key", "value" },
                { "key", "other value" },
                { "other key", "value" }
            };

            dictionary.MonitorEvents();

            // act
            dictionary.Remove( "key" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Remove )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems == null )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void remove_should_raise_event_when_value_is_removed_from_key()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>()
            {
                { "key", "value" },
                { "key", "other value" },
                { "other key", "value" }
            };

            dictionary.MonitorEvents();

            // act
            dictionary.Remove( "key", "value" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void remove_should_raise_event_when_last_value_is_removed_from_key()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>()
            {
                { "key", "value" },
                { "other key", "value" }
            };

            dictionary.MonitorEvents();

            // act
            dictionary.Remove( "key", "value" );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Remove )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems == null )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Fact]
        public void set_range_should_filter_duplicates()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 1, "other test" },
            };

            // act
            dictionary.SetRange( 1, new[] { "test", "test" } );

            // assert
            dictionary[1].Should().Equal( new[] { "test" } );
        }

        [Fact]
        public void set_range_should_raise_events_when_item_is_added()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>();

            dictionary.MonitorEvents();

            // act
            dictionary.SetRange( "key", new[] { "value" } );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Add )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems == null );
        }

        [Theory]
        [InlineData( "value" )]
        [InlineData( "other value" )]
        public void set_range_should_raise_events_when_item_is_replaced( string value )
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>() { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary.SetRange( "key", new[] { value } );

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }

        [Theory]
        [InlineData( "test", true )]
        [InlineData( "blah", false )]
        public void contains_should_return_exected_result( string value, bool expected )
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() )
            {
                { 1, "test" },
                { 1, "other test" },
            };

            // act
            var contains = dictionary.Contains( 1, value );

            // assert
            contains.Should().Be( expected );
        }

        [Fact]
        public void indexer_should_filter_duplicates()
        {
            // arrange
            var dictionary = new MultivalueDictionary<int, string>( () => new HashSet<string>() );

            // act
            dictionary[1] = new[] { "test", "other test", "test" };

            // assert
            dictionary[1].Should().Equal( new[] { "test", "other test" } );
        }

        [Fact]
        public void indexer_should_raise_events_when_item_is_added()
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>();

            dictionary.MonitorEvents();

            // act
            dictionary["key"] = new[] { "value" };

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Count" );
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Add )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == -1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems == null );
        }

        [Theory]
        [InlineData( "value" )]
        [InlineData( "other value" )]
        public void indexer_should_raise_events_when_item_is_replaced( string value )
        {
            // arrange
            var dictionary = new MultivalueDictionary<string, string>() { { "key", "value" } };

            dictionary.MonitorEvents();

            // act
            dictionary["key"] = new[] { value };

            // assert
            dictionary.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            dictionary.ShouldRaise( "CollectionChanged" )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.Action == Replace )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.NewItems.Count == 1 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldStartingIndex == 0 )
                      .WithArgs<NotifyCollectionChangedEventArgs>( e => e.OldItems.Count == 1 );
        }
    }
}