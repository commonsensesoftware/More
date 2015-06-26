namespace More.ComponentModel
{
    using Moq;
    using Moq.Protected;
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="UnitOfWork{T}"/>.
    /// </summary>
    public class UnitOfWorkTTest
    {
        [Fact( DisplayName = "register new should mark item for insert" )]
        public void RegisterNewShouldMarkItemForInsert()
        {
            // arrange
            var eventRaised = false;
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var target = mock.Object;

            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";
            
            // act
            target.RegisterNew( new object() );

            // assert
            Assert.True( eventRaised );
            Assert.True( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register new should not allow delete" )]
        public void RegisterNewShouldNotAllowDeletedItem()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var target = mock.Object;

            // act
            target.RegisterRemoved( item );

            // assert
            Assert.Throws<InvalidOperationException>( () => target.RegisterNew( item ) );
        }

        [Fact( DisplayName = "register new should not allow update" )]
        public void RegisterNewShouldNotAllowUpdatedItem()
        {
            // arrange
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var target = mock.Object;

            // act
            target.RegisterChanged( item );

            // assert
            Assert.Throws<InvalidOperationException>( () => target.RegisterNew( item ) );
        }

        [Fact( DisplayName = "register changed should mark item for update" )]
        public void RegisterChangedShouldMarkItemForUpdate()
        {
            // arrange
            var eventRaised = false;
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var target = mock.Object;

            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";
            
            // act
            target.RegisterNew( new object() );

            // assert
            Assert.True( eventRaised );
            Assert.True( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register changed should not trigger update for new item" )]
        public void RegisterChangedShouldNotTriggerUpdateForNewItem()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var target = mock.Object;

            target.RegisterNew( item );
            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";
            
            // act
            target.RegisterChanged( item );

            // assert
            Assert.False( eventRaised );
            Assert.True( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register changed should not trigger update for deleted item" )]
        public void RegisterChangedShouldNotTriggerUpdateForDeletedItem()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var target = mock.Object;

            target.RegisterRemoved( item );
            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.RegisterChanged( item );

            // assert
            Assert.False( eventRaised );
            Assert.True( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register removed should mark item for delete" )]
        public void RegisterRemovedShouldMarkItemForDelete()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( false );

            var target = mock.Object;

            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.RegisterRemoved( item );

            // assert
            Assert.True( eventRaised );
            Assert.True( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register removed should not trigger delete of new item" )]
        public void RegisterRemovedShouldNotTriggerDeleteOfUncommittedNewItem()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( true );

            var target = mock.Object;

            target.RegisterNew( item );
            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.RegisterRemoved( item );

            // assert
            Assert.True( eventRaised );
            Assert.False( target.HasPendingChanges );
        }

        [Fact( DisplayName = "register removed should not trigger delete of untracked item" )]
        public void RegisterRemovedShouldNotTriggerDeleteOfUnknownItem()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup<bool>( "IsNew", ItExpr.IsAny<object>() ).Returns( true );

            var target = mock.Object;

            // item is unknown because it was never added to the unit of work and it's not "new"
            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.RegisterRemoved( item );

            // assert
            Assert.False( eventRaised );
            Assert.False( target.HasPendingChanges );
        }

        [Fact( DisplayName = "unregister should not raise event when no changes are triggered" )]
        public void UnregisterShouldNotRaiseEventWhenThereAreNoChanges()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var target = mock.Object;

            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.Unregister( item );

            // assert
            Assert.False( eventRaised );
            Assert.False( target.HasPendingChanges );
        }

        [Fact( DisplayName = "unregister should raise event when changes are triggered" )]
        public void UnregisterShouldRaiseEventWhenThereAreChanges()
        {
            // arrange
            var eventRaised = false;
            var item = new object();
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };
            var target = mock.Object;

            target.RegisterNew( item );
            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.Unregister( item );

            // assert
            Assert.True( eventRaised );
            Assert.False( target.HasPendingChanges );
        }

        [Fact( DisplayName = "rollback should accept changes" )]
        public void RollbackShouldAcceptChanges()
        {
            // arrange
            var eventRaised = false;
            var mock = new Mock<UnitOfWork<object>>() { CallBase = true };

            mock.Protected().Setup( "AcceptChanges" );

            var target = mock.Object;

            target.PropertyChanged += ( s, e ) => eventRaised = e.PropertyName == "HasPendingChanges";

            // act
            target.Rollback();

            mock.Protected().Verify( "AcceptChanges", Times.Once() );

            // assert
            Assert.False( eventRaised );
            Assert.False( target.HasPendingChanges );
        }
    }
}
