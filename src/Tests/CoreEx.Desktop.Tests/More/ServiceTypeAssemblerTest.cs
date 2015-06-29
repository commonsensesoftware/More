namespace More
{
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// Provides unit tests for <see cref="ServiceTypeGenerator"/>.
    /// </summary>
    public class ServiceTypeAssemblerTest
    {
        [ServiceKey( "Gregorian" )]
        public sealed class MockCalendarService
        {
        }

        [Fact( DisplayName = "apply key should not replace attribute" )]
        public void ApplyKeyShouldNotReplaceExplicitAttributeDeclaration()
        {
            var assembler = new ServiceTypeAssembler();
            var expected = "Gregorian";
            var type = assembler.ApplyKey( typeof( MockCalendarService ), "Fiscal" );
            var actual = type.GetCustomAttribute<ServiceKeyAttribute>( false );

            Assert.NotNull( actual );
            Assert.Equal( expected, actual.Key );
        }

        [Theory( DisplayName = "apply key should return projected type" )]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        [InlineData( typeof( Lazy<IEnumerable<ICalendarProvider>> ), "Gregorian" )]
        public void ApplyKeyShouldReturnProjectedType( Type serviceType, string key )
        {
            var assembler = new ServiceTypeAssembler();
            var type = assembler.ApplyKey( serviceType, key );

            // unwrap nested generics
            while ( type.IsGenericType )
                type = type.GenericTypeArguments[0];

            var actual = type.GetCustomAttribute<ServiceKeyAttribute>( false );

            Assert.NotNull( actual );
            Assert.Equal( key, actual.Key );
        }

        [Theory( DisplayName = "extract key should return value from decorated type" )]
        [InlineData( typeof( MockCalendarService ), "Gregorian" )]
        [InlineData( typeof( ICalendarProvider ), null )]
        [InlineData( typeof( ICalendarProvider ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), "Fiscal" )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ), "Fiscal" )]
        public void ExtractKeyShouldReturnValueFromDecoratedTypes( Type serviceType, string key )
        {
            var assembler = new ServiceTypeAssembler();
            var type = assembler.ApplyKey( serviceType, key );
            var actual = assembler.ExtractKey( type );

            Assert.Equal( key, actual );
        }

        [Theory( DisplayName = "extract key should return null for undecorated type" )]
        [InlineData( typeof( ICalendarProvider ) )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IEnumerable<Lazy<ICalendarProvider>> ) )]
        public void ExtractKeyShouldReturnNullForUndecoratedType( Type serviceType )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.ExtractKey( serviceType );
            Assert.Null( actual );
        }

        [Theory( DisplayName = "for many should return expected type" )]
        [InlineData( typeof( ICalendarProvider ), typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), typeof( IEnumerable<ICalendarProvider> ) )]
        [InlineData( typeof( List<Lazy<ICalendarProvider>> ), typeof( List<Lazy<ICalendarProvider>> ) )]
        public void ForManyShouldReturnExpectedType( Type serviceType, Type expected )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.ForMany( serviceType );
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "is for many should return expected result" )]
        [InlineData( typeof( ICalendarProvider ), false )]
        [InlineData( typeof( IEnumerable<ICalendarProvider> ), true )]
        [InlineData( typeof( List<Lazy<ICalendarProvider>> ), true )]
        public void IsForManyShouldReturnExpectedResult( Type serviceType, bool expected )
        {
            var assembler = new ServiceTypeAssembler();
            var actual = assembler.IsForMany( serviceType );
            Assert.Equal( expected, actual );
        }
    }
}
