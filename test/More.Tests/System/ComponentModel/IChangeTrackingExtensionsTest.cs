namespace System.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using More.ComponentModel;
    using More.ComponentModel.DataAnnotations;
    using More.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class IChangeTrackingExtensionsTest
    {
        [Fact]
        public void is_valid_should_not_allow_null()
        {
            // arrange
            var items = default( IEnumerable<ValidatableObject> );

            // act
            Action isValid = () => items.IsValid();

            // assert
            isValid.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( items ) );
        }

        [Fact]
        public void is_valid_should_return_false_when_any_items_are_invalid()
        {
            // arrange
            var items = new[] { new Mock<ValidatableObject>().Object, new ValidatableObjectWithAnError() };

            // act
            var valid = items.IsValid();

            // assert
            valid.Should().BeFalse();
        }

        [Fact]
        public void is_valid_should_return_true_when_all_items_are_valid()
        {
            // arrange
            var items = new[] { new Mock<ValidatableObject>().Object, new Mock<ValidatableObject>().Object };

            // act
            var valid = items.IsValid();

            // assert
            valid.Should().BeTrue();
        }

        [Fact]
        public void is_changed_should_not_allow_null_items()
        {
            // arrange
            var items = default( IEnumerable<EditableObject> );

            // act
            Action isChanged = () => items.IsChanged();

            // assert
            isChanged.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( items ) );
        }

        [Fact]
        public void is_changed_should_return_false_when_no_items_have_changed()
        {
            // arrange
            var items = new[] { new MockEditableObject(), new MockEditableObject() };

            // act
            var changed = items.IsChanged();

            // assert
            changed.Should().BeFalse();
        }

        [Fact]
        public void is_changed_should_return_true_when_any_item_has_changed()
        {
            // arrange
            var items = new[] { new MockEditableObject() { FirstName = "test" }, new MockEditableObject() };

            // act
            var changed = items.IsChanged();

            // assert
            changed.Should().BeTrue();
        }

        [Fact]
        public void accept_changes_should_not_allow_null()
        {
            // arrange
            var items = default( IEnumerable<EditableObject> );

            // act
            Action acceptChanges = () => items.AcceptChanges();

            // arrange
            acceptChanges.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( items ) );
        }

        [Fact]
        public void accept_changes_should_commit_all_changes()
        {
            // arrange
            var items = new[] { new MockEditableObject() { FirstName = "test1" }, new MockEditableObject() { FirstName = "test2" } };

            items.AcceptChanges();

            // act
            var unchanged = items.All( i => !i.IsChanged );

            // assert
            unchanged.Should().BeTrue();
        }

        [Fact]
        public void reject_changes_should_not_allow_null()
        {
            // arrange
            var items = default( IEnumerable<EditableObject> );

            // act
            Action rejectChanges = () => items.RejectChanges();

            // assert
            rejectChanges.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( items ) );
        }

        [Fact]
        public void reject_changes_should_revert_all_changes()
        {
            // arrange
            var items = new[] { new MockEditableObject() { FirstName = "test1" }, new MockEditableObject() { FirstName = "test2" } };

            items.RejectChanges();

            // act
            var unchanged = items.All( i => !i.IsChanged );

            // assert
            unchanged.Should().BeTrue();
        }

        sealed class ValidatableObjectWithAnError : ValidatableObject
        {
            public ValidatableObjectWithAnError() => PropertyErrors.Add( "Test", new Mock<IValidationResult>().Object );
        }
    }
}