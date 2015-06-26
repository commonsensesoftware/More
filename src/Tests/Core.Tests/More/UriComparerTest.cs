namespace More
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="UriComparer"/> class.
    /// </summary>
    public class UriComparerTest
    {
        [Fact( DisplayName = "uri comparer should set expected default properties" )]
        public void ConstructorsShouldSetExpectedDefaultPropertyValue()
        {
            // arrange
            var target = new UriComparer();

            // act

            // assert
            Assert.Equal( UriComponents.AbsoluteUri, target.UriComponents );
            Assert.Equal( UriFormat.Unescaped, target.UriFormat );
            Assert.True( target.IgnoreCase );
        }

        [Theory( DisplayName = "uri comparer should set expected properties" )]
        [InlineData( UriComponents.Fragment, UriFormat.SafeUnescaped, true )]
        [InlineData( UriComponents.Host, UriFormat.UriEscaped, false )]
        public void ConstructorsShouldSetExpectedProperties( UriComponents components, UriFormat format, bool ignoreCase )
        {
            // arrange
            var target = new UriComparer( components, format, ignoreCase );

            // act
            
            // assert
            Assert.Equal( components, target.UriComponents );
            Assert.Equal( format, target.UriFormat );
            Assert.Equal( ignoreCase, target.IgnoreCase );
        }

        [Fact( DisplayName = "uri comparer generic and non-generic methods should return same comparison" )]
        public void IComparerAndIComparerOfTCompareShouldBeTheSame()
        {
            // arrange
            var target = new UriComparer();
            IComparer c1 = target;
            IComparer<Uri> c2 = target;
            var uri1 = new Uri( "about:blank" );
            var uri2 = new Uri( "http://www.tempuri.org" );
            
            // act
            var r1 = c1.Compare( uri1, uri2 );
            var r2 = c2.Compare( uri1, uri2 );
            
            // assert
            Assert.Equal( r1, r2 );
        }

        [Fact( DisplayName = "uri comparer generic and non-generic method should return same equality" )]
        public void IEqualityComparerAndIEqualityOfTComparerEqualsShouldBeTheSame()
        {
            // arrange
            var target = new UriComparer();
            IEqualityComparer c1 = target;
            IEqualityComparer<Uri> c2 = target;
            var uri1 = new Uri( "about:blank" );
            var uri2 = new Uri( "http://www.tempuri.org" );
            
            // act
            var r1 = c1.Equals( uri1, uri2 );
            var r2 = c2.Equals( uri1, uri2 );
            
            // assert
            Assert.Equal( r1, r2 );
        }

        [Fact( DisplayName = "uri comparer generic and non-generic method should return same hash code" )]
        public void IEqualityComparerAndIEqualityOfTGetHashCodeShouldBeTheSame()
        {
            // arrange
            var target = new UriComparer();
            IEqualityComparer c1 = target;
            IEqualityComparer<Uri> c2 = target;
            var uri = new Uri( "http://www.tempuri.org" );
            
            // act
            var r1 = c1.GetHashCode( uri );
            var r2 = c2.GetHashCode( uri );
            
            // assert
            Assert.Equal( r1, r2 );
        }

        [Fact( DisplayName = "compare should equeal uri compare" )]
        public void CompareShouldEqualUriCompare()
        {
            var target = new UriComparer();
            var url1 = new Uri( "about:blank" );
            var url2 = new Uri( "about:Blank" );
            var url3 = new Uri( "http://www.tempuri.org" );

            Assert.Equal( Uri.Compare( null, null, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( null, null ) );
            Assert.Equal( Uri.Compare( url1, null, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( url1, null ) );
            Assert.Equal( Uri.Compare( null, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( null, url2 ) );

            Assert.Equal( Uri.Compare( url1, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( url1, url3 ) );

            target = new UriComparer( UriComponents.AbsoluteUri, UriFormat.Unescaped, false );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.Ordinal ), target.Compare( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.Ordinal ), target.Compare( url1, url3 ) );

            target = new UriComparer( UriComponents.Scheme, UriFormat.SafeUnescaped );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase ), target.Compare( url1, url3 ) );

            target = new UriComparer( UriComponents.Scheme, UriFormat.SafeUnescaped, false );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.Ordinal ), target.Compare( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.Ordinal ), target.Compare( url1, url3 ) );
        }

        [Fact( DisplayName = "equals should be equivalent to compare" )]
        public void EqualsShouldBeEquivalentToUriCompare()
        {
            var target = new UriComparer();
            var url1 = new Uri( "about:blank" );
            var url2 = new Uri( "about:Blank" );
            var url3 = new Uri( "http://www.tempuri.org" );

            Assert.Equal( Uri.Compare( null, null, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( null, null ) );
            Assert.Equal( Uri.Compare( url1, null, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( url1, null ) );
            Assert.Equal( Uri.Compare( null, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( null, url2 ) );

            Assert.Equal( Uri.Compare( url1, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( url1, url3 ) );

            target = new UriComparer( UriComponents.AbsoluteUri, UriFormat.Unescaped, false );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.Ordinal ) == 0, target.Equals( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.Ordinal ) == 0, target.Equals( url1, url3 ) );

            target = new UriComparer( UriComponents.Scheme, UriFormat.SafeUnescaped );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase ) == 0, target.Equals( url1, url3 ) );

            target = new UriComparer( UriComponents.Scheme, UriFormat.SafeUnescaped, false );
            Assert.Equal( Uri.Compare( url1, url2, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.Ordinal ) == 0, target.Equals( url1, url2 ) );
            Assert.Equal( Uri.Compare( url1, url3, UriComponents.Scheme, UriFormat.SafeUnescaped, StringComparison.Ordinal ) == 0, target.Equals( url1, url3 ) );
        }

        [Fact( DisplayName = "get hash code should return expected value" )]
        public void GetHashCodeShouldReturnCorrectValue()
        {
            // arrange
            var target = new UriComparer();
            var uri = new Uri( "about:blank" );
            var expected = uri.GetHashCode();

            // act
            var actual = target.GetHashCode( uri );
            
            // assert
            Assert.Equal( 0, target.GetHashCode( null ) );
            Assert.Equal( expected, actual );
        }
    }
}
