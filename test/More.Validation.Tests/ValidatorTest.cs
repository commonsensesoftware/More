namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;

    public class ValidatorTest
    {
        [Fact]
        public void for_should_return_same_validator_for_type()
        {
            // arrange
            var validator = new Validator();
            var first = validator.For<IComponent>();

            // act
            var second = validator.For<IComponent>();

            // assert
            second.Should().BeSameAs( first );
        }

        [Fact]
        public void create_context_should_not_allow_null_instance()
        {
            // arrange
            var instance = default( object );
            var validator = new Validator();

            // act
            Action createContext = () => validator.CreateContext( instance, null );

            // assert
            createContext.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void validate_object_should_not_allow_null_context()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action validateObject = () => validator.ValidateObject( null, validationContext );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Fact]
        public void validate_object_should_not_allow_null_context_for_all_properties()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action validateObject = () => validator.ValidateObject( null, validationContext, true );

            // assert
            validateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Fact]
        public void try_validate_object_should_not_allow_null_context()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action tryValidateObject = () => validator.TryValidateObject( null, validationContext, null );

            // assert
            tryValidateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Fact]
        public void try_validate_object_should_not_allow_null_context_for_all_properties()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action tryValidateObject = () => validator.TryValidateObject( null, validationContext, null, true );

            // assert
            tryValidateObject.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Fact]
        public void validate_property_should_not_allow_null_context()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action validateProperty = () => validator.ValidateProperty( null, validationContext );

            // assert
            validateProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Fact]
        public void try_validate_property_should_not_allow_null_context()
        {
            // arrange
            var validationContext = default( IValidationContext );
            var validator = new Validator();

            // act
            Action tryValidateProperty = () => validator.TryValidateProperty( null, validationContext, null );

            // assert
            tryValidateProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( validationContext ) );
        }

        [Theory]
        [MemberData( nameof( ComponentData ) )]
        public void try_validate_object_should_return_expected_result( IComponent instance, int resultCount, bool expected )
        {
            // arrange
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().Be( expected );
            results.Should().HaveCount( resultCount );
        }

        [Fact]
        public void try_validate_object_should_return_true_for_null_instance()
        {
            // arrange
            var instance = default( IComponent );
            var validator = new Validator();
            var context = validator.CreateContext( new Mock<IComponent>().Object, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void try_validate_object_should_return_true_for_instance_without_rules()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Theory]
        [MemberData( nameof( ComponentData ) )]
        public void try_validate_property_should_return_expected_result( IComponent instance, int resultCount, bool expected )
        {
            // arrange
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var value = instance.Site;

            validator.For<IComponent>().Property( c => c.Site ).Required();
            context.MemberName = "Site";

            // act
            var valid = validator.TryValidateProperty( value, context, results );

            // assert
            valid.Should().Be( expected );
            results.Should().HaveCount( resultCount );
        }

        [Fact]
        public void try_validate_property_should_return_true_for_type_without_rules()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var value = instance.Site;

            // act
            var valid = validator.TryValidateProperty( value, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void validate_object_should_throw_exception_for_invalid_object()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            Action validateObject = () => validator.ValidateObject( instance, context );

            // assert
            validateObject.ShouldThrow<ValidationException>().And.ValidationResult.MemberNames.Should().Equal( new[] { "Site" } );
        }

        [Fact]
        public void validate_object_should_throw_exception_for_invalid_value()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var value = instance.Site;

            validator.For<IComponent>().Property( c => c.Site ).Required();
            context.MemberName = "Site";

            // act
            Action validateObject = () => validator.ValidateProperty( value, context );

            // assert
            validateObject.ShouldThrow<ValidationException>().And
                          .ShouldBeEquivalentTo(
                            new { ValidationResult = new { MemberNames = new[] { "Site" } }, Value = value },
                            options => options.ExcludingMissingMembers() );
        }

        public static IEnumerable<object[]> ComponentData
        {
            get
            {
                var component1 = new Mock<IComponent>();
                var component2 = new Mock<IComponent>();

                component2.SetupProperty( c => c.Site, new Mock<ISite>().Object );

                yield return new object[] { component1.Object, 1, false };
                yield return new object[] { component2.Object, 0, true };
            }
        }
    }
}