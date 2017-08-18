namespace More
{
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ServiceTypeGenerator"/>.
    /// </summary>
    public class ServiceTypeAssemblerTest
    {
        [ServiceKey( "Gregorian" )]
        public sealed class MockCalendarService
        {
        }

        [Fact]
        public void apply_key_should_not_replace_attribute()
        {
            var assembler = new ServiceTypeAssembler();
            var expected = "Gregorian";
            var type = assembler.ApplyKey( typeof( MockCalendarService ), "Fiscal" );
            var actual = type.GetCustomAttribute<ServiceKeyAttribute>( false );

            Assert.NotNull( actual );
            Assert.Equal( expected, actual.Key );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        [InlineData( typeof( Lazy<IEnumerable<ICalendarProvider>> ), "Gregorian" )]
        public void apply_key_should_return_projected_type( Type serviceType, string key )
        {
            var assembler = new ServiceTypeAssembler();
            var keyedServiceType = assembler.ApplyKey( serviceType, key );
            var actual = keyedServiceType.GetCustomAttribute<ServiceKeyAttribute>( false );

            Assert.NotNull( actual );
            Assert.Equal( key, actual.Key );
        }

        [Theory]
        [InlineData( typeof( MockCalendarService ), "Gregorian" )]
        [InlineData( typeof( ICalendarProvider ), null )]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        public void extract_key_should_return_value_from_decorated_type( Type serviceType, string key )
        {
            var assembler = new ServiceTypeAssembler();
            var type = assembler.ApplyKey( serviceType, key );
            var actual = assembler.ExtractKey( type );

            Assert.Equal( key, actual );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ) )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ) )]
        public void extract_key_should_return_null_for_undecorated_type( Type serviceType )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.ExtractKey( serviceType );
            Assert.Null( actual );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IDictionary<string, ICalendarProvider> ), typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ) )]
        [InlineData( typeof( Lazy<ICalendarProvider> ), typeof( IEnumerable<Lazy<ICalendarProvider>> ) )]
        public void for_many_should_return_expected_type( Type serviceType, Type expected )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.ForMany( serviceType );
            Assert.Equal( expected, actual );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), false )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), true )]
        [InlineData( typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ), true )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), true )]
        public void is_for_many_should_return_expected_result( Type serviceType, bool expected )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.IsForMany( serviceType );
            Assert.Equal( expected, actual );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), typeof( ICalendarProvider ), false )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), typeof( ICalendarProvider ), true )]
        [InlineData( typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ), typeof( IDictionary<string, ICalendarProvider> ), true )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), typeof( Lazy<ICalendarProvider> ), true )]
        public void is_for_many_should_return_inner_type( Type serviceType, Type innerServiceType, bool shouldBeMany )
        {
            var assembler = new ServiceTypeAssembler();
            Type singleServiceType;
            var result = assembler.IsForMany( serviceType, out singleServiceType );
            Assert.Equal( shouldBeMany, result );
            Assert.Equal( innerServiceType, singleServiceType );
        }
    }
}
