namespace More
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;
    using static System.StringComparison;
    using static System.UriComponents;
    using static System.UriFormat;

    public class UriComparerTest
    {
        [Fact]
        public void uri_comparer_should_set_expected_default_properties()
        {
            // arrange
            var comparer = new UriComparer();

            // act

            // assert
            comparer.ShouldBeEquivalentTo( new { UriComponents = AbsoluteUri, UriFormat = Unescaped, IgnoreCase = true } );
        }

        [Theory]
        [InlineData( Fragment, SafeUnescaped, true )]
        [InlineData( Host, UriEscaped, false )]
        public void uri_comparer_should_set_expected_properties( UriComponents components, UriFormat format, bool ignoreCase )
        {
            // arrange
            var comparer = new UriComparer( components, format, ignoreCase );

            // act

            // assert
            comparer.ShouldBeEquivalentTo( new { UriComponents = components, UriFormat = format, IgnoreCase = ignoreCase } );
        }

        [Fact]
        public void uri_comparer_generic_and_nonX2Dgeneric_methods_should_return_same_comparison()
        {
            // arrange
            var comparer = new UriComparer();
            IComparer nonGenericComparer = comparer;
            IComparer<Uri> genericComparer = comparer;
            var uri1 = new Uri( "about:blank" );
            var uri2 = new Uri( "http://www.tempuri.org" );

            // act
            var result1 = nonGenericComparer.Compare( uri1, uri2 );
            var result2 = genericComparer.Compare( uri1, uri2 );

            // assert
            result1.Should().Be( result2 );
        }

        [Fact]
        public void uri_comparer_generic_and_nonX2Dgeneric_method_should_return_same_equality()
        {
            // arrange
            var comparer = new UriComparer();
            IEqualityComparer nonGenericComparer = comparer;
            IEqualityComparer<Uri> genericComparer = comparer;
            var uri1 = new Uri( "about:blank" );
            var uri2 = new Uri( "http://www.tempuri.org" );

            // act
            var result1 = nonGenericComparer.Equals( uri1, uri2 );
            var result2 = genericComparer.Equals( uri1, uri2 );

            // assert
            result1.Should().Be( result2 );
        }

        [Fact]
        public void uri_comparer_generic_and_nonX2Dgeneric_method_should_return_same_hash_code()
        {
            // arrange
            var comparer = new UriComparer();
            IEqualityComparer nonGenericComparer = comparer;
            IEqualityComparer<Uri> genericComparer = comparer;
            var uri = new Uri( "http://www.tempuri.org" );

            // act
            var result1 = nonGenericComparer.GetHashCode( uri );
            var result2 = genericComparer.GetHashCode( uri );

            // assert
            result1.Should().Be( result2 );
        }

        [Theory]
        [InlineData( AbsoluteUri, Unescaped, null, null, true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", null, true )]
        [InlineData( AbsoluteUri, Unescaped, null, "about:Blank", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "about:Blank", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "http://www.tempuri.org", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "about:Blank", false )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "http://www.tempuri.org", false )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "about:Blank", true )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "http://www.tempuri.org", true )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "about:Blank", false )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "http://www.tempuri.org", false )]
        public void compare_should_equivalent_to_uri_compare( UriComponents components, UriFormat format, string urlString1, string urlString2, bool ignoreCase )
        {
            // arrange
            var comparer = new UriComparer( components, format, ignoreCase );
            var comparison = ignoreCase ? OrdinalIgnoreCase : Ordinal;
            var url1 = urlString1 == null ? null : new Uri( urlString1 );
            var url2 = urlString2 == null ? null : new Uri( urlString2 );

            // act
            var result1 = comparer.Compare( url1, url2 );
            var result2 = Uri.Compare( url1, url2, components, format, comparison );

            // assert
            result1.Should().Be( result2 );
        }

        [Theory]
        [InlineData( AbsoluteUri, Unescaped, null, null, true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", null, true )]
        [InlineData( AbsoluteUri, Unescaped, null, "about:Blank", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "about:Blank", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "http://www.tempuri.org", true )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "about:Blank", false )]
        [InlineData( AbsoluteUri, Unescaped, "about:blank", "http://www.tempuri.org", false )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "about:Blank", true )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "http://www.tempuri.org", true )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "about:Blank", false )]
        [InlineData( Scheme, SafeUnescaped, "about:blank", "http://www.tempuri.org", false )]
        public void equals_should_be_equivalent_to_compare( UriComponents components, UriFormat format, string urlString1, string urlString2, bool ignoreCase )
        {
            // arrange
            var comparer = new UriComparer( components, format, ignoreCase );
            var comparison = ignoreCase ? OrdinalIgnoreCase : Ordinal;
            var url1 = urlString1 == null ? null : new Uri( urlString1 );
            var url2 = urlString2 == null ? null : new Uri( urlString2 );

            // act
            var equals1 = comparer.Equals( url1, url2 );
            var equals2 = Uri.Compare( url1, url2, components, format, comparison ) == 0;

            // assert
            equals1.Should().Be( equals2 );
        }

        [Fact]
        public void get_hash_code_should_return_expected_value()
        {
            // arrange
            var comparer = new UriComparer();
            var uri = new Uri( "about:blank" );
            var expected = uri.GetHashCode();

            // act
            var hashCode = comparer.GetHashCode( uri );

            // assert
            hashCode.Should().Be( expected );
        }

        [Fact]
        public void get_hash_code_should_return_0_for_null()
        {
            // arrange
            var comparer = new UriComparer();

            // act
            var hashCode = comparer.GetHashCode( default( Uri ) );

            // assert
            hashCode.Should().Be( 0 );
        }
    }
}