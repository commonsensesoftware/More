namespace More.ComponentModel.DataAnnotations
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObjectValidator{T}"/>.
    /// </summary>
    public class ObjectValidatorTTest
    {
        [Fact( DisplayName = "property should not allow null expression" )]
        public void PropertyShouldNotAllowNullExpression()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();
            Expression<Func<IComponent, ISite>> propertyExpression = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.Property( propertyExpression ) );

            // assert
            Assert.Equal( "propertyExpression", ex.ParamName );
        }

        [Fact( DisplayName = "property should return same builder for expression" )]
        public void PropertyShouldReturnSameBuilderForExpression()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();

            // act
            var expected = validator.Property( c => c.Site );
            var actual = validator.Property( c => c.Site );

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "validate object should not allow null instance" )]
        public void ValidateObjectShouldNotAllowNullInstance()
        {
            // arrange
            IComponent instance = null;
            var objectValidator = new ObjectValidator<IComponent>();
            IObjectValidator<IComponent> validatorOfT = objectValidator;
            IObjectValidator validator = objectValidator;

            // act
            var ex1 = Assert.Throws<ArgumentNullException>( () => validatorOfT.ValidateObject( instance ) );
            var ex2 = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( instance ) );

            // assert
            Assert.Equal( "instance", ex1.ParamName );
            Assert.Equal( "instance", ex2.ParamName );
        }

        [Fact( DisplayName = "validate object should not allow null instance" )]
        public void ValidateObjectWithPropertyNamesShouldNotAllowNullInstance()
        {
            // arrange
            IComponent instance = null;
            var propertyNames = new[] { "Site" };
            var objectValidator = new ObjectValidator<IComponent>();
            IObjectValidator<IComponent> validatorOfT = objectValidator;
            IObjectValidator validator = objectValidator;

            // act
            var ex1 = Assert.Throws<ArgumentNullException>( () => validatorOfT.ValidateObject( instance, propertyNames ) );
            var ex2 = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( instance, propertyNames ) );

            // assert
            Assert.Equal( "instance", ex1.ParamName );
            Assert.Equal( "instance", ex2.ParamName );
        }

        [Fact( DisplayName = "validate object should not allow null property names" )]
        public void ValidateObjectShouldNotAllowNullPropertyNames()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            IEnumerable<string> propertyNames = null;
            var objectValidator = new ObjectValidator<IComponent>();
            IObjectValidator<IComponent> validatorOfT = objectValidator;
            IObjectValidator validator = objectValidator;

            // act
            var ex1 = Assert.Throws<ArgumentNullException>( () => validatorOfT.ValidateObject( instance, propertyNames ) );
            var ex2 = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( instance, propertyNames ) );

            // assert
            Assert.Equal( "propertyNames", ex1.ParamName );
            Assert.Equal( "propertyNames", ex2.ParamName );
        }

        [Fact( DisplayName = "validate object should validate all properties" )]
        public void ValidateObjectShouldValidateAllProperties()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new ObjectValidator<IComponent>();

            validator.Property( c => c.Site ).Required();

            // act
            var actual = validator.ValidateObject( instance );

            // assert
            Assert.Equal( 1, actual.Count );
            Assert.Equal( "Site", actual[0].MemberNames.Single() );
        }

        [Fact( DisplayName = "validate object should validate specified properties" )]
        public void ValidateObjectShouldValidateSpecifiedProperties()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var validator = new ObjectValidator<IComponent>();
            var propertyNames = new[] { "Site" };

            validator.Property( c => c.Site ).Required();

            // act
            var actual = validator.ValidateObject( instance, propertyNames );

            // assert
            Assert.Equal( 1, actual.Count );
            Assert.Equal( "Site", actual[0].MemberNames.Single() );
        }

        [Theory( DisplayName = "validate property should not allow null or empty property name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ValidatePropertyShouldNotAllowNullOrEmptyPropertyName( string propertyName )
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();
            object value = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => validator.ValidateProperty( propertyName, value ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "validate property should return expected result" )]
        public void ValidatePropertyShouldReturnExpectedResult()
        {
            // arrange
            var validator = new ObjectValidator<IComponent>();

            validator.Property( c => c.Site ).Required();

            // act
            var actual = validator.ValidateProperty( "Site", null );

            // assert
            Assert.Equal( 1, actual.Count );
            Assert.Equal( "Site", actual[0].MemberNames.Single() );
        }
    }
}
