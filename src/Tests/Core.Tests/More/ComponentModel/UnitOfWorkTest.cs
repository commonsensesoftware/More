namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="UnitOfWork"/>.
    /// </summary>
    public class UnitOfWorkTest : IDisposable
    {
        private readonly IUnitOfWorkFactoryProvider originalProvider;

        public UnitOfWorkTest()
        {
            originalProvider = UnitOfWork.Provider;
        }

        public void Dispose()
        {
            UnitOfWork.Provider = originalProvider;
            GC.SuppressFinalize( this );
        }

        private static IUnitOfWorkFactoryProvider CreateUnitOfWorkProvider()
        {
            var factory = new Mock<IUnitOfWorkFactory>();
            var provider = new Mock<IUnitOfWorkFactoryProvider>();
            var specification = new Specification<Type>( t => typeof( object ).Equals( t ) );
            var current = new Mock<IUnitOfWork<object>>().Object;

            factory.SetupGet( f => f.Specification ).Returns( specification );
            factory.Setup( f => f.Create<object>() ).Returns( () => new Mock<IUnitOfWork<object>>().Object );
            factory.Setup( f => f.GetCurrent<object>() ).Returns( () => current );
            factory.Setup( f => f.SetCurrent<object>( It.IsAny<IUnitOfWork<object>>() ) )
                   .Callback<IUnitOfWork<object>>( u => current = u );

            provider.SetupGet( p => p.Factories ).Returns( new[] { factory.Object } );

            return provider.Object;
        }

        [Fact( DisplayName = "create should return new unit of work" )]
        public void CreateShouldCreateNewUnitOfWork()
        {
            // arrange

            // act
            var a1 = UnitOfWork.Create<object>();
            var a2 = UnitOfWork.Create<object>();

            // assert
            Assert.NotNull( a1 );
            Assert.NotNull( a2 );
            Assert.NotEqual( a1, a2 );
        }

        [Fact( DisplayName = "get current should return singleton instance" )]
        public void GetCurrentShouldCreateSingleInstance()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();
            
            // act
            var a1 = UnitOfWork.GetCurrent<object>();
            var a2 = UnitOfWork.GetCurrent<object>();

            // assert
            Assert.NotNull( a1 );
            Assert.NotNull( a2 );
            Assert.Equal( a1, a2 );
        }

        [Fact( DisplayName = "set current should replace current unit of work" )]
        public void SetCurrentShouldUpdateCurrentUnitOfWork()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();

            var a1 = UnitOfWork.Create<object>();

            // act
            UnitOfWork.SetCurrent( a1 );

            var a2 = UnitOfWork.GetCurrent<object>();

            // assert
            Assert.NotNull( a1 );
            Assert.NotNull( a2 );
            Assert.Equal( a1, a2 );
        }

        [Fact( DisplayName = "new current should create and update current unit of work" )]
        public void NewCurrentShouldUpdateCurrentUnitOfWork()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();

            // act
            var a1 = UnitOfWork.GetCurrent<object>();
            var a2 = UnitOfWork.NewCurrent<object>();

            // assert
            Assert.NotNull( a1 );
            Assert.NotNull( a2 );
            Assert.NotEqual( a1, a2 );
        }

        [Fact( DisplayName = "commit async should throw exception when uncommitable" )]
        public async Task UncommitableUnitOfWorkShouldThrowExceptionOnCommit()
        {
            // arrange
            var target = UnitOfWork.Create<object>();

            // act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>( () => target.CommitAsync() );

            // assert
        }

        [Fact( DisplayName = "provider should write expected value" )]
        public void ProviderPropertyShouldWriteExpectedValue()
        {
            // arrange
            var provider = new Mock<IUnitOfWorkFactoryProvider>().Object;
            var current = UnitOfWork.Provider;

            // act
            UnitOfWork.Provider = provider;

            // assert
            Assert.NotEqual( current, UnitOfWork.Provider );
            Assert.Equal( provider, UnitOfWork.Provider );
        }

        [Fact( DisplayName = "get factory should throw exception when specification is ambiguous" )]
        public void GetFactoryShouldThrowExceptionWhenMoreThanOneInstanceIsFound()
        {
            // arrange
            var factory1 = new Mock<IUnitOfWorkFactory>();
            var factory2 = new Mock<IUnitOfWorkFactory>();
            var provider = new Mock<IUnitOfWorkFactoryProvider>();
            var specification = new Specification<Type>( t => true );

            factory1.SetupGet( f => f.Specification ).Returns( specification );
            factory2.SetupGet( f => f.Specification ).Returns( specification );
            provider.SetupGet( p => p.Factories ).Returns( ( new[] { factory1.Object, factory2.Object } ).AsEnumerable() );

            // act
            UnitOfWork.Provider = provider.Object;

            // assert
            Assert.Throws<InvalidOperationException>( () => UnitOfWork.Create<object>() );
        }
    }
}
