namespace More.ComponentModel.DataAnnotations
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="Validator"/>.
    /// </summary>
    public class ValidatorTest
    {
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

        [Fact( DisplayName = "for should return same validator for type" )]
        public void ForShouldReturnSameValidatorForType()
        {
            // arrange
            var validator = new Validator();
            var expected = validator.For<IComponent>();

            // act
            var actual = validator.For<IComponent>();

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "create context should not allow null instance" )]
        public void CreateContextShouldNotAllowNullInstance()
        {
            // arrange
            object instance = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.CreateContext( instance, null ) );

            // assert
            Assert.Equal( "instance", ex.ParamName );
        }

        [Fact( DisplayName = "validate object should not allow null context" )]
        public void ValidationObjectShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( null, context ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Fact( DisplayName = "validate object should not allow null context" )]
        public void ValidationObjectForAllPropertiesShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( null, context, true ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Fact( DisplayName = "try validate object should not allow null context" )]
        public void TryValidationObjectShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.TryValidateObject( null, context, null ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Fact( DisplayName = "try validate object should not allow null context" )]
        public void TryValidationObjectForAllPropertiesShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.TryValidateObject( null, context, null, true ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Fact( DisplayName = "validate property should not allow null context" )]
        public void ValidationPropertyShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.ValidateProperty( null, context ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Fact( DisplayName = "try validate property should not allow null context" )]
        public void TryValidationPropertyShouldNotAllowNullContext()
        {
            // arrange
            IValidationContext context = null;
            var validator = new Validator();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.TryValidateProperty( null, context, null ) );

            // assert
            Assert.Equal( "validationContext", ex.ParamName );
        }

        [Theory( DisplayName = "try validate object should return expected result" )]
        [MemberData( "ComponentData" )]
        public void TryValidateObjectShouldReturnExpectedResult( IComponent instance, int resultCount, bool expected )
        {
            // arrange
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            var actual = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( resultCount, results.Count );
        }

        [Fact( DisplayName = "try validate object should return true for null instance" )]
        public void TryValidateObjectShouldReturnTrueForNullInstance()
        {
            // arrange
            IComponent instance = null;
            var validator = new Validator();
            var context = validator.CreateContext( new Mock<IComponent>().Object, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "try validate object should return true for instance without rules" )]
        public void TryValidateObjectShouldReturnTrueForInstanceWithoutRules()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Theory( DisplayName = "try validate property should return expected result" )]
        [MemberData( "ComponentData" )]
        public void TryValidatePropertyShouldReturnExpectedResult( IComponent instance, int resultCount, bool expected )
        {
            // arrange
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var value = instance.Site;

            validator.For<IComponent>().Property( c => c.Site ).Required();
            context.MemberName = "Site";

            // act
            var actual = validator.TryValidateProperty( value, context, results );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( resultCount, results.Count );
        }

        [Fact( DisplayName = "try validate property should return true for type without rules" )]
        public void TryValidatePropertyShouldReturnTrueForTypeWithoutRules()
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
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "validate object should throw exception for invalid object" )]
        public void ValidateObjectShouldThrowExceptionForInvalidObject()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new Validator();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<IComponent>().Property( c => c.Site ).Required();

            // act
            var ex = Assert.Throws<ValidationException>( () => validator.ValidateObject( instance, context ) );

            // assert
            Assert.Equal( "Site", ex.ValidationResult.MemberNames.First() );
            Assert.Null( ex.Value );
        }

        [Fact( DisplayName = "validate object should throw exception for invalid value" )]
        public void ValidatePropertyShouldThrowExceptionForInvalidValue()
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
            var ex = Assert.Throws<ValidationException>( () => validator.ValidateProperty( value, context ) );

            // assert
            Assert.Equal( "Site", ex.ValidationResult.MemberNames.First() );
            Assert.Equal( value, ex.Value );
        }
    }
}
