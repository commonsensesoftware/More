namespace More
{
    using System;
    using System.Collections;
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
                    new Tuple<Type, string, object>( typeof( Service1 ), null, new Service1() )
                };

                yield return new object[] { services, typeof( Service1 ), new[] { services[0].Item3, services[2].Item3 } };
                yield return new object[] { services, typeof( Service2 ), new[] { services[1].Item3 } };
            }
        }

        public static IEnumerable<object[]> GetServicesWithKeyData
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

                yield return new object[] { services, typeof( Service1 ), null, new[] { services[0].Item3, services[3].Item3 } };
                yield return new object[] { services, typeof( Service2 ), null, new[] { services[1].Item3 } };
                yield return new object[] { services, typeof( Service3 ), "Keyed", new[] { services[2].Item3, services[4].Item3 } };
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

        [Theory]
        [MemberData( nameof( NullResolveFuncData ) )]
        public void new_service_provider_adapter_should_not_allow_null_resolve_function( Action<Func<Type, string, object>> test )
        {
            // arrange
            Func<Type, string, object> resolve = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( resolve ) );

            // assert
            Assert.Equal( "resolve", ex.ParamName );
        }

        [Fact]
        public void new_service_provider_adapter_should_not_allow_null_resolve_all_function()
        {
            // arrange
            Func<Type, string, IEnumerable<object>> resolveAll = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new ServiceProviderAdapter( ( t, s ) => null, resolveAll ) );

            // assert
            Assert.Equal( "resolveAll", ex.ParamName );
        }

        [Fact]
        public void get_service_should_not_all_null_service_type()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => new object() );
            Type serviceType = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => serviceProvider.GetService( serviceType ) );

            // assert
            Assert.Equal( "serviceType", ex.ParamName );
        }

        [Fact]
        public void get_service_should_return_self_when_requested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );

            // act
            var actual = serviceProvider.GetService( typeof( IServiceProvider ) );

            // assert
            Assert.Same( serviceProvider, actual );
        }

        [Fact]
        public void get_services_should_return_self_when_requested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );
            var expected = new object[] { serviceProvider };

            // act
            var actual = serviceProvider.GetServices( typeof( IServiceProvider ) );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Theory]
        [MemberData( nameof( GetServiceData ) )]
        public void get_service_should_return_expected_object( IReadOnlyList<Tuple<Type, string, object>> services, Type serviceType, object expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => services.Where( i => i.Item1 == t && i.Item2 == s ).Select( i => i.Item3 ).FirstOrDefault() );

            // act
            var actual = serviceProvider.GetService( serviceType );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( nameof( GetServicesData ) )]
        public void get_services_should_return_expected_objects( IReadOnlyList<Tuple<Type, string, object>> services, Type serviceType, IEnumerable<object> expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter(
                ( t, k ) => services.Where( i => i.Item1.Equals( t ) && i.Item2 == k ).Select( i => i.Item3 ).FirstOrDefault(),
                ( t, k ) => services.Where( i => i.Item1.Equals( t ) && i.Item2 == k ).Select( i => i.Item3 ) );

            // act
            var actual = serviceProvider.GetServices( serviceType );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Theory]
        [MemberData( nameof( GetServicesWithKeyData ) )]
        public void get_services_with_key_should_return_expected_objects( IReadOnlyList<Tuple<Type, string, object>> services, Type serviceType, string key, IEnumerable<object> expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter(
                ( t, k ) => services.Where( i => i.Item1.Equals( t ) && i.Item2 == k ).Select( i => i.Item3 ).FirstOrDefault(),
                ( t, k ) => services.Where( i => i.Item1.Equals( t ) && i.Item2 == k ).Select( i => i.Item3 ) );

            // act
            var actual = serviceProvider.GetServices( serviceType, key );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }
    }
}
