namespace System.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using Xunit;

    public class ValidationAdapterExtensionsTest
    {
        [Fact]
        public void adapt_should_return_validation_context_adapter()
        {
            // arrange
            var instance = new object();
            var service = new object();
            var items = new Dictionary<object, object>() { ["Test"] = "Test" };
            var context = new ValidationContext( instance, new ServiceContainer(), items ) { MemberName = "Foo" };

            // act
            var adapted = context.Adapt();

            // assert
            adapted.ShouldBeEquivalentTo(
                new
                {
                    DisplayName = context.DisplayName,
                    MemberName = "Foo",
                    ObjectInstance = instance,
                    ObjectType = instance.GetType(),
                    Items = new Dictionary<object, object>() { ["Test"] = "Test" }
                } );
        }

        [Fact]
        public void validation_context_adapter_should_call_underlying_service_provider()
        {
            // arrange
            var instance = new object();
            var service = new object();
            var serviceProvider = new ServiceContainer();
            var items = new Dictionary<object, object>() { ["Test"] = "Test" };
            var context = new ValidationContext( instance, serviceProvider, items );

            serviceProvider.AddService( typeof( object ), service );

            // act
            var adapted = context.Adapt();

            // assert
            adapted.GetService( typeof( object ) ).Should().BeSameAs( context.GetService( typeof( object ) ) );
        }

        [Fact]
        public void adapt_should_return_validation_result_adapter()
        {
            // arrange
            var result = new ValidationResult( "Invalid", new[] { "Foo", "Bar" } );

            // act
            var adapted = result.Adapt();

            // assert
            adapted.ShouldBeEquivalentTo( new { ErrorMessage = "Invalid", MemberNames = new[] { "Foo", "Bar" } } );
        }
    }
}