namespace More
{
    using System;
    using Xunit;

    public class WeakDelegateTest
    {
        public bool Fact1( string arg1 )
        {
            return default( bool );
        }

        public static bool Fact2( string arg1 )
        {
            return default( bool );
        }

        public void Fact3( object arg1 )
        {
        }

        [Fact( DisplayName = "weak delegate should not allow null delegate" )]
        public void ConstructorShouldNotAllowNullDelegate()
        {
            // act
            Delegate strongDelegate = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new WeakDelegate( strongDelegate ) );

            // assert
            Assert.Equal( "strongDelegate", ex.ParamName );
        }

        [Fact( DisplayName = "is match should return false for null" )]
        public void IsMatchShouldReturnFalseForNull()
        {
            Assert.False( new WeakDelegate( new Func<string, bool>( Fact1 ) ).IsMatch( null ) );
        }

        [Fact( DisplayName = "is match should return false for different methods" )]
        public void IsMatchShouldReturnFalseForDifferentMethods()
        {
            Func<string, bool> source = Fact1;
            Func<string, bool> target = Fact2;
            Assert.False( new WeakDelegate( source ).IsMatch( target ) );
        }

        [Fact( DisplayName = "is match should return true for same method" )]
        public void IsMatchShouldReturnForTheSameMethod()
        {
            Func<string, bool> source = Fact1;
            Func<string, bool> target = Fact1;
            Assert.True( new WeakDelegate( source ).IsMatch( target ) );
        }

        [Fact( DisplayName = "is covariant should return true for covariant action" )]
        public void IsCovariantWithMethodShouldReturnTrueIfDelegatesAreCovariant()
        {
            Assert.True( new WeakDelegate( new Action<object>( Fact3 ) ).IsCovariantWithMethod( typeof( string ) ) );
        }

        [Fact( DisplayName = "is covariant should return true for covariant function" )]
        public void IsCovariantWithFunctionShouldReturnTrueIfDelegatesAreCovariant()
        {
            Assert.True( new WeakDelegate( new Func<string, bool>( Fact1 ) ).IsCovariantWithFunction( typeof( bool ), typeof( string ) ) );
        }

        [Fact( DisplayName = "is covariant should return false for incompatible delegate" )]
        public void IsCovariantWithFunctionShouldReturnFalseIfDelegatesAreIncompatible()
        {
            Assert.False( new WeakDelegate( new Action<object>( Fact3 ) ).IsCovariantWithFunction( typeof( bool ), typeof( string ) ) );
        }

        [Fact( DisplayName = "create delegate should reify delegate" )]
        public void CreateDelegateShouldReifyStrongDelegate()
        {
            Func<string, bool> expected = Fact1;
            var target = new WeakDelegate( expected );
            var actual = target.CreateDelegate();

            Assert.NotNull( actual );
            Assert.IsAssignableFrom<Func<string, bool>>( actual );
        }
    }
}
