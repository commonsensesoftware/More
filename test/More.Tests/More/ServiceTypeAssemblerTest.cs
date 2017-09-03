namespace More
{
    using FluentAssertions;
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public class ServiceTypeAssemblerTest
    {
        [Fact]
        public void apply_key_should_not_replace_attribute()
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var type = assembler.ApplyKey( typeof( MockCalendarService ), "Fiscal" );
            var attribute = type.GetCustomAttribute<ServiceKeyAttribute>( false );

            // assert
            attribute.Key.Should().Be( "Gregorian" );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        [InlineData( typeof( Lazy<IEnumerable<ICalendarProvider>> ), "Gregorian" )]
        public void apply_key_should_return_projected_type( Type serviceType, string key )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var type = assembler.ApplyKey( serviceType, key );
            var attribute = type.GetCustomAttribute<ServiceKeyAttribute>( false );

            // assert
            attribute.Key.Should().Be( key );
        }

        [Theory]
        [InlineData( typeof( MockCalendarService ), "Gregorian" )]
        [InlineData( typeof( ICalendarProvider ), null )]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        public void extract_key_should_return_value_from_decorated_type( Type serviceType, string key )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var type = assembler.ApplyKey( serviceType, key );
            var result = assembler.ExtractKey( type );

            // assert
            result.Should().Be( key );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ) )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ) )]
        public void extract_key_should_return_null_for_undecorated_type( Type serviceType )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var key = assembler.ExtractKey( serviceType );

            // assert
            key.Should().BeNull();
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IDictionary<string, ICalendarProvider> ), typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ) )]
        [InlineData( typeof( Lazy<ICalendarProvider> ), typeof( IEnumerable<Lazy<ICalendarProvider>> ) )]
        public void for_many_should_return_expected_type( Type serviceType, Type expected )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var result = assembler.ForMany( serviceType );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), false )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), true )]
        [InlineData( typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ), true )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), true )]
        public void is_for_many_should_return_expected_result( Type serviceType, bool expected )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var result = assembler.IsForMany( serviceType );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( typeof( ICalendarProvider ), typeof( ICalendarProvider ), false )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), typeof( ICalendarProvider ), true )]
        [InlineData( typeof( IEnumerable<IDictionary<string, ICalendarProvider>> ), typeof( IDictionary<string, ICalendarProvider> ), true )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), typeof( Lazy<ICalendarProvider> ), true )]
        public void is_for_many_should_return_inner_type( Type serviceType, Type innerServiceType, bool forMany )
        {
            // arrange
            var assembler = new ServiceTypeAssembler();

            // act
            var result = assembler.IsForMany( serviceType, out var singleServiceType );

            // assert
            result.Should().Be( forMany );
            singleServiceType.Should().Be( innerServiceType );
        }

        [ServiceKey( "Gregorian" )]
        public sealed class MockCalendarService { }
    }
}