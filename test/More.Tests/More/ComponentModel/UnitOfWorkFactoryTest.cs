namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using Xunit;

    public class UnitOfWorkFactoryTest
    {
        [Theory]
        [InlineData( typeof( object ), true )]
        [InlineData( typeof( string ), false )]
        public void specification_should_only_match_registered_item_type( Type itemType, bool expected )
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>().Object;
            var factory = new MockUnitOfWorkFactory();

            factory.InvokeRegisterFactoryMethod( () => unitOfWork );

            // act
            var result = factory.Specification.IsSatisfiedBy( itemType );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void create_should_return_transient_object()
        {
            // arrange
            var factory = new MockUnitOfWorkFactory();

            factory.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );

            var first = factory.Create<object>();

            // act
            var second = factory.Create<object>();

            // assert
            second.Should().NotBeSameAs( first );
        }

        [Fact]
        public void get_current_should_return_same_object()
        {
            // arrange
            var factory = new MockUnitOfWorkFactory();

            factory.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );

            var first = factory.GetCurrent<object>();

            // act
            var second = factory.GetCurrent<object>();

            // assert
            second.Should().BeSameAs( first );
        }

        [Fact]
        public void set_current_should_replace_current_instance()
        {
            // arrange
            var factory = new MockUnitOfWorkFactory();

            factory.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );

            var original = factory.Create<object>();
            var first = factory.GetCurrent<object>();

            // act
            factory.SetCurrent( original );
            var second = factory.GetCurrent<object>();

            // assert
            second.Should().BeSameAs( original );
            second.Should().NotBeSameAs( first );
        }

        sealed class MockUnitOfWorkFactory : UnitOfWorkFactory
        {
            internal void InvokeRegisterFactoryMethod<TItem>( Func<IUnitOfWork<TItem>> factory ) where TItem : class => RegisterFactoryMethod( factory );
        }
    }
}