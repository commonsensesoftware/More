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

        [Fact( DisplayName = "add service should register callback" )]
        public void AddServiceShouldRegisterCallback()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo() );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), false ), Times.Once() );
        }

        [Fact( DisplayName = "add service should register callback with promotion" )]
        public void AddServiceShouldRegisterCallbackWithPromotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.AddService( It.IsAny<Type>(), It.IsAny<ServiceCreatorCallback>(), It.IsAny<bool>() ) );

            // act
            target.Object.AddService<IFoo, Foo>( ( t, sc ) => new Foo(), true );

            // assert
            target.Verify( sc => sc.AddService( typeof( IFoo ), It.IsAny<ServiceCreatorCallback>(), true ), Times.Once() );
        }

        [Fact( DisplayName = "add service should register instance" )]
        public void AddServiceShouldRegisterInstance()
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

        [Fact( DisplayName = "add service should register instance with promotion" )]
        public void AddServiceShouldRegisterInstanceWithPromotion()
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

        [Fact( DisplayName = "add service should register instance" )]
        public void AddServiceShouldRegisterInstanceOfT()
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

        [Fact( DisplayName = "add service should register instance with promotion" )]
        public void AddServiceShouldRegisterInstanceOfTWithPromotion()
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

        [Fact( DisplayName = "remove service should unregister service type" )]
        public void RemoveServiceShouldUnregisterServiceType()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.RemoveService<IServiceProvider>();

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), false ), Times.Once() );
        }

        [Fact( DisplayName = "remove service should unregister service type with promotion" )]
        public void RemoveServiceShouldUnregisterServiceTypeWithPromotion()
        {
            // arrange
            var target = new Mock<IServiceContainer>();

            target.Setup( sc => sc.RemoveService( It.IsAny<Type>(), It.IsAny<bool>() ) );

            // act
            target.Object.RemoveService<IServiceProvider>( true );

            // assert
            target.Verify( sc => sc.RemoveService( typeof( IServiceProvider ), true ), Times.Once() );
        }

        [Fact( DisplayName = "replace service should re-register callback" )]
        public void ReplaceServiceShouldReregisterCallback()
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

        [Fact( DisplayName = "replace service should re-register callback with promotion" )]
        public void ReplaceServiceShouldReregisterCallbackWithPromotion()
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

        [Fact( DisplayName = "replace service should re-register instance" )]
        public void ReplaceServiceShouldReregisterInstance()
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

        [Fact( DisplayName = "replace service should re-register instance with promotion" )]
        public void ReplaceServiceShouldReregisterInstanceWithPromotion()
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

        [Fact( DisplayName = "replace service should re-register instance" )]
        public void ReplaceServiceShouldReregisterInstanceOfT()
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

        [Fact( DisplayName = "replace service should re-register instance with promotion" )]
        public void ReplaceServiceShouldReregisterInstanceOfTWithPromotion()
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
