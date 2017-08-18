namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using Xunit;

    public class PropertyValidatorT1T2Test
    {
        [Fact]
        public void new_property_validator_should_not_allow_null_expression()
        {
            // arrange
            var propertyExpression = default( Expression<Func<IComponent, ISite>> );

            // act
            Action @new = () => new PropertyValidator<IComponent, ISite>( propertyExpression, "Site" );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyExpression ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_property_validator_should_not_allow_null_or_empty_property_name( string propertyName )
        {
            // arrange

            // act
            Action @new = () => new PropertyValidator<IComponent, ISite>( c => c.Site, propertyName );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void apply_should_return_self()
        {
            // arrange
            var validator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );

            // act
            var builder = validator.Apply( new RequiredRule<ISite>() );

            // assert
            builder.Should().BeSameAs( validator );
        }

        [Fact]
        public void validate_value_should_evaluate_value()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var value = default( ISite );
            IPropertyValidator validator = propertyValidator;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var result = validator.ValidateValue( value );

            // assert
            result.Should().HaveCount( 1 );
        }

        [Fact]
        public void generic_validate_value_should_evaluate_value()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var value = default( ISite );
            IPropertyValidator<IComponent> validator = propertyValidator;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var result = validator.ValidateValue( value );

            // assert
            result.Should().HaveCount( 1 );
        }

        [Fact]
        public void validate_object_should_not_allow_null_instance()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var instance = default( IComponent );
            IPropertyValidator validator = propertyValidator;

            // act
            Action validateObject = () => validator.ValidateObject( instance );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void generic_validate_object_should_not_allow_null_instance()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var instance = default( IComponent );
            IPropertyValidator<IComponent> validator = propertyValidator;

            // act
            Action validateObject = () => validator.ValidateObject( instance );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void validate_object_should_evaluate_instance()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            IPropertyValidator validator = propertyValidator;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var results = validator.ValidateObject( instance );

            // assert
            results.Should().HaveCount( 1 );
        }

        [Fact]
        public void generic_validate_object_should_evaluate_instance()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            IPropertyValidator<IComponent> validator = propertyValidator;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var results = validator.ValidateObject( instance );

            // assert
            results.Should().HaveCount( 1 );
        }

        [Fact]
        public void validate_value_should_return_expected_result_for_incompatible_value()
        {
            // arrange
            IPropertyValidator validator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var errorMessage = "The Site field of type System.ComponentModel.ISite is incompatible with a value of type System.Object.";
            var value = new object();

            // act
            var results = validator.ValidateValue( value );

            // assert
            results.ShouldBeEquivalentTo( new[] { new { ErrorMessage = errorMessage, MemberNames = new[] { "Site" } } } );
        }

        [Fact]
        public void validate_value_should_return_expected_result_for_nonX2Dnull_value()
        {
            // arrange
            IPropertyValidator validator = new PropertyValidator<ISite, bool>( s => s.DesignMode, "DesignMode" );
            var errorMessage = "The DesignMode field of type System.Boolean cannot be validated with a null value.";
            var value = default( object );

            // act
            var results = validator.ValidateValue( value );

            // assert
            results.ShouldBeEquivalentTo( new[] { new { ErrorMessage = errorMessage, MemberNames = new[] { "DesignMode" } } } );
        }
    }
}