namespace More
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class WeakDelegateTTest
    {
        [Fact]
        public void weak_delegate_should_not_allow_null_delegate()
        {
            // arrange
            var strongDelegate = default( Delegate );

            // act
            Action @new = () => new WeakDelegate<Delegate>( strongDelegate );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( strongDelegate ) );
        }

        [Fact]
        public void weak_delegate_should_not_allow_nonX2Ddelegate()
        {
            // arrange
            var strongDelegate = "Test";

            // act
            Action @new = () => new WeakDelegate<string>( strongDelegate );

            // assert
            @new.ShouldThrow<ArgumentException>().And.ParamName.Should().Be( nameof( strongDelegate ) );
        }

        [Fact]
        public void create_typed_delegate_should_reify_delegate()
        {
            // arrange
            Func<string, bool> original = Fact1;
            var @delegate = new WeakDelegate<Func<string, bool>>( original );

            // act
            var reified = @delegate.CreateTypedDelegate();

            // assert
            reified.Should().Be( original );
        }

        public bool Fact1( string arg1 ) => default( bool );
    }
}