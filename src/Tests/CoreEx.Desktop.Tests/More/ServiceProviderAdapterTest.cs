namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ServiceProviderAdapter"/>.
    /// </summary>
    public class ServiceProviderAdapterTest
    {
        private sealed class Service1
        {
        }

        private sealed class Service2
        {
        }

        [ServiceKey( "Keyed" )]
        private sealed class Service3
        {
        }

        public static IEnumerable<object[]> GetServiceData
        {
            get
            {
                var services = new[]
                {
                    new Tuple<Type, string, object>( typeof( Service1 ), null, new Service1() ),
                    new Tuple<Type, string, object>( typeof( Service2 ), null, new Service2() ),
                    new Tuple<Type, string, object>( typeof( Service3 ), "Keyed", new Service3() ),
                };

                yield return new object[] { services, typeof( Service1 ), services[0].Item3 };
                yield return new object[] { services, typeof( Service2 ), services[1].Item3 };
                yield return new object[] { services, typeof( Service3 ), services[2].Item3 };
            }
        }

        public static IEnumerable<object[]> GetServicesData
        {
            get
            {
                var services = new[]
                {
                    new Tuple<Type, string, object>( typeof( Service1 ), null, new Service1() ),
                    new Tuple<Type, string, object>( typeof( Service2 ), null, new Service2() ),
                    new Tuple<Type, string, object>( typeof( Service3 ), "Keyed", new Service3() ),
                    new Tuple<Type, string, object>( typeof( Service1 ), null, new Service1() ),
                    new Tuple<Type, string, object>( typeof( Service3 ), "Keyed", new Service3() ),
                };

                yield return new object[] { services, typeof( IEnumerable<Service1> ), new[] { services[0].Item3, services[3].Item3 } };
                yield return new object[] { services, typeof( IEnumerable<Service2> ), new[] { services[1].Item3 } };
                yield return new object[] { services, typeof( IEnumerable<Service3> ), new[] { services[2].Item3, services[4].Item3 } };
            }
        }

        public static IEnumerable<object[]> NullResolveFuncData
        {
            get
            {
                yield return new object[] { new Action<Func<Type, string, object>>( f => new ServiceProviderAdapter( f ) ) };
                yield return new object[] { new Action<Func<Type, string, object>>( f => new ServiceProviderAdapter( f, ( t, s ) => Enumerable.Empty<object>() ) ) };
            }
        }

        [Theory( DisplayName = "new service provider adapter should not allow null resolve function" )]
        [MemberData( "NullResolveFuncData" )]
        public void ConstructorShouldNotAllowNullResolveFunction( Action<Func<Type, string, object>> test )
        {
            // arrange
            Func<Type, string, object> resolve = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( resolve ) );

            // assert
            Assert.Equal( "resolve", ex.ParamName );
        }

        [Fact( DisplayName = "new service provider adapter should not allow null resolve all function" )]
        public void ConstructorShouldNotAllowNullResolveAllFunction()
        {
            // arrange
            Func<Type, string, IEnumerable<object>> resolveAll = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new ServiceProviderAdapter( ( t, s ) => null, resolveAll ) );

            // assert
            Assert.Equal( "resolveAll", ex.ParamName );
        }

        [Fact( DisplayName = "get service should not all null service type" )]
        public void GetServiceShouldNotAllowNullServiceType()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => new object() );
            Type serviceType = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => serviceProvider.GetService( serviceType ) );

            // assert
            Assert.Equal( "serviceType", ex.ParamName );
        }

        [Fact( DisplayName = "get service should return self when requested" )]
        public void GetServiceShouldReturnSelfWhenRequested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );

            // act
            var actual = serviceProvider.GetService( typeof( IServiceProvider ) );

            // assert
            Assert.Same( serviceProvider, actual );
        }

        [Fact( DisplayName = "get services should return self when requested" )]
        public void GetServicesShouldReturnSelfWhenRequested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );
            var expected = new object[] { serviceProvider };

            // act
            var actual = serviceProvider.GetServices( typeof( IServiceProvider ) );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Theory( DisplayName = "get service should return expected object" )]
        [MemberData( "GetServiceData" )]
        public void GetServiceShouldReturnExpectedObject( IReadOnlyList<Tuple<Type, string, object>> services, Type serviceType, object expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => services.Where( i => i.Item1 == t && i.Item2 == s ).Select( i => i.Item3 ).FirstOrDefault() );

            // act
            var actual = serviceProvider.GetService( serviceType );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "get services should return expected objects" )]
        [MemberData( "GetServicesData" )]
        public void GetServicesShouldReturnExpectedObjects( IReadOnlyList<Tuple<Type, string, object>> services, Type serviceType, IEnumerable<object> expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter(
                ( t, s ) => services.Where( i => i.Item1 == t && i.Item2 == s ).Select( i => i.Item3 ).FirstOrDefault(),
                ( t, s ) => services.Where( i => i.Item1 == t && i.Item2 == s ).Select( i => i.Item3 ) );

            // act
            var actual = serviceProvider.GetServices( serviceType );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }
    }
}
