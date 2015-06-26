namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="Node{T}"/> class.
    /// </summary>
    public class NodeTTest
    {
        [Fact( DisplayName = "value should write expected value" )]
        public void ValuePropertyShouldBeReadWrite()
        {
            // arrange
            var target = new Node<string>();
            var expected = "Test";

            // act
            Assert.PropertyChanged( target, "Value", () => target.Value = expected );

            // assert
            Assert.Equal( expected, target.Value );
        }

        [Fact( DisplayName = "depth should return correct value" )]
        public void DepthPropertyShouldReturnCorrectLevel()
        {
            var target = new Node<string>();
            Assert.Equal( 0, target.Depth );

            target.Add( "1" );
            Assert.Equal( 1, target[0].Depth );
            
            target[0].Add( "2" );
            Assert.Equal( 2, target[0][0].Depth );
            
            target[0][0].Add( "3" );
            Assert.Equal( 3, target[0][0][0].Depth );
        }

        [Fact( DisplayName = "parent should return expected object" )]
        public void ParentPropertyShouldReturnCorrectObject()
        {
            var target = new Node<string>();
            Assert.Null( target.Parent );

            target.Add( "1" );
            Assert.Equal( target, target[0].Parent );
            
            target[0].Add( "2" );
            Assert.Equal( target[0], target[0][0].Parent );
            
            target[0][0].Add( "3" );
            Assert.Equal( target[0][0], target[0][0][0].Parent );
        }

        [Fact( DisplayName = "insert should add new value" )]
        public void ShouldInsertNewValue()
        {
            // arrange
            var target = new Node<string>();
            var expected = "Test";
            
            target.AddRange( new[] { "1", "2", "3" } );

            // act
            target.Insert( 1, expected );

            // assert
            Assert.Equal( 4, target.Count );
            Assert.Equal( expected, target[1].Value );
        }

        [Fact( DisplayName = "add should append value" )]
        public void ShouldAddNewValue()
        {
            // arrange
            var target = new Node<string>();
            var expected = "Test";

            // act
            target.Add( expected );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( expected, target[0].Value );
        }

        [Fact( DisplayName = "add range should append values" )]
        public void ShouldAddRangeOfNewValues()
        {
            // arrange
            var target = new Node<string>();
            var expected = new[] { "1", "2", "3" };

            // act
            target.AddRange( expected );

            // assert
            Assert.Equal( expected.Length, target.Count );
            Assert.True( expected.SequenceEqual( target.Select( i => i.Value ) ) );
        }

        [Fact( DisplayName = "remove should remove value" )]
        public void ShouldRemoveValue()
        {
            // arrange
            var target = new Node<string>();

            target.AddRange( new[] { "1", "2", "3" } );

            // act
            Assert.True( target.Remove( "2" ) );

            // assert
            Assert.Equal( 2, target.Count );
            Assert.False( target.Remove( "4" ) );
        }
    }
}
