namespace More
{
    using System;
    using Xunit;

    public class WeakDelegateTTest
    {
        public bool Fact1( string arg1 )
        {
            return default( bool );
        }

        [Fact( DisplayName = "weak delegate should not allow null delegate" )]
        public void ConstructorShouldNotAllowNullDelegate()
        {
            // arrange
            Delegate strongDelegate = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new WeakDelegate<Delegate>( strongDelegate ) );

            // assert
            Assert.Equal( "strongDelegate", ex.ParamName );
        }

        [Fact( DisplayName = "weak delegate should not allow non-delegate" )]
        public void ConstructorShouldNotAllowNonDelegateType()
        {
            // arrange
            var strongDelegate = "Test";

            // act
            var ex = Assert.Throws<ArgumentException>( () => new WeakDelegate<string>( strongDelegate ) );

            // assert
            Assert.Equal( "strongDelegate", ex.ParamName );
        }

        [Fact( DisplayName = "create typed delegate should reify delegate" )]
        public void CreateTypedDelegateShouldReifyDelegate()
        {
            Func<string, bool> expected = Fact1;
            var target = new WeakDelegate<Func<string, bool>>( expected );
            var actual = target.CreateTypedDelegate();
            Assert.Equal( expected, actual );
        }
    }
}
