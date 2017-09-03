namespace More
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ServiceProviderAdapterTest
    {
        [Theory]
        [MemberData( nameof( NullResolveFuncData ) )]
        public void new_service_provider_adapter_should_not_allow_null_resolve_function( Action<Func<Type, string, object>> test )
        {
            // arrange
            var resolve = default( Func<Type, string, object> );

            // act
            Action @new = () => test( resolve );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( resolve ) );
        }

        [Fact]
        public void new_service_provider_adapter_should_not_allow_null_resolve_all_function()
        {
            // arrange
            var resolveAll = default( Func<Type, string, IEnumerable<object>> );

            // act
            Action @new = () => new ServiceProviderAdapter( ( t, s ) => null, resolveAll );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( resolveAll ) );
        }

        [Fact]
        public void get_service_should_not_all_null_service_type()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => new object() );
            var serviceType = default( Type );

            // act
            Action getService = () => serviceProvider.GetService( serviceType );

            // assert
            getService.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( serviceType ) );
        }

        [Fact]
        public void get_service_should_return_self_when_requested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );

            // act
            var service = serviceProvider.GetService( typeof( IServiceProvider ) );

            // assert
            service.Should().BeSameAs( serviceProvider );
        }

        [Fact]
        public void get_services_should_return_self_when_requested()
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, s ) => null );

            // act
            var services = serviceProvider.GetServices( typeof( IServiceProvider ) );

            // assert
            services.Should().Equal( new object[] { serviceProvider } );
        }

        [Theory]
        [MemberData( nameof( GetServiceData ) )]
        public void get_service_should_return_expected_object( IReadOnlyList<(Type ServiceType, string Key, object Service)> services, Type serviceType, object expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter( ( t, k ) => services.Where( i => i.ServiceType == t && i.Key == k ).Select( i => i.Service ).FirstOrDefault() );

            // act
            var service = serviceProvider.GetService( serviceType );

            // assert
            service.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( GetServicesData ) )]
        public void get_services_should_return_expected_objects( IReadOnlyList<(Type ServiceType, string Key, object Service)> allServices, Type serviceType, IEnumerable<object> expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter(
                ( t, k ) => allServices.Where( i => i.ServiceType.Equals( t ) && i.Key == k ).Select( i => i.Service ).FirstOrDefault(),
                ( t, k ) => allServices.Where( i => i.ServiceType.Equals( t ) && i.Key == k ).Select( i => i.Service ) );

            // act
            var services = serviceProvider.GetServices( serviceType );

            // assert
            services.Should().BeEquivalentTo( expected );
        }

        [Theory]
        [MemberData( nameof( GetServicesWithKeyData ) )]
        public void get_services_with_key_should_return_expected_objects( IReadOnlyList<(Type ServiceType, string Key, object Service)> allServices, Type serviceType, string key, IEnumerable<object> expected )
        {
            // arrange
            var serviceProvider = new ServiceProviderAdapter(
                ( t, k ) => allServices.Where( i => i.ServiceType.Equals( t ) && i.Key == k ).Select( i => i.Service ).FirstOrDefault(),
                ( t, k ) => allServices.Where( i => i.ServiceType.Equals( t ) && i.Key == k ).Select( i => i.Service ) );

            // act
            var services = serviceProvider.GetServices( serviceType, key );

            // assert
            services.Should().BeEquivalentTo( expected );
        }

        sealed class Service1 { }

        sealed class Service2 { }

        [ServiceKey( "Keyed" )]
        sealed class Service3 { }

        public static IEnumerable<object[]> GetServiceData
        {
            get
            {
                var services = new[]
                {
                    ( typeof( Service1 ), null, new Service1() ),
                    ( typeof( Service2 ), null, new Service2() ),
                    ( typeof( Service3 ), "Keyed", (object) new Service3() )
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
                    ( typeof( Service1 ), null, new Service1() ),
                    ( typeof( Service2 ), null, new Service2() ),
                    ( typeof( Service1 ), default(string), (object) new Service1() )
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
                    ( typeof( Service1 ), null, new Service1() ),
                    ( typeof( Service2 ), null, new Service2() ),
                    ( typeof( Service3 ), "Keyed", new Service3() ),
                    ( typeof( Service1 ), null, new Service1() ),
                    ( typeof( Service3 ), "Keyed", (object) new Service3() ),
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
    }
}