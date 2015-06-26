namespace More.ComponentModel
{
    using Mocks;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="EditableObject"/>.
    /// </summary>
    public class EditableObjectTest
    {
        [Fact( DisplayName = "recovery model should write expected value" )]
        public void RecoveryModelPropertyShouldBeReadWrite()
        {
            var target = new MockEditableObject();
            Assert.Equal( EditRecoveryModel.Simple, target.RecoveryModel );
            target.RecoveryModel = EditRecoveryModel.Full;
            Assert.Equal( EditRecoveryModel.Full, target.RecoveryModel );
        }

        [Fact( DisplayName = "object should begin edit" )]
        public void BeginEndShouldStartEditOperation()
        {
            // arrange
            var target = new MockEditableObject();

            // act
            target.BeginEdit();

            // assert
            Assert.True( target.IsEditing );
        }

        [Fact( DisplayName = "object should cancel edit" )]
        public void CancelEditShouldAbortEditOperation()
        {
            // arrange
            var target = new MockEditableObject();

            // act
            target.BeginEdit();
            target.CancelEdit();

            // assert
            Assert.False( target.IsEditing );
        }

        [Fact( DisplayName = "object should end edit" )]
        public void EndEditShouldCompleteEditOperation()
        {
            // arrange
            var target = new MockEditableObject();

            // act
            target.BeginEdit();
            target.EndEdit();

            // assert
            Assert.False( target.IsEditing );
        }

        [Fact( DisplayName = "object should commit edit" )]
        public void EndEditShouldCommitChangesToObject()
        {
            // arrange
            var target = new MockEditableObject();
            var createdDate = target.CreatedDate;

            // act
            target.Id = 1;
            target.FirstName = string.Empty;
            target.LastName = string.Empty;

            target.BeginEdit();
            target.FirstName = "unit";
            target.LastName = "test";
            target.EndEdit();

            // assert
            Assert.False( target.IsEditing );
            Assert.Equal( 1, target.Id );
            Assert.Equal( "unit", target.FirstName );
            Assert.Equal( "test", target.LastName );
            Assert.Equal( createdDate, target.CreatedDate );
        }

        [Fact( DisplayName = "object should rollback changes" )]
        public void CancelEditShouldRollbackPropertyChangesToObject()
        {
            // arrange
            var target = new MockEditableObject();
            var createdDate = target.CreatedDate;

            target.Id = 1;
            target.FirstName = string.Empty;
            target.LastName = string.Empty;

            // act
            target.BeginEdit();
            target.Id = 2;
            target.FirstName = "unit";
            target.LastName = "test";
            target.CancelEdit();

            // assert
            Assert.False( target.IsEditing );
            Assert.Equal( 2, target.Id ); // doesn't support rollback
            Assert.Equal( string.Empty, target.FirstName );
            Assert.Equal( string.Empty, target.LastName );
            Assert.Equal( createdDate, target.CreatedDate );
        }

        [Fact( DisplayName = "object should rollback changes" )]
        public void CancelEditShouldRollbackFieldChangesToObject()
        {
            // arrange
            var target = new MockEditableObject( ( @this, uneditable ) => new FieldTransaction( @this, new []{ "LastModified" } ) );
            var lastModified = target.LastModified;

            // act
            target.BeginEdit();
            target.LastModified = DateTime.Now;
            target.CancelEdit();

            // assert
            Assert.False( target.IsEditing );
            Assert.Equal( lastModified, target.LastModified );
        }

        [Fact( DisplayName = "object should be unchanged after accept changes" )]
        public void AcceptChangesShouldSignalNoChanges()
        {
            // arrange
            var target = new MockEditableObject();

            target.Id = 5;
            target.LastName = "bill";
            target.FirstName = "mei";

            // act
            var changed = target.IsChanged;
            target.AcceptChanges();

            // assert
            Assert.True( changed );
            Assert.False( target.IsChanged );
        }

        [Fact( DisplayName = "is changed should be true if modified" )]
        public void IsChangedPropertyShouldBeTrueUntilAcceptChangesIsInvoked()
        {
            var target = new MockEditableObject();

            Assert.False( target.IsChanged );

            target.BeginEdit();
            target.Id = 5;
            target.LastName = "bill";
            target.FirstName = "mei";
            target.CancelEdit();

            // initial state was unchanged, so it should still be unchanged
            Assert.False( target.IsChanged );

            target.BeginEdit();
            target.Id = 10;
            target.LastName = "john";
            target.FirstName = "doe";
            target.EndEdit();

            // change committed, so it should now be changed
            Assert.True( target.IsChanged );

            target.BeginEdit();
            target.Id = 20;
            target.LastName = "jane";
            target.FirstName = "doe";
            target.CancelEdit();

            // although previous edit was canceled, the object was already the
            // changed state.  it should, therfore, still be in the changed state
            Assert.True( target.IsChanged );

            target.AcceptChanges();

            Assert.False( target.IsChanged );
        }

        [Fact( DisplayName = "object should be unchanged after reject changes" )]
        public void RejectChangesShouldSignalNoChanges()
        {
            // arrange
            var target = new MockEditableObject();

            target.Id = 5;
            target.LastName = "bill";
            target.FirstName = "mei";

            // act
            var changed = target.IsChanged;
            target.RejectChanges();

            // assert
            Assert.True( changed );
            Assert.False( target.IsChanged );
        }

        [Fact( DisplayName = "reject changes should revert object to savepoint" )]
        public void RejectChangesShouldRevertToSavepoint()
        {
            // arrange
            var target = new MockEditableObject();

            target.RecoveryModel = EditRecoveryModel.Full;
            target.Id = 5;
            target.FirstName = "bill";
            target.LastName = "mei";
            target.AcceptChanges();

            // act
            target.BeginEdit();
            target.FirstName = "john";
            target.LastName = "doe";
            target.EndEdit();

            var changed = target.IsChanged;

            target.RejectChanges();

            // assert
            Assert.True( changed );
            Assert.False( target.IsChanged );
            Assert.Equal( "bill", target.FirstName );
            Assert.Equal( "mei", target.LastName );
        }

        [Fact( DisplayName = "accept changes should create new savepoint" )]
        public void AcceptChangesShouldCreateNewSavepoint()
        {
            // arrange
            var target = new MockEditableObject();

            target.RecoveryModel = EditRecoveryModel.Full;
            target.Id = 5;
            target.FirstName = "bill";
            target.LastName = "mei";
            target.AcceptChanges();

            // act
            target.BeginEdit();
            target.FirstName = "john";
            target.LastName = "doe";
            target.EndEdit();
            target.AcceptChanges();

            target.BeginEdit();
            target.FirstName = "jane";
            target.LastName = "doe";
            target.EndEdit();

            target.RejectChanges();

            // assert
            Assert.False( target.IsChanged );
            Assert.Equal( "john", target.FirstName );
            Assert.Equal( "doe", target.LastName );
        }

        [Fact( DisplayName = "property changed should trigger state change when unsuppressed" )]
        public void OnPropertyChangedShouldTriggerStateChangeWhenNotSuppressed()
        {
            var target = new MockEditableObject();
            target.InvokeOnPropertyChanged( "FirstName", false );
            Assert.True( target.IsChanged );
        }

        [Fact( DisplayName = "property changed should not trigger state change when suppressed" )]
        public void OnPropertyChangedShouldNotTriggerStateChangeWhenSuppressed()
        {
            var target = new MockEditableObject();
            target.InvokeOnPropertyChanged( "FirstName", true );
            Assert.False( target.IsChanged );
        }
    }
}
