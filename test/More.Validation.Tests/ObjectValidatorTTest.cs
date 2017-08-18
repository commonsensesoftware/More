namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;

    public class ObjectValidatorTTest
    {
        [Fact]
        public void property_should_not_allow_null_expression()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();
            var propertyExpression = default( Expression<Func<IComponent, ISite>> );

            // act
            Action property = () => validator.Property( propertyExpression );

            // assert
            property.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyExpression ) );
        }

        [Fact]
        public void property_should_return_same_builder_for_expression()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();

            // act
            var first = validator.Property( c => c.Site );
            var second = validator.Property( c => c.Site );

            // assert
            second.Should().BeSameAs( first );
        }

        [Fact]
        public void validate_object_should_not_allow_null_instance()
        {
            // arrange
            var instance = default( IComponent );
            IObjectValidator validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void generic_validate_object_should_not_allow_null_instance()
        {
            // arrange
            var instance = default( IComponent );
            IObjectValidator<IComponent> validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void validate_object_should_not_allow_null_instance_with_property_names()
        {
            // arrange
            var instance = default( IComponent );
            var propertyNames = new[] { "Site" };
            IObjectValidator validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance, propertyNames );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void generic_validate_object_should_not_allow_null_instance_with_property_names()
        {
            // arrange
            var instance = default( IComponent );
            var propertyNames = new[] { "Site" };
            IObjectValidator<IComponent> validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance, propertyNames );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void validate_object_should_not_allow_null_property_names()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var propertyNames = default( IEnumerable<string> );
            IObjectValidator validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance, propertyNames );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyNames ) );
        }

        [Fact]
        public void generic_validate_object_should_not_allow_null_property_names()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var propertyNames = default( IEnumerable<string> );
            IObjectValidator<IComponent> validator = new ObjectValidator<IComponent>();

            // act
            Action validateObject = () => validator.ValidateObject( instance, propertyNames );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyNames ) );
        }

        [Fact]
        public void validate_object_should_validate_all_properties()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new ObjectValidator<IComponent>();

            validator.Property( c => c.Site ).Required();

            // act
            var results = validator.ValidateObject( instance );

            // assert
            results.Single().MemberNames.Single().Should().Be( "Site" );
        }

        [Fact]
        public void validate_object_should_validate_specified_properties()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new ObjectValidator<IComponent>();
            var propertyNames = new[] { "Site" };

            validator.Property( c => c.Site ).Required();

            // act
            var results = validator.ValidateObject( instance, propertyNames );

            // assert
            results.Single().MemberNames.Single().Should().Be( "Site" );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void validate_property_should_not_allow_null_or_empty_property_name( string propertyName )
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();
            var value = default( object );

            // act
            Action validateProperty = () => validator.ValidateProperty( propertyName, value );

            // assert
            validateProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void validate_property_should_return_expected_result()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();

            validator.Property( c => c.Site ).Required();

            // act
            var results = validator.ValidateProperty( "Site", null );

            // assert
            results.Single().MemberNames.Single().Should().Be( "Site" );
        }
    }
}