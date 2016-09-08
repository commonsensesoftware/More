namespace System.ComponentModel.DataAnnotations
{
    using More;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidationAdapterExtensionsTest"/>.
    /// </summary>
    public class ValidationAdapterExtensionsTest
    {
        [Fact( DisplayName = "adapt should return validation context adapter" )]
        public void AdaptShouldReturnValidationContextAdapter()
        {
            // arrange
            var instance = new object();
            var service = new object();
            var serviceProvider = new ServiceContainer();
            var items = new Dictionary<object, object>() { { "Test", "Test" } };
            var expected = new ValidationContext( instance, serviceProvider, items );

            expected.MemberName = "Foo";
            serviceProvider.AddService( typeof( object ), service );

            // act
            var actual = expected.Adapt();

            // assert
            Assert.Equal( expected.DisplayName, actual.DisplayName );
            Assert.Same( expected.Items["Test"], actual.Items["Test"] );
            Assert.Equal( expected.MemberName, actual.MemberName );
            Assert.Same( expected.ObjectInstance, actual.ObjectInstance );
            Assert.Equal( expected.ObjectType, actual.ObjectType );
            Assert.Same( expected.GetService( typeof( object ) ), actual.GetService( typeof( object ) ) );
        }

        [Fact( DisplayName = "adapt should return validation result adapter" )]
        public void AdaptShouldReturnValidationResultAdapter()
        {
            // arrange
            var expected = new ValidationResult( "Invalid", new[] { "Foo", "Bar" } );

            // act
            var actual = expected.Adapt();

            // assert
            Assert.Equal( expected.ErrorMessage, actual.ErrorMessage );
            Assert.Equal( expected.MemberNames, actual.MemberNames );
        }
    }
}
