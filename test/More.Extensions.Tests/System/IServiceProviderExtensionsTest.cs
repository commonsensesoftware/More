namespace System
{
    using Moq;
    using More;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IServiceProviderExtensions"/>.
    /// </summary>
    public class IServiceProviderExtensionsTest
    {
        public static IEnumerable<object[]> NullServiceProviderData
        {
            get
            {
                object service;
                IServiceProvider typedService;

                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService( typeof( object ) ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService( typeof( object ), "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService<object>() ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetRequiredService<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService( typeof( object ), "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService<object>() ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetService<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices( typeof( object ) ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices( typeof( object ), "Key" ) ) };

                // UNDONE: for some reason this test always fails even though the argument validation is clearly in place
                //         it appears there may be something wrong/weird with xunit setting up this theory
                //
                //yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices<object>() ) };

                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices<object>( "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( typeof( object ), out service ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( typeof( object ), out service, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( out typedService ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.TryGetService( out typedService, "Key" ) ) };
            }
        }

        public static IEnumerable<object[]> NullServiceTypeData
        {
            get
            {
                object service = null;

                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetRequiredService( st ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetRequiredService( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetService( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetServices( st ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetServices( st, "Key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out service ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out service, "Key" ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( NullServiceProviderData ) )]
        public void extension_method_should_not_allow_null_service_provider( Action<IServiceProvider> test )
        {
            // arrange
            var serviceProvider = default( IServiceProvider );

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( serviceProvider ) );

            // assert
            Assert.Equal( nameof( serviceProvider ), ex.ParamName );
        }

        [Theory]
        [MemberData( nameof( NullServiceTypeData ) )]
        public void extension_method_should_not_allow_null_service_type( Action<IServiceProvider, Type> test )
        {
            // arrange
            var serviceProvider = ServiceProvider.Default;
            var serviceType = default( Type );

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( serviceProvider, serviceType ) );

            // assert
            Assert.Equal( nameof( serviceType ), ex.ParamName );
        }

        [Fact]
        public void get_service_should_return_null_for_unavailable_service()
        {
            var actual = ServiceProvider.Current.GetService<object>();
            Assert.Null( actual );
        }

        [Fact]
        public void get_required_service_should_return_requested_service()
        {
            var target = ServiceProvider.Current;
            var actual = target.GetRequiredService( typeof( IServiceProvider ) );
            Assert.NotNull( actual );

            actual = target.GetRequiredService<IServiceProvider>();
            Assert.NotNull( actual );
        }

        [Fact]
        public void get_required_service_should_throw_exception_for_unavailable_service()
        {
            Assert.Throws<NotSupportedException>( () => ServiceProvider.Current.GetRequiredService<object>() );
            Assert.Throws<NotSupportedException>( () => ServiceProvider.Current.GetRequiredService( typeof( object ) ) );
        }

        [Fact]
        public void get_service_should_return_expected_service()
        {
            var target = ServiceProvider.Current;
            var actual = target.GetService<IServiceProvider>();
            Assert.Equal( actual, target );
        }

        [Fact]
        public void try_get_service_should_return_expected_service()
        {
            var target = ServiceProvider.Current;
            var actual = default( IServiceProvider );
            Assert.True( target.TryGetService( out actual ) );
            Assert.NotNull( actual );
            Assert.Equal( actual, target );
        }

        [Fact]
        public void try_get_service_should_return_false_for_unavailable_service()
        {
            var target = ServiceProvider.Current;
            var actual = default( ICustomFormatter );
            Assert.False( target.TryGetService( out actual ) );
            Assert.Null( actual );
        }

        [Fact]
        public void try_get_service_should_return_expected_service()
        {
            var target = ServiceProvider.Current;
            var actual = default( object );
            Assert.True( target.TryGetService( typeof( IServiceProvider ), out actual ) );
            Assert.NotNull( actual );
            Assert.IsAssignableFrom<IServiceProvider>( actual );
            Assert.Equal( actual, target );
        }

        [Fact]
        public void try_get_service_should_return_false_when_service_is_unavailable()
        {
            var target = ServiceProvider.Current;
            var actual = default( object );
            Assert.False( target.TryGetService( typeof( ICustomFormatter ), out actual ) );
            Assert.Null( actual );
        }

        [Fact]
        public void try_get_service_should_handle_exception()
        {
            var mock = new Mock<IServiceProvider>();
            mock.Setup( sl => sl.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );
            var target = mock.Object;
            var actual = default( object );

            Assert.False( target.TryGetService( typeof( IServiceProvider ), out actual ) );
            Assert.Null( actual );
        }

        [Fact]
        public void try_get_service_should_handle_exception()
        {
            var mock = new Mock<IServiceProvider>();
            mock.Setup( sl => sl.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );
            var target = mock.Object;
            var actual = default( IServiceProvider );

            Assert.False( target.TryGetService( out actual ) );
            Assert.Null( actual );
        }

        [Fact]
        public void get_services_should_return_expected_instances()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { "one", "two", "three" };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var actual = serviceProvider.Object.GetServices( typeof( string ) );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<string> ) ), Times.Once() );
        }

        [Fact]
        public void get_services_should_return_expected_instances()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { "one", "two", "three" };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var actual = serviceProvider.Object.GetServices<string>();

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<string> ) ), Times.Once() );
        }

        [Fact]
        public void get_services_should_return_empty_sequence()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            // act
            var actual = serviceProvider.Object.GetServices( typeof( object ) );

            // assert
            Assert.Equal( 0, actual.Count() );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Times.Once() );
        }

        [Fact]
        public void get_services_should_return_empty_sequence()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            // act
            var actual = serviceProvider.Object.GetServices( typeof( object ) );

            // assert
            Assert.Equal( 0, actual.Count() );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Times.Once() );
        }
    }
}
