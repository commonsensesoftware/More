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
    /// Provides unit tests for <see cref="PropertyValidator{TObject,TValue}"/>.
    /// </summary>
    public class PropertyValidatorT1T2Test
    {
        [Fact( DisplayName = "new property validator should not allow null expression" )]
        public void ConstructorShouldNotAllowNullPropertyExpression()
        {
            // arrange
            Expression<Func<IComponent, ISite>> propertyExpression = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new PropertyValidator<IComponent, ISite>( propertyExpression, "Site" ) );

            // assert
            Assert.Equal( "propertyExpression", ex.ParamName );
        }

        [Theory( DisplayName = "new property validator should not allow null or empty property name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ConstructorShouldNotAllowNullPropertyName( string propertyName )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new PropertyValidator<IComponent, ISite>( c => c.Site, propertyName ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "apply should return self" )]
        public void ApplyShouldReturnSelf()
        {
            // arrange
            var expected = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );

            // act
            var actual = expected.Apply( new RequiredRule<ISite>() );

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "validate value should evaluate value" )]
        public void ValidateValueShouldEvaluateValue()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            IPropertyValidator validator = propertyValidator;
            IPropertyValidator<IComponent> validatorOfT = propertyValidator;
            ISite value = null;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var actual1 = validator.ValidateValue( value );
            var actual2 = validatorOfT.ValidateValue( value );

            // assert
            Assert.Equal( 1, actual1.Count );
            Assert.Equal( 1, actual2.Count );
        }

        [Fact( DisplayName = "validate object should not allow null instance" )]
        public void ValidateObjectShouldNotAllowNullInstance()
        {
            // arrange
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            IPropertyValidator validator = propertyValidator;
            IPropertyValidator<IComponent> validatorOfT = propertyValidator;
            IComponent instance = null;
            
            // act
            var ex1 = Assert.Throws<ArgumentNullException>( () => validator.ValidateObject( instance ) );
            var ex2 = Assert.Throws<ArgumentNullException>( () => validatorOfT.ValidateObject( instance ) );

            // assert
            Assert.Equal( "instance", ex1.ParamName );
            Assert.Equal( "instance", ex2.ParamName );
        }

        [Fact( DisplayName = "validate object should evaluate instance" )]
        public void ValidateObjectShouldEvaluateInstance()
        {
            // arrange
            var instance = new Mock<IComponent>().Object;
            var propertyValidator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            IPropertyValidator validator = propertyValidator;
            IPropertyValidator<IComponent> validatorOfT = propertyValidator;

            propertyValidator.Apply( new RequiredRule<ISite>() );

            // act
            var actual1 = validator.ValidateObject( instance );
            var actual2 = validatorOfT.ValidateObject( instance );

            // assert
            Assert.Equal( 1, actual1.Count );
            Assert.Equal( 1, actual2.Count );
        }

        [Fact( DisplayName = "validate value should return expected result for incompatible value" )]
        public void ValidateValueShouldReturnExpectedResultForIncompatibleValue()
        {
            // arrange
            IPropertyValidator validator = new PropertyValidator<IComponent, ISite>( c => c.Site, "Site" );
            var errorMessage = "The Site field of type System.ComponentModel.ISite is incompatible with a value of type System.Object.";
            var value = new object();

            // act
            var actual = validator.ValidateValue( value );

            // assert
            Assert.Equal( 1, actual.Count );
            Assert.Equal( errorMessage, actual[0].ErrorMessage );
            Assert.Equal( "Site", actual[0].MemberNames.Single() );
        }

        [Fact( DisplayName = "validate value should return expected result for non-null value" )]
        public void ValidateValueShouldReturnExpectedResultForNonNullValue()
        {
            // arrange
            IPropertyValidator validator = new PropertyValidator<ISite, bool>( s => s.DesignMode, "DesignMode" );
            var errorMessage = "The DesignMode field of type System.Boolean cannot be validated with a null value.";
            object value = null;

            // act
            var actual = validator.ValidateValue( value );

            // assert
            Assert.Equal( 1, actual.Count );
            Assert.Equal( errorMessage, actual[0].ErrorMessage );
            Assert.Equal( "DesignMode", actual[0].MemberNames.Single() );
        }
    }
}
