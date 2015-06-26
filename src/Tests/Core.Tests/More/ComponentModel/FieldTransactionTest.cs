namespace More.ComponentModel
{
    using Mocks;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="FieldTransactionTest"/> class.
    /// </summary>
    public class FieldTransactionTest
    {
        [Fact( DisplayName = "new field transaction should not allow null parameters" )]
        public void ConstructorsShouldNotAllowNullParameters()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new FieldTransaction( null ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new FieldTransaction( null, new[] { "ID" } ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new FieldTransaction( new object(), (IEnumerable<string>) null ) );
            Assert.Equal( "fieldNames", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new FieldTransaction( null, f => true ) );
            Assert.Equal( "target", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new FieldTransaction( new object(), (Func<FieldInfo, bool>) null ) );
            Assert.Equal( "filter", ex.ParamName );
        }

        [Fact( DisplayName = "begin should not support nested transactions" )]
        public void ShouldNotSupportBeginNestedTransaction()
        {
            var target = new FieldTransaction( new MockEditableObject() );
            target.Begin();
            Assert.Throws<InvalidOperationException>( () => target.Begin() );
        }

        [Fact( DisplayName = "commit should require started transaction" )]
        public void ShouldNotCommitUnstartedTransaction()
        {
            var target = new FieldTransaction( new MockEditableObject() );
            Assert.Throws<InvalidOperationException>( () => target.Commit() );
        }

        [Fact( DisplayName = "rollback should require started transaction" )]
        public void ShouldNotRollbackUnstartedTransaction()
        {
            var target = new FieldTransaction( new MockEditableObject() );
            Assert.Throws<InvalidOperationException>( () => target.Rollback() );
        }

        [Fact( DisplayName = "commit should complete transaction" )]
        public void ShouldCommitChangesToObject()
        {
            // arrange
            var obj = new MockEditableObject();
            var transaction = new FieldTransaction( obj, new[] { "LastModified" } );
            var lastModified = DateTime.Now;

            // act
            transaction.Begin();
            obj.LastModified = lastModified;
            transaction.Commit();

            // assert
            Assert.Equal( lastModified, obj.LastModified );
        }

        [Fact( DisplayName = "rollback should revert changes" )]
        public void ShouldRollbackChangesToObject()
        {
            // arrange
            var obj = new MockEditableObject();
            var transaction = new FieldTransaction( obj, new[] { "LastModified" } );
            var lastModified = obj.LastModified;

            // act
            transaction.Begin();
            obj.LastModified = DateTime.Now;
            transaction.Rollback();

            // assert
            Assert.Equal( lastModified, obj.LastModified );
        }
    }
}

