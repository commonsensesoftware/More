namespace System.ComponentModel
{
    using Moq;
    using System;
    using System.ComponentModel.Design;
    using Xunit;
    using static Moq.Times;

    public class IServiceContainerExtensionsTest
    {
        [Fact]
        public void add_service_should_register_callback()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo() );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), false ), Once() );
        }

        [Fact]
        public void add_service_should_register_callback_with_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo(), true );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), true ), Once() );
        }

        [Fact]
        public void add_service_should_register_instance()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            IFoo foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService( foo );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Once() );
        }

        [Fact]
        public void add_service_should_register_instance_with_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            IFoo foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService( foo, true );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Once() );
        }

        [Fact]
        public void add_service_should_register_instance_with_type_mapping()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService<IFoo, Foo>( foo );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Once() );
        }

        [Fact]
        public void add_service_should_register_instance_with_type_mapping_and_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<bool>() ) );

            // act
            container.Object.AddService<IFoo, Foo>( foo, true );

            // assert
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Once() );
        }

        [Fact]
        public void remove_service_should_unregister_service_type()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.RemoveService<IServiceProvider>();

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), false ), Once() );
        }

        [Fact]
        public void remove_service_should_unregister_service_type_with_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.RemoveService<IServiceProvider>( true );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), true ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_callback()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService<IFoo, Foo>( ( t, sc ) => new Foo() );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IFoo ), false ), Once() );
            container.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), false ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_callback_with_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService<IFoo, Foo>( ( t, sc ) => new Foo(), true );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IFoo ), true ), Once() );
            container.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), true ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService( foo );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( Foo ), false ), Once() );
            container.Verify( sc => sc.AddService( typeof( Foo ), foo, false ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance_with_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService( foo, true );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( Foo ), true ), Once() );
            container.Verify( sc => sc.AddService( typeof( Foo ), foo, true ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance_with_type_mapping()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService<IFoo, Foo>( foo, false );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IFoo ), false ), Once() );
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, false ), Once() );
        }

        [Fact]
        public void replace_service_should_reX2Dregister_instance_with_type_mapping_and_promotion()
        {
            // arrange
            var container = new Mock<IServiceContainer>();
            var foo = new Foo();

            container.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );
            container.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            container.Object.ReplaceService<IFoo, Foo>( foo, true );

            // assert
            container.Verify( sc => sc.RemoveService( typeof( IFoo ), true ), Once() );
            container.Verify( sc => sc.AddService( typeof( IFoo ), foo, true ), Once() );
        }

        interface IFoo { }

        sealed class Foo : IFoo { }
    }
}