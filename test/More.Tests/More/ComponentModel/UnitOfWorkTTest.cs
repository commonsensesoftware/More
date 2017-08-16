namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using Moq.Protected;
    using System;
    using Xunit;

    public class UnitOfWorkTTest
    {
        [Fact]
        public void register_new_should_mark_item_for_insert()
        {
            // arrange
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterNew( new object() );

            // assert
            unitOfWork.HasPendingChanges.Should().BeTrue();
            unitOfWork.ShouldRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void register_new_should_not_allow_delete()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var unitOfWork = mock.Object;

            unitOfWork.RegisterRemoved( item );

            // act
            Action registerNew = () => unitOfWork.RegisterNew( item );

            // assert
            registerNew.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void register_new_should_not_allow_update()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var unitOfWork = mock.Object;

            unitOfWork.RegisterChanged( item );

            // act
            Action registerNew = () => unitOfWork.RegisterNew( item );

            // assert
            registerNew.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void register_changed_should_mark_item_for_update()
        {
            // arrange
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterNew( new object() );

            // assert
            unitOfWork.HasPendingChanges.Should().BeTrue();
            unitOfWork.ShouldRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void register_changed_should_not_trigger_update_for_new_item()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var unitOfWork = mock.Object;

            unitOfWork.RegisterNew( item );
            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterChanged( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeTrue();
            unitOfWork.ShouldNotRaisePropertyChangeFor( u => u.HasPendingChanges );

        }

        [Fact]
        public void register_changed_should_not_trigger_update_for_deleted_item()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var unitOfWork = mock.Object;

            unitOfWork.RegisterRemoved( item );
            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterChanged( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeTrue();
            unitOfWork.ShouldNotRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void register_removed_should_mark_item_for_delete()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterRemoved( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeTrue();
            unitOfWork.ShouldRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void register_removed_should_not_trigger_delete_of_new_item()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( true );

            var unitOfWork = mock.Object;

            unitOfWork.RegisterNew( item );
            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterRemoved( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeFalse();
            unitOfWork.ShouldRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void register_removed_should_not_trigger_delete_of_untracked_item()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( true );

            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.RegisterRemoved( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeFalse();
            unitOfWork.ShouldNotRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void unregister_should_not_raise_event_when_no_changes_are_triggered()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.Unregister( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeFalse();
            unitOfWork.ShouldNotRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void unregister_should_raise_event_when_changes_are_triggered()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var unitOfWork = mock.Object;

            unitOfWork.RegisterNew( item );
            unitOfWork.MonitorEvents();

            // act
            unitOfWork.Unregister( item );

            // assert
            unitOfWork.HasPendingChanges.Should().BeFalse();
            unitOfWork.ShouldRaisePropertyChangeFor( u => u.HasPendingChanges );
        }

        [Fact]
        public void rollback_should_accept_changes()
        {
            // arrange
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup( "AcceptChanges" );

            var unitOfWork = mock.Object;

            unitOfWork.MonitorEvents();

            // act
            unitOfWork.Rollback();

            // assert
            mock.Protected().Verify( "AcceptChanges", Times.Once() );
            unitOfWork.HasPendingChanges.Should().BeFalse();
            unitOfWork.ShouldNotRaisePropertyChangeFor( u => u.HasPendingChanges );
        }
    }
}