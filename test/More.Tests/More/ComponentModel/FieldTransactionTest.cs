namespace More.ComponentModel
{
    using FluentAssertions;
    using Mocks;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public class FieldTransactionTest
    {
        public static IEnumerable<object[]> NullTargetData
        {
            get
            {
                yield return new object[] { new Action<object>( target => new FieldTransaction( target ) ) };
                yield return new object[] { new Action<object>( target => new FieldTransaction( target, new[] { "ID" } ) ) };
                yield return new object[] { new Action<object>( target => new FieldTransaction( target, f => true ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( NullTargetData ) )]
        public void new_field_transaction_should_not_allow_null_target( Action<object> newFieldTransaction )
        {
            // arrange
            var target = default( object );

            // act
            Action @new = () => newFieldTransaction( target );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( target ) );
        }

        [Fact]
        public void new_field_transaction_should_not_allow_null_field_names()
        {
            var memberNames = default( IEnumerable<string> );

            // act
            Action @new = () => new FieldTransaction( new object(), memberNames );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( memberNames ) );
        }

        [Fact]
        public void new_field_transaction_should_not_allow_null_filter()
        {
            // arrange
            var memberFilter = default( Func<FieldInfo, bool> );

            // act
            Action @new = () => new FieldTransaction( new object(), memberFilter );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( memberFilter ) );
        }

        [Fact]
        public void begin_should_not_allow_nested_transactions()
        {
            // arrange
            var transaction = new FieldTransaction( new MockEditableObject() );

            transaction.Begin();

            // act
            Action begin = transaction.Begin;

            // assert
            begin.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void commit_should_require_started_transaction()
        {
            // arrange
            var transaction = new FieldTransaction( new MockEditableObject() );

            // act
            Action commit = transaction.Commit;

            // assert
            commit.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void rollback_should_require_started_transaction()
        {
            // arrange
            var transaction = new FieldTransaction( new MockEditableObject() );

            // act
            Action rollback = transaction.Rollback;

            // assert
            rollback.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void commit_should_complete_transaction()
        {
            // arrange
            var obj = new MockEditableObject();
            var transaction = new FieldTransaction( obj, new[] { nameof( MockEditableObject.LastModified ) } );
            var lastModified = DateTime.Now;

            // act
            transaction.Begin();
            obj.LastModified = lastModified;
            transaction.Commit();

            // assert
            obj.LastModified.Should().Be( lastModified );
        }

        [Fact]
        public void rollback_should_revert_changes()
        {
            // arrange
            var obj = new MockEditableObject();
            var transaction = new FieldTransaction( obj, new[] { nameof( MockEditableObject.LastModified ) } );
            var lastModified = obj.LastModified;

            // act
            transaction.Begin();
            obj.LastModified = DateTime.Now;
            transaction.Rollback();

            // assert
            obj.LastModified.Should().Be( lastModified );
        }
    }
}