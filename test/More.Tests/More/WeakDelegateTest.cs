namespace More
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class WeakDelegateTest
    {
        [Fact]
        public void weak_delegate_should_not_allow_null_delegate()
        {
            // act
            var strongDelegate = default( Delegate );

            // act
            Action @new = () => new WeakDelegate( strongDelegate );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( strongDelegate ) );
        }

        [Fact]
        public void is_match_should_return_false_for_null()
        {
            // arrange
            var @delegate = new WeakDelegate( new Func<string, bool>( Fact1 ) );

            // act
            var match = @delegate.IsMatch( default( Delegate ) );

            // assert;
            match.Should().BeFalse();
        }

        [Fact]
        public void is_match_should_return_false_for_different_methods()
        {
            // arrange
            Func<string, bool> source = Fact1;
            Func<string, bool> target = Fact2;
            var @delegate = new WeakDelegate( source );

            // act
            var match = @delegate.IsMatch( target );

            // assert
            match.Should().BeFalse();
        }

        [Fact]
        public void is_match_should_return_true_for_same_method()
        {
            // arrange
            Func<string, bool> source = Fact1;
            Func<string, bool> target = Fact1;
            var @delegate = new WeakDelegate( source );

            // act
            var match = @delegate.IsMatch( target );

            // assert
            match.Should().BeTrue();
        }

        [Fact]
        public void is_covariant_should_return_true_for_covariant_action()
        {
            // arrange
            var @delegate = new WeakDelegate( new Action<object>( Fact3 ) );

            // act
            var covariant = @delegate.IsCovariantWithMethod( typeof( string ) );

            // assert
            covariant.Should().BeTrue();
        }

        [Fact]
        public void is_covariant_should_return_true_for_covariant_function()
        {
            // arrange
            var @delegate = new WeakDelegate( new Func<string, bool>( Fact1 ) );

            // act
            var covariant = @delegate.IsCovariantWithFunction( typeof( bool ), typeof( string ) );

            // assert
            covariant.Should().BeTrue();
        }

        [Fact]
        public void is_covariant_should_return_false_for_incompatible_delegate()
        {
            // arrange
            var @delegate = new WeakDelegate( new Action<object>( Fact3 ) );

            // act
            var covariant = @delegate.IsCovariantWithFunction( typeof( bool ), typeof( string ) );

            // assert
            covariant.Should().BeFalse();
        }

        [Fact]
        public void create_delegate_should_reify_delegate()
        {
            // arrange
            Func<string, bool> original = Fact1;
            var @delegate = new WeakDelegate( original );

            // act
            var reified = @delegate.CreateDelegate();

            // assert
            reified.Should().NotBeNull().And.BeAssignableTo<Func<string, bool>>();
        }

        bool Fact1( string arg1 ) => default( bool );

        static bool Fact2( string arg1 ) => default( bool );

        void Fact3( object arg1 ) { }
    }
}