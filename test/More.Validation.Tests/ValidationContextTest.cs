namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ValidationContextTest
    {
        [Theory]
        [MemberData( nameof( NullInstanceData ) )]
        public void new_validation_context_should_not_allow_null_instance( Action<object> newValidationContext )
        {
            // arrange
            var instance = default( object );

            // act
            Action @new = () => newValidationContext( instance );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( instance ) );
        }

        [Fact]
        public void new_validation_context_should_not_allow_null_service_provider()
        {
            // arrange
            var serviceProvider = default( IServiceProvider );
            var instance = new object();

            // act
            Action @new = () => new ValidationContext( instance, serviceProvider, null );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( serviceProvider ) );
        }

        [Theory]
        [MemberData( nameof( NewData ) )]
        public void new_validation_context_should_set_expected_properties( Func<object, IDictionary<object, object>, ValidationContext> @new )
        {
            // arrange
            var instance = new object();
            var items = new Dictionary<object, object>();

            // act
            var context = @new( instance, items );

            // assert
            context.ShouldBeEquivalentTo(
                new
                {
                    DisplayName = default( string ),
                    MemberName = default( string ),
                    ObjectInstance = instance,
                    ObjectType = instance.GetType(),
                    Items = items
                } );
        }

        [Fact]
        public void get_service_should_call_underlying_service_provider()
        {
            // arrange
            var instance = new object();
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            var context = new ValidationContext( instance, serviceProvider.Object, null );

            // act
            var service = context.GetService( typeof( object ) );

            // assert
            service.Should().BeNull();
            serviceProvider.Verify( sp => sp.GetService( typeof( object ) ), Times.Once() );
        }

        public static IEnumerable<object[]> NullInstanceData
        {
            get
            {
                yield return new object[] { new Action<object>( i => new ValidationContext( i, null ) ) };
                yield return new object[] { new Action<object>( i => new ValidationContext( i, ServiceProvider.Default, null ) ) };
            }
        }

        public static IEnumerable<object[]> NewData
        {
            get
            {
                yield return new object[] { new Func<object, IDictionary<object, object>, ValidationContext>( ( instance, items ) => new ValidationContext( instance, items ) ) };
                yield return new object[] { new Func<object, IDictionary<object, object>, ValidationContext>( ( instance, items ) => new ValidationContext( instance, ServiceProvider.Default, items ) ) };
            }
        }
    }
}