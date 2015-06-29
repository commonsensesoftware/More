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
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices( typeof( object ), "key" ) ) };
                yield return new object[] { new Action<IServiceProvider>( sp => sp.GetServices<object>() ) };
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
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.GetServices( st, "key" ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out service ) ) };
                yield return new object[] { new Action<IServiceProvider, Type>( ( sp, st ) => sp.TryGetService( st, out service, "Key" ) ) };
            }
        }

        [Theory( DisplayName = "extension method should not allow null service provider" )]
        [MemberData( "NullServiceProviderData" )]
        public void ExtensionMethodShouldNotAllowNullServiceProvider( Action<IServiceProvider> test )
        {
            // arrange
            IServiceProvider serviceProvider = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( serviceProvider ) );

            // assert
            Assert.Equal( "serviceProvider", ex.ParamName );
        }

        [Theory( DisplayName = "extension method should not allow null service type" )]
        [MemberData( "NullServiceTypeData" )]
        public void ExtensionMethodShouldNotAllowNullServiceType( Action<IServiceProvider, Type> test )
        {
            // arrange
            var serviceProvider = ServiceProvider.Default;
            Type serviceType = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( serviceProvider, serviceType ) );

            // assert
            Assert.Equal( "serviceType", ex.ParamName );
        }

        [Fact( DisplayName = "get servie should return null for unavailable service" )]
        public void GetServiceShouldReturnNullWhenRequestedServiceIsUnavailable()
        {
            var actual = ServiceProvider.Current.GetService<object>();
            Assert.Null( actual );
        }

        [Fact( DisplayName = "get required service should return requested service" )]
        public void GetRequiredServiceShouldReturnRequestedService()
        {
            var target = ServiceProvider.Current;
            var actual = target.GetRequiredService( typeof( IServiceProvider ) );
            Assert.NotNull( actual );

            actual = target.GetRequiredService<IServiceProvider>();
            Assert.NotNull( actual );
        }

        [Fact( DisplayName = "get required service should throw exception for unavailable service" )]
        public void GetRequiredServiceShouldThrowExceptionWhenRequestedServiceIsUnavailable()
        {
            Assert.Throws<NotSupportedException>( () => ServiceProvider.Current.GetRequiredService<object>() );
            Assert.Throws<NotSupportedException>( () => ServiceProvider.Current.GetRequiredService( typeof( object ) ) );
        }

        [Fact( DisplayName = "get service should return expected service" )]
        public void GetServiceShouldReturnRequestedService()
        {
            var target = ServiceProvider.Current;
            var actual = target.GetService<IServiceProvider>();
            Assert.Equal( actual, target );
        }

        [Fact( DisplayName = "try get service should return expected service" )]
        public void TryGetServiceOfTShouldReturnRequestedService()
        {
            var target = ServiceProvider.Current;
            IServiceProvider actual = null;
            Assert.True( target.TryGetService( out actual ) );
            Assert.NotNull( actual );
            Assert.Equal( actual, target );
        }

        [Fact( DisplayName = "try get service should return false for unavailable service" )]
        public void TryGetServiceOfTShouldReturnFalseWhenRequestedServiceIsUnavailable()
        {
            var target = ServiceProvider.Current;
            ICustomFormatter actual = null;
            Assert.False( target.TryGetService( out actual ) );
            Assert.Null( actual );
        }

        [Fact( DisplayName = "try get service should return expected service" )]
        public void TryGetServiceShouldReturnRequestedService()
        {
            var target = ServiceProvider.Current;
            object actual = null;
            Assert.True( target.TryGetService( typeof( IServiceProvider ), out actual ) );
            Assert.NotNull( actual );
            Assert.IsAssignableFrom<IServiceProvider>( actual );
            Assert.Equal( actual, target );
        }

        [Fact( DisplayName = "try get service should return false when service is unavailable" )]
        public void TryGetServiceShouldReturnFalseWhenRequestedServiceIsUnavailable()
        {
            var target = ServiceProvider.Current;
            object actual = null;
            Assert.False( target.TryGetService( typeof( ICustomFormatter ), out actual ) );
            Assert.Null( actual );
        }

        [Fact( DisplayName = "try get service should handle exception" )]
        public void TryGetServiceShouldHandleException()
        {
            var mock = new Mock<IServiceProvider>();
            mock.Setup( sl => sl.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );
            var target = mock.Object;
            object actual = null;

            Assert.False( target.TryGetService( typeof( IServiceProvider ), out actual ) );
            Assert.Null( actual );
        }

        [Fact( DisplayName = "try get service should handle exception" )]
        public void TryGetServiceOfTShouldHandleException()
        {
            var mock = new Mock<IServiceProvider>();
            mock.Setup( sl => sl.GetService( It.IsAny<Type>() ) ).Throws( new Exception() );
            var target = mock.Object;
            IServiceProvider actual = null;

            Assert.False( target.TryGetService( out actual ) );
            Assert.Null( actual );
        }

        [Fact( DisplayName = "get services should return expected instances" )]
        public void GetServicesShouldReturnExpectedInstances()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { new object(), new object(), new object() };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var actual = serviceProvider.Object.GetServices( typeof( object ) );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Times.Once() );
        }

        [Fact( DisplayName = "get services should return expected instances" )]
        public void GetServicesOfTShouldReturnExpectedInstances()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var expected = new[] { new object(), new object(), new object() };

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( expected.ToArray() );

            // act
            var actual = serviceProvider.Object.GetServices<object>();

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
            serviceProvider.Verify( sp => sp.GetService( typeof( IEnumerable<object> ) ), Times.Once() );
        }

        [Fact( DisplayName = "get services should return empty sequence" )]
        public void GetServicesShouldReturnEmptySequence()
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

        [Fact( DisplayName = "get services should return empty sequence" )]
        public void GetServicesOfTShouldReturnEmptySequence()
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
