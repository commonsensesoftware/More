namespace More.ComponentModel
{
    using FluentAssertions;
    using Mocks;
    using System;
    using Xunit;
    using static More.ComponentModel.EditRecoveryModel;
    using static System.String;

    public class EditableObjectTest
    {
        [Fact]
        public void recovery_model_should_be_simple_by_default()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            var recoveryModel = editableObject.RecoveryModel;

            // assert
            recoveryModel.Should().Be( Simple );
        }

        [Fact]
        public void recovery_model_should_write_expected_value()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.RecoveryModel = Full;

            // assert
            editableObject.RecoveryModel.Should().Be( Full );
        }

        [Fact]
        public void object_should_begin_edit()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.BeginEdit();

            // assert
            editableObject.IsEditing.Should().BeTrue();
        }

        [Fact]
        public void object_should_cancel_edit()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.BeginEdit();
            editableObject.CancelEdit();

            // assert
            editableObject.IsEditing.Should().BeFalse();
        }

        [Fact]
        public void object_should_end_edit()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.BeginEdit();
            editableObject.EndEdit();

            // assert
            editableObject.IsEditing.Should().BeFalse();
        }

        [Fact]
        public void object_should_commit_edit()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                Id = 1,
                FirstName = Empty,
                LastName = Empty
            };
            var createdDate = editableObject.CreatedDate;

            // act
            editableObject.BeginEdit();
            editableObject.FirstName = "unit";
            editableObject.LastName = "test";
            editableObject.EndEdit();

            // assert
            editableObject.ShouldBeEquivalentTo(
                new
                {
                    IsEditing = false,
                    Id = 1,
                    FirstName = "unit",
                    LastName = "test",
                    CreatedDate = createdDate
                },
                options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public void object_should_rollback_changes_to_properties()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                Id = 1,
                FirstName = Empty,
                LastName = Empty
            };
            var createdDate = editableObject.CreatedDate;

            // act
            editableObject.BeginEdit();
            editableObject.Id = 2;
            editableObject.FirstName = "unit";
            editableObject.LastName = "test";
            editableObject.CancelEdit();

            // assert
            editableObject.ShouldBeEquivalentTo(
                new
                {
                    IsEditing = false,
                    Id = 2,
                    FirstName = Empty,
                    LastName = Empty,
                    CreatedDate = createdDate
                },
                options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public void object_should_rollback_changes_to_fields()
        {
            // arrange
            var editableObject = new MockEditableObject( ( @this, uneditable ) => new FieldTransaction( @this, new[] { nameof( MockEditableObject.LastModified ) } ) );
            var lastModified = editableObject.LastModified;

            // act
            editableObject.BeginEdit();
            editableObject.LastModified = DateTime.Now;
            editableObject.CancelEdit();

            // assert
            editableObject.ShouldBeEquivalentTo( new { IsEditing = false, LastModified = lastModified }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void object_should_be_unchanged_after_accept_changes()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                Id = 5,
                LastName = "bill",
                FirstName = "mei"
            };

            // act
            var wasChanged = editableObject.IsChanged;
            editableObject.AcceptChanges();

            // assert
            wasChanged.Should().BeTrue();
            editableObject.IsChanged.Should().BeFalse();
        }

        [Fact]
        public void is_changed_should_be_true_if_modified()
        {
            var editableObject = new MockEditableObject();

            editableObject.IsChanged.Should().BeFalse();

            editableObject.BeginEdit();
            editableObject.Id = 5;
            editableObject.LastName = "bill";
            editableObject.FirstName = "mei";
            editableObject.CancelEdit();

            // initial state was unchanged, so it should still be unchanged
            editableObject.IsChanged.Should().BeFalse();

            editableObject.BeginEdit();
            editableObject.Id = 10;
            editableObject.LastName = "john";
            editableObject.FirstName = "doe";
            editableObject.EndEdit();

            // change committed, so it should now be changed
            editableObject.IsChanged.Should().BeTrue();

            editableObject.BeginEdit();
            editableObject.Id = 20;
            editableObject.LastName = "jane";
            editableObject.FirstName = "doe";
            editableObject.CancelEdit();

            // although previous edit was canceled, the object was already the
            // changed state.  it should, therfore, still be in the changed state
            editableObject.IsChanged.Should().BeTrue();

            editableObject.AcceptChanges();
            editableObject.IsChanged.Should().BeFalse();
        }

        [Fact]
        public void object_should_be_unchanged_after_reject_changes()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                Id = 5,
                LastName = "bill",
                FirstName = "mei"
            };

            // act
            var wasChanged = editableObject.IsChanged;
            editableObject.RejectChanges();

            // assert
            wasChanged.Should().BeTrue();
            editableObject.IsChanged.Should().BeFalse();
        }

        [Fact]
        public void reject_changes_should_revert_object_to_savepoint()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                RecoveryModel = Full,
                Id = 5,
                FirstName = "bill",
                LastName = "mei"
            };
            editableObject.AcceptChanges();

            // act
            editableObject.BeginEdit();
            editableObject.FirstName = "john";
            editableObject.LastName = "doe";
            editableObject.EndEdit();

            var wasChanged = editableObject.IsChanged;

            editableObject.RejectChanges();

            // assert
            wasChanged.Should().BeTrue();
            editableObject.ShouldBeEquivalentTo( new { IsChanged = false, FirstName = "bill", LastName = "mei" }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void accept_changes_should_create_new_savepoint()
        {
            // arrange
            var editableObject = new MockEditableObject()
            {
                RecoveryModel = Full,
                Id = 5,
                FirstName = "bill",
                LastName = "mei"
            };
            editableObject.AcceptChanges();

            // act
            editableObject.BeginEdit();
            editableObject.FirstName = "john";
            editableObject.LastName = "doe";
            editableObject.EndEdit();
            editableObject.AcceptChanges();

            editableObject.BeginEdit();
            editableObject.FirstName = "jane";
            editableObject.LastName = "doe";
            editableObject.EndEdit();

            editableObject.RejectChanges();

            // assert
            editableObject.ShouldBeEquivalentTo( new { IsChanged = false, FirstName = "john", LastName = "doe" }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void property_changed_should_trigger_state_change_when_unsuppressed()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.InvokeOnPropertyChanged( nameof( MockEditableObject.FirstName ), false );

            // assert
            editableObject.IsChanged.Should().BeTrue();
        }

        [Fact]
        public void property_changed_should_not_trigger_state_change_when_suppressed()
        {
            // arrange
            var editableObject = new MockEditableObject();

            // act
            editableObject.InvokeOnPropertyChanged( nameof( MockEditableObject.FirstName ), true );

            // assert
            editableObject.IsChanged.Should().BeFalse();
        }

        [Fact]
        public void object_should_cancel_edit_when_no_members_are_filtered()
        {
            // arrange
            var @object = new UnfilteredMemberObject() { Name = "Bob" };

            // act
            @object.BeginEdit();
            @object.Name = "John";
            @object.CancelEdit();

            // assert
            @object.Name.Should().Be( "Bob" );
        }

        public class UnfilteredMemberObject : EditableObject
        {
            public string Name { get; set; }
        }
    }
}