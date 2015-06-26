namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ActivityDescriptor"/>.
    /// </summary>
    public class ActivityDescriptorTest
    {
        [Fact( DisplayName = "new activity descriptor should set default properties" )]
        public void ConstructorShouldSetDefaultProperties()
        {
            // arrange
            var name = "Activity";
            var desc = "";

            // act
            var descriptor = new ActivityDescriptor();

            // assert
            Assert.True( ActivityDescriptor.IsValidIdentifier( descriptor.Id ) );
            Assert.Equal( name, descriptor.Name );
            Assert.Equal( desc, descriptor.Description );
        }
        [Fact( DisplayName = "new activity descriptor should set expected properties" )]
        public void ConstructorShouldSetExpectedProperties()
        {
            // arrange
            var id = Guid.NewGuid().ToString();
            var name = "Test";
            var desc = "Test";

            // act
            var descriptor = new ActivityDescriptor( id, name, desc );

            // assert
            Assert.Equal( id, descriptor.Id );
            Assert.Equal( name, descriptor.Name );
            Assert.Equal( desc, descriptor.Description );
        }

        [Theory( DisplayName = "is valid identifier should return expected result" )]
        [InlineData( null, false )]
        [InlineData( "", false )]
        [InlineData( "abc", false )]
        [InlineData( "00000000-0000-0000-0000-000000000000", false )]
        [InlineData( "c976b124-2f07-4320-8e1a-99a3fdb77f6c", true )]
        public void IsValidIdentifierShouldReturnExpectedResult( string activityId, bool expected )
        {
            // arrange


            // act
            var actual = ActivityDescriptor.IsValidIdentifier( activityId );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "id should not allow invalid value" )]
        [InlineData( null, typeof( ArgumentNullException ) )]
        [InlineData( "", typeof( ArgumentNullException ) )]
        [InlineData( "abc", typeof( ArgumentException ) )]
        [InlineData( "00000000-0000-0000-0000-000000000000", typeof( ArgumentException ) )]
        public void IdPropertyShouldNotAllowInvalidValue( string value, Type exceptionType )
        {
            // arrange
            var descriptor = new ActivityDescriptor();

            // act
            var ex = (ArgumentException) Assert.Throws( exceptionType, () => descriptor.Id = value );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }
    }
}
