namespace More.ComponentModel
{
    using Mocks;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides unit tests for the <see cref="PropertyTransaction"/> class.
    /// </summary>
    public class PropertyTransactionTest
    {
        [Fact( DisplayName = "new property transaction should not allow null parameter" )]
        public void ConstructorsShouldNotAllowNullParameters()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new PropertyTransaction( null ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new PropertyTransaction( null, new[] { "ID" } ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new PropertyTransaction( new object(), (IEnumerable<string>) null ) );
            Assert.Equal( "memberNames", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new PropertyTransaction( null, f => true ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new PropertyTransaction( new object(), (Func<PropertyInfo, bool>) null ) );
            Assert.Equal( "memberFilter", ex.ParamName );
        }

        [Fact( DisplayName = "begin should not allow nested transaction" )]
        public void ShouldNotSupportBeginNestedTransaction()
        {
            // arrange
            var target = new PropertyTransaction( new MockEditableObject() );

            // act
            target.Begin();

            // assert
            Assert.Throws<InvalidOperationException>( () => target.Begin() );
        }

        [Fact( DisplayName = "commit should not be allowed without starting a transaction" )]
        public void ShouldNotCommitUnstartedTransaction()
        {
            // arrange
            var target = new PropertyTransaction( new MockEditableObject() );

            // act

            // assert
            Assert.Throws<InvalidOperationException>( () => target.Commit() );
        }

        [Fact( DisplayName = "rollback should not be allowed without starting a transaction" )]
        public void ShouldNotRollbackUnstartedTransaction()
        {
            // arrange
            var target = new PropertyTransaction( new MockEditableObject() );

            // act

            // assert
            Assert.Throws<InvalidOperationException>( () => target.Rollback() );
        }

        [Fact( DisplayName = "commit should complete changes to object" )]
        public void ShouldCommitChangesToObject()
        {
            // arrange
            var obj = new MockEditableObject();
            var createdDate = obj.CreatedDate;
            var transaction = new PropertyTransaction( obj );

            obj.Id = 1;
            obj.FirstName = string.Empty;
            obj.LastName = string.Empty;

            // act
            transaction.Begin();
            obj.FirstName = "unit";
            obj.LastName = "test";
            transaction.Commit();

            // assert
            Assert.Equal( 1, obj.Id );
            Assert.Equal( "unit", obj.FirstName );
            Assert.Equal( "test", obj.LastName );
            Assert.Equal( createdDate, obj.CreatedDate );
        }

        [Fact( DisplayName = "rollback should revert changes to object" )]
        public void ShouldRollbackChangesToObject()
        {
            // arrange
            var obj = new MockEditableObject();
            var createdDate = obj.CreatedDate;
            var transaction = new PropertyTransaction( obj, new[] { "FirstName", "LastName" } );

            obj.Id = 1;
            obj.FirstName = string.Empty;
            obj.LastName = string.Empty;

            // act
            transaction.Begin();
            obj.Id = 2;
            obj.FirstName = "unit";
            obj.LastName = "test";
            transaction.Rollback();

            // assert
            Assert.Equal( 2, obj.Id );
            Assert.Equal( string.Empty, obj.FirstName );
            Assert.Equal( string.Empty, obj.LastName );
            Assert.Equal( createdDate, obj.CreatedDate );
        }
    }
}
