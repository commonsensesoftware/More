namespace System
{
    using FluentAssertions;
    using Moq;
    using More;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Moq.Times;

    public class IServiceProviderExtensionsTest
    {
        [Theory]
        [MemberData( nameof( NullServiceProviderData ) )]
        public void extension_method_should_not_allow_null_service_provider( Action<IServiceProvider> test )
        {
            // arrange
            var serviceProvider = default( IServiceProvider );

            // act
            Action extensionMethod = () => test( serviceProvider );

            // assert
            extensionMethod.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( serviceProvider ) );
        }

        [Theory]
        [MemberData( nameof( NullServiceTypeData ) )]
        public void extension_method_should_not_allow_null_service_type( Action<IServiceProvider, Type> test )
        {
            // arrange
            var serviceProvider = ServiceProvider.Default;
            var serviceType = default( Type );

            // act
            Action extensionMethod = () => test( serviceProvider, serviceType );

            // assert
            extensionMethod.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( serviceType ) );
        }

        [Fact]
        public void get_service_should_return_null_for_unavailable_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var service = serviceProvider.GetService<object>();

            // assert
            service.Should().BeNull();
        }

        [Fact]
        public void get_required_service_should_return_requested_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var service = serviceProvider.GetRequiredService( typeof( IServiceProvider ) );

            // assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void get_required_service_should_return_requested_service_with_generic_type()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var service = serviceProvider.GetRequiredService<IServiceProvider>();

            // assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void get_required_service_should_throw_exception_for_unavailable_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            Action getRequiredService = () => ServiceProvider.Current.GetRequiredService( typeof( object ) );

            // assert
            getRequiredService.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void get_required_service_should_throw_exception_for_unavailable_service_with_generic_type()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            Action getRequiredService = () => ServiceProvider.Current.GetRequiredService<object>();

            // assert
            getRequiredService.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void get_service_should_return_expected_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // acdt
            var service = serviceProvider.GetService<IServiceProvider>();

            // assert
            service.Should().BeSameAs( serviceProvider );
        }

        [Fact]
        public void try_get_service_should_return_expected_service_with_generic_type()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var succeeded = serviceProvider.TryGetService( out IServiceProvider service );

            // assert
            succeeded.Should().BeTrue();
            service.Should().BeSameAs( serviceProvider );
        }

        [Fact]
        public void try_get_service_should_return_false_for_unavailable_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var succeeded = serviceProvider.TryGetService( out ICustomFormatter service );

            // assert
            succeeded.Should().BeFalse();
            service.Should().BeNull();
        }

        [Fact]
        public void try_get_service_should_return_expected_service()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var succeeded = serviceProvider.TryGetService( typeof( IServiceProvider ), out object service );

            // act
            succeeded.Should().BeTrue();
            service.Should().BeSameAs( serviceProvider );
        }

        [Fact]
        public void try_get_service_should_return_false_when_service_is_unavailable()
        {
            // arrange
            var serviceProvider = ServiceProvider.Current;

            // act
            var succeeded = serviceProvider.TryGetService( typeof( ICustomFormatter ), out object service );

            // assert
            succeeded.Should().BeFalse();
            service.Should().BeNull();
        }

        [Fact]
        public void try_get_service_should_handle_exception()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );

            // act
            var succeeded = serviceProvider.Object.TryGetService( typeof( IServiceProvider ), out object service );

            // assert
            succeeded.Should().BeFalse();
            service.Should().BeNull();
        }

        [Fact]
        public void try_get_service_should_handle_exception_with_generic_type()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );

            // act
            var succeeded = serviceProvider.Object.TryGetService( out IServiceProvider service );

            // assert
            succeeded.Should().BeFalse();
            service.Should().BeNull();
        }

        [Fact]
        public void get_services_should_return_expected_instances()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { "one", "two", "three" };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var services = serviceProvider.Object.GetServices( typeof( string ) );

            // assert
            services.Should().Equal( expected );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<string> ) ), Once() );
        }

        [Fact]
        public void get_services_should_return_expected_instances_with_generic_type()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { "one", "two", "three" };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var services = serviceProvider.Object.GetServices<string>();

            // assert
            services.Should().Equal( expected );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<string> ) ), Once() );
        }

        [Fact]
        public void get_services_should_return_empty_sequence()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            // act
            var services = serviceProvider.Object.GetServices( typeof( object ) );

            // assert
            services.Should().BeEmpty();
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Once() );
        }

        [Fact]
        public void get_services_should_return_empty_sequence_with_generic_type()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            // act
            var services = serviceProvider.Object.GetServices<object>();

            // assert
            services.Should().BeEmpty();
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Once() );
        }

        public static IEnumerable<object[]> NullServiceProviderData
        {
            get
            {
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService( typeof( object ) ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService( typeof( object ), "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService<object>() ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService( typeof( object ), "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService<object>() ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices( typeof( object ) ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices( typeof( object ), "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( typeof( object ), out object service ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( typeof( object ), out object service, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( out IServiceProvider service ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( out IServiceProvider service, "Key" ) ) };
            }
        }

        public static IEnumerable<object[]> NullServiceTypeData
        {
            get
            {
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetRequiredService( st ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetRequiredService( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetService( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetServices( st ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetServices( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out object service ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out object service, "Key" ) ) };
            }
        }
    }
}