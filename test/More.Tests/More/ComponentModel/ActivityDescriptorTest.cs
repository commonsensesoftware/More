namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ActivityDescriptorTest
    {
        [Fact]
        public void new_activity_descriptor_should_set_default_properties()
        {
            // arrange
            var name = "Activity";
            var desc = "";

            // act
            var descriptor = new ActivityDescriptor();

            // assert
            ActivityDescriptor.IsValidIdentifier( descriptor.Id ).Should().BeTrue();
            descriptor.ShouldBeEquivalentTo( new { Id = descriptor.Id, Name = name, Description = desc } );
        }
        [Fact]
        public void new_activity_descriptor_should_set_expected_properties()
        {
            // arrange
            var id = Guid.NewGuid().ToString();
            var name = "Test";
            var desc = "Test";

            // act
            var descriptor = new ActivityDescriptor( id, name, desc );

            // assert
            descriptor.ShouldBeEquivalentTo( new { Id = id, Name = name, Description = desc } );
        }

        [Theory]
        [InlineData( null, false )]
        [InlineData( "", false )]
        [InlineData( "abc", false )]
        [InlineData( "00000000-0000-0000-0000-000000000000", false )]
        [InlineData( "c976b124-2f07-4320-8e1a-99a3fdb77f6c", true )]
        public void is_valid_identifier_should_return_expected_result( string activityId, bool expected )
        {
            // arrange


            // act
            var result = ActivityDescriptor.IsValidIdentifier( activityId );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( null, typeof( ArgumentNullException ) )]
        [InlineData( "", typeof( ArgumentNullException ) )]
        [InlineData( "abc", typeof( ArgumentException ) )]
        [InlineData( "00000000-0000-0000-0000-000000000000", typeof( ArgumentException ) )]
        public void id_should_not_allow_invalid_value( string value, Type exceptionType )
        {
            // arrange
            var descriptor = new ActivityDescriptor();

            // act
            Action setId = () => descriptor.Id = value;

            // assert
            setId.ShouldThrow<ArgumentException>().And.ParamName.Should().Be( nameof( value ) );
        }
    }
}