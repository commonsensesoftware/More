namespace System.ComponentModel
{
    using Moq;
    using System;
    using System.ComponentModel.Design;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IServiceContainerExtensions"/>.
    /// </summary>
    public class IServiceContainerExtensionsTest
    {
        private interface IFoo
        {
        }

        private sealed class Foo : IFoo
        {
        }

        [Fact]
        public void add_service_should_register_callback()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo() );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), false ), Times.Once() );
        }

        [Fact]
        public void add_service_should_register_callback_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo(), true );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), true ), Times.Once() );
        }

        [Fact]
        public void add_service_should_register_instance()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            IFoo foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService( foo );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Times.Once() );
        }

        [Fact]
        public void add_service_should_register_instance_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            IFoo foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService( foo, true );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Times.Once() );
        }

        [Fact]
        public void add_service_should_register_instance()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( foo );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Times.Once() );
        }

        [Fact]
        public void add_service_should_register_instance_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( foo, true );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Times.Once() );
        }

        [Fact]
        public void remove_service_should_unregister_service_type()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.RemoveService<IServiceProvider>();

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), false ), Times.Once() );
        }

        [Fact]
        public void remove_service_should_unregister_service_type_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.RemoveService<IServiceProvider>( true );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), true ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_callback()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService<IFoo, Foo>( ( t, sc ) => new Foo() );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IFoo ), false ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), false ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_callback_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService<IFoo, Foo>( ( t, sc ) => new Foo(), true );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IFoo ), true ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), true ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService( foo );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( Foo ), false ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( Foo ), foo, false ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService( foo, true );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( Foo ), true ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( Foo ), foo, true ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService<IFoo, Foo>( foo, false );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IFoo ), false ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Times.Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance_with_promotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();
            var foo = new Foo();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.ReplaceService<IFoo, Foo>( foo, true );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IFoo ), true ), Times.Once() );
            target.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Times.Once() );
        }
    }
}
