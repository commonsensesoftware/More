namespace System.ComponentModel
{
    using Moq;
    using More.ComponentModel;
    using More.Mocks;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IChangeTrackingExtensions" />.
    /// </summary>
    public class IChangeTrackingExtensionsTest
    {
        [Fact( DisplayName = "is valid should not allow null" )]
        public void IsValidShouldNotAllowNullArguments()
        {
            // arrange
            IEnumerable<ValidatableObject> items = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => items.IsValid() );
            
            // assert
            Assert.Equal( "items", ex.ParamName );
        }

        [Fact( DisplayName = "is valid should return false when any items are invalid" )]
        public void IsValidShouldReturnFalseWhenAnyItemIsInvalid()
        {
            // arrange
            var item1 = new Mock<ValidatableObject>();
            var item2 = new Mock<ValidatableObject>();

            item1.SetupGet( i => i.IsValid ).Returns( true );
            item2.SetupGet( i => i.IsValid ).Returns( false );

            var items = new[] { item1.Object, item2.Object };

            // act
            var valid = items.IsValid();

            // assert
            Assert.False( valid );
        }

        [Fact( DisplayName = "is valid should return true when all items are valid" )]
        public void IsValidShouldReturnTrueAllItemsAreValid()
        {
            // arrange
            var item1 = new Mock<ValidatableObject>();
            var item2 = new Mock<ValidatableObject>();

            item1.SetupGet( i => i.IsValid ).Returns( true );
            item2.SetupGet( i => i.IsValid ).Returns( true );

            var items = new[] { item1.Object, item2.Object };

            // act
            var valid = items.IsValid();

            // assert
            Assert.True( valid );
        }

        [Fact( DisplayName = "is changed should not allow null items" )]
        public void IsChangedShouldNotAllowNullItems()
        {
            // arrange
            IEnumerable<EditableObject> items = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => items.IsChanged() );
            
            // assert
            Assert.Equal( "items", ex.ParamName );
        }

        [Fact( DisplayName = "is changed should return false when no items have changed" )]
        public void IsChangedShouldReturnFalseWhenNoItemHaveBeenChanged()
        {
            // arrange
            var items = new[] { new MockEditableObject(), new MockEditableObject() };

            // act
            var changed = items.IsChanged();

            // assert
            Assert.False( changed );
        }

        [Fact( DisplayName = "is changed should return true when any item has changed" )]
        public void IsChangedShouldReturnTrueWhenAnyItemHasBeenChanged()
        {
            // arrange
            var items = new[] { new MockEditableObject(), new MockEditableObject() };

            items[0].FirstName = "test";

            // act
            var changed = items.IsChanged();

            // assert
            Assert.True( changed );
        }

        [Fact( DisplayName = "accept changes should not allow null" )]
        public void AcceptChangesShouldNotAllowNullArguments()
        {
            // arrange
            IEnumerable<EditableObject> items = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => items.AcceptChanges() );
            
            // arrange
            Assert.Equal( "items", ex.ParamName );
        }

        [Fact( DisplayName = "accept changes should commit all changes" )]
        public void AcceptChangesShouldCommitChangesToAllItems()
        {
            // arrange
            var items = new[] { new MockEditableObject(), new MockEditableObject() };
            
            items[0].FirstName = "test1";
            items[1].FirstName = "test2";
            items.AcceptChanges();

            // act
            var unchanged = items.All( i => !i.IsChanged );

            Assert.True( unchanged );
        }

        [Fact( DisplayName = "reject changes should not allow null" )]
        public void RejectChangesShouldNotAllowNullArguments()
        {
            // arrange
            IEnumerable<EditableObject> items = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => items.RejectChanges() );
            
            // assert
            Assert.Equal( "items", ex.ParamName );
        }

        [Fact( DisplayName = "reject changes should revert all changes" )]
        public void RejectChangesShouldRevertChangesToAllItems()
        {
            // arrange
            var items = new[] { new MockEditableObject(), new MockEditableObject() };

            items[0].FirstName = "test1";
            items[1].FirstName = "test2";
            items.RejectChanges();

            // act
            var unchanged = items.All( i => !i.IsChanged );

            // assert
            Assert.True( unchanged );
        }
    }
}
