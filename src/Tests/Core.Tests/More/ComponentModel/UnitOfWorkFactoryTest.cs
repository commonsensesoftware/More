namespace More.ComponentModel
{
    using Moq;
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="UnitOfWorkFactory"/>.
    /// </summary>
    public class UnitOfWorkFactoryTest
    {
        private sealed class MockUnitOfWorkFactory : UnitOfWorkFactory
        {
            internal MockUnitOfWorkFactory()
            {
            }

            internal void InvokeRegisterFactoryMethod<TItem>( Func<IUnitOfWork<TItem>> factory ) where TItem : class
            {
                this.RegisterFactoryMethod( factory );
            }
        }

        [Fact( DisplayName = "specification should match registered item type" )]
        public void SpecificationShouldMatchRegisteredItemType()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>().Object;
            var target = new MockUnitOfWorkFactory();

            target.InvokeRegisterFactoryMethod( () => unitOfWork );

            // act
            var actual = target.Specification.IsSatisfiedBy( typeof( object ) );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "specification should not match unregistered item type" )]
        public void SpecificationShouldNotMatchUnregisteredItemType()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>().Object;
            var target = new MockUnitOfWorkFactory();
            
            target.InvokeRegisterFactoryMethod( () => unitOfWork );
            
            // act
            var actual = target.Specification.IsSatisfiedBy( typeof( string ) );

            // assert
            Assert.False( actual );
        }

        [Fact( DisplayName = "create should return transient object" )]
        public void CreateShouldReturnTransientObject()
        {
            // arrange
            var target = new MockUnitOfWorkFactory();
            
            target.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );
            
            var expected = target.Create<object>();
            
            // act
            var actual = target.Create<object>();
            
            // assert
            Assert.NotNull( expected );
            Assert.NotNull( actual );
            Assert.NotEqual( expected, actual );
        }

        [Fact( DisplayName = "get current should return same object" )]
        public void GetCurrentShouldReturnExpectedResult()
        {
            // arrange
            var target = new MockUnitOfWorkFactory();

            target.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );
            
            var expected = target.GetCurrent<object>();
            
            // act
            var actual = target.GetCurrent<object>();

            // assert
            Assert.NotNull( expected );
            Assert.NotNull( actual );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "set current should replace current instance" )]
        public void SetCurrentShouldReplaceCurrentInstance()
        {
            // arrange
            var target = new MockUnitOfWorkFactory();

            target.InvokeRegisterFactoryMethod( () => new Mock<IUnitOfWork<object>>().Object );

            var expected = target.Create<object>();
            var current = target.GetCurrent<object>();

            // act
            target.SetCurrent( expected );
            var actual = target.GetCurrent<object>();

            // assert
            Assert.Equal( expected, actual );
            Assert.NotEqual( current, actual );
        }
    }
}
