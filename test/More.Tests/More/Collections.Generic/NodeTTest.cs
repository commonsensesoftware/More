namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    public class NodeTTest
    {
        [Fact]
        public void value_should_write_expected_value()
        {
            // arrange
            var node = new Node<string>();
            var expected = "Test";

            node.MonitorEvents<INotifyPropertyChanged>();

            // act
            node.Value = expected;

            // assert
            node.Value.Should().Be( expected );
            node.ShouldRaisePropertyChangeFor( t => t.Value );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 1 )]
        [InlineData( 2 )]
        [InlineData( 3 )]
        public void depth_should_return_correct_value( int depth )
        {
            // arrange
            var root = new Node<string>();
            var node = root;

            // act
            for ( var i = 0; i < depth; i++ )
            {
                node.Add( ( i + 1 ).ToString() );
                node = node[0];
            }

            // assert
            node.Depth.Should().Be( depth );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 1 )]
        [InlineData( 2 )]
        [InlineData( 3 )]
        public void parent_should_return_expected_object( int depth )
        {
            // arrange
            var node = new Node<string>();
            var parent = default( Node<string> );

            // act
            for ( var i = 0; i < depth; i++ )
            {
                node.Add( ( i + 1 ).ToString() );
                node = node[0];
                parent = node.Parent;
            }

            // assert
            node.Parent.Should().BeSameAs( parent );
        }

        [Fact]
        public void insert_should_add_new_value()
        {
            // arrange
            var node = new Node<string>();
            var expected = "Test";

            node.AddRange( new[] { "1", "2", "3" } );

            // act
            node.Insert( 1, expected );

            // assert
            node.Should().HaveCount( 4 );
            node[1].Value.Should().Be( expected );
        }

        [Fact]
        public void add_should_append_value()
        {
            // arrange
            var node = new Node<string>();
            var expected = "Test";

            // act
            node.Add( expected );

            // assert
            node.Should().HaveCount( 1 );
            node[0].Value.Should().Be( expected );
        }

        [Fact]
        public void add_range_should_append_values()
        {
            // arrange
            var node = new Node<string>();
            var expected = new[] { "1", "2", "3" };

            // act
            node.AddRange( expected );

            // assert
            node.Select( n => n.Value ).Should().Equal( expected );
        }

        [Fact]
        public void remove_should_remove_value()
        {
            // arrange
            var node = new Node<string>();

            node.AddRange( new[] { "1", "2", "3" } );

            // act
            var result = node.Remove( "2" );

            // assert
            result.Should().BeTrue();
            node.Should().HaveCount( 2 );
        }

        [Fact]
        public void remove_should_not_remove_value()
        {
            // arrange
            var node = new Node<string>();

            node.AddRange( new[] { "1", "2", "3" } );

            // act
            var result = node.Remove( "4" );

            // assert
            result.Should().BeFalse();
            node.Should().HaveCount( 3 );
        }
    }
}