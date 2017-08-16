namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class UnitOfWorkTest : IDisposable
    {
        [Fact]
        public void create_should_return_new_unit_of_work()
        {
            // arrange

            // act
            var item1 = UnitOfWork.Create<object>();
            var item2 = UnitOfWork.Create<object>();

            // assert
            item1.Should().NotBeNull().And.Should().NotBeSameAs( item2 );
        }

        [Fact]
        public void get_current_should_return_singleton_instance()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();

            // act
            var item1 = UnitOfWork.GetCurrent<object>();
            var item2 = UnitOfWork.GetCurrent<object>();

            // assert
            item1.Should().NotBeNull();
            item1.Should().BeSameAs( item2 );
        }

        [Fact]
        public void set_current_should_replace_current_unit_of_work()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();

            var item1 = UnitOfWork.Create<object>();

            // act
            UnitOfWork.SetCurrent( item1 );

            var item2 = UnitOfWork.GetCurrent<object>();

            // assert
            item1.Should().NotBeNull();
            item1.Should().BeSameAs( item2 );
        }

        [Fact]
        public void new_current_should_create_and_update_current_unit_of_work()
        {
            // arrange
            UnitOfWork.Provider = CreateUnitOfWorkProvider();

            // act
            var item1 = UnitOfWork.GetCurrent<object>();
            var item2 = UnitOfWork.NewCurrent<object>();

            // assert
            item1.Should().NotBeNull().And.Should().NotBeSameAs( item2 );
        }

        [Fact]
        public void commit_async_should_throw_exception_when_uncommitable()
        {
            // arrange
            var unitOfWork = UnitOfWork.Create<object>();

            // act
            Func<Task> commitAsync = unitOfWork.CommitAsync;

            // assert
            commitAsync.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void provider_should_write_expected_value()
        {
            // arrange
            var provider = new Mock<IUnitOfWorkFactoryProvider>().Object;
            var current = UnitOfWork.Provider;

            // act
            UnitOfWork.Provider = provider;

            // assert
            UnitOfWork.Provider.Should().Be( provider ).And.Should().NotBe( current );
        }

        [Fact]
        public void get_factory_should_throw_exception_when_specification_is_ambiguous()
        {
            // arrange
            var factory1 = new Mock<IUnitOfWorkFactory>();
            var factory2 = new Mock<IUnitOfWorkFactory>();
            var provider = new Mock<IUnitOfWorkFactoryProvider>();
            var ambiguousSpecification = new Specification<Type>( t => true );

            factory1.SetupGet( f => f.Specification ).Returns( ambiguousSpecification );
            factory2.SetupGet( f => f.Specification ).Returns( ambiguousSpecification );
            provider.SetupGet( p => p.Factories ).Returns( ( new[] { factory1.Object, factory2.Object } ).AsEnumerable() );

            UnitOfWork.Provider = provider.Object;

            // act
            Action create = () => UnitOfWork.Create<object>();

            // assert
            create.ShouldThrow<InvalidOperationException>();
        }

        readonly IUnitOfWorkFactoryProvider originalProvider;

        public UnitOfWorkTest() => originalProvider = UnitOfWork.Provider;

        public void Dispose()
        {
            UnitOfWork.Provider = originalProvider;
            GC.SuppressFinalize( this );
        }

        static IUnitOfWorkFactoryProvider CreateUnitOfWorkProvider()
        {
            var factory = new Mock<IUnitOfWorkFactory>();
            var provider = new Mock<IUnitOfWorkFactoryProvider>();
            var specification = new Specification<Type>( t => typeof( object ).Equals( t ) );
            var current = new Mock<IUnitOfWork<object>>().Object;

            factory.SetupGet( f => f.Specification ).Returns( specification );
            factory.Setup( f => f.Create<object>() ).Returns( () => new Mock<IUnitOfWork<object>>().Object );
            factory.Setup( f => f.GetCurrent<object>() ).Returns( () => current );
            factory.Setup( f => f.SetCurrent( It.IsAny<IUnitOfWork<object>>() ) )
                   .Callback<IUnitOfWork<object>>( u => current = u );

            provider.SetupGet( p => p.Factories ).Returns( new[] { factory.Object } );

            return provider.Object;
        }
    }
}