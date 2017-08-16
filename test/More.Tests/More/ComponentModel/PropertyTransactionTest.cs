namespace More.ComponentModel
{
    using FluentAssertions;
    using Mocks;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;
    using static System.String;

    public class PropertyTransactionTest
    {
        public static IEnumerable<object[]> NullTargetData
        {
            get
            {
                yield return new object[] { new Action<object>( target => new PropertyTransaction( target ) ) };
                yield return new object[] { new Action<object>( target => new PropertyTransaction( target, new[] { "ID" } ) ) };
                yield return new object[] { new Action<object>( target => new PropertyTransaction( target, f => true ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( NullTargetData ) )]
        public void new_property_transaction_should_not_allow_null_target( Action<object> newPropertyTransaction )
        {
            // arrange
            var target = default( object );

            // act
            Action @new = () => newPropertyTransaction( target );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( target ) );
        }

        [Fact]
        public void new_property_transaction_should_not_allow_null_property_names()
        {
            // arrange
            var memberNames = default( IEnumerable<string> );

            // act
            Action @new = () => new PropertyTransaction( new object(), memberNames );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( memberNames ) );
        }

        [Fact]
        public void new_field_transaction_should_not_allow_null_filter()
        {
            // arrange
            var memberFilter = default( Func<PropertyInfo, bool> );

            // act
            Action @new = () => new PropertyTransaction( new object(), memberFilter );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( memberFilter ) );
        }

        [Fact]
        public void begin_should_not_allow_nested_transaction()
        {
            // arrange
            var transaction = new PropertyTransaction( new MockEditableObject() );

            transaction.Begin();

            // act
            Action begin = transaction.Begin;

            // assert
            begin.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void commit_should_not_be_allowed_without_starting_a_transaction()
        {
            // arrange
            var transaction = new PropertyTransaction( new MockEditableObject() );

            // act
            Action commit = transaction.Commit;

            // assert
            commit.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void rollback_should_not_be_allowed_without_starting_a_transaction()
        {
            // arrange
            var transaction = new PropertyTransaction( new MockEditableObject() );

            // act
            Action rollback = transaction.Rollback;

            // assert
            rollback.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void commit_should_complete_changes_to_object()
        {
            // arrange
            var obj = new MockEditableObject() { Id = 1, FirstName = Empty, LastName = Empty };
            var createdDate = obj.CreatedDate;
            var transaction = new PropertyTransaction( obj );

            // act
            transaction.Begin();
            obj.FirstName = "unit";
            obj.LastName = "test";
            transaction.Commit();

            // assert
            obj.ShouldBeEquivalentTo(
                new { Id = 1, FirstName = "unit", LastName = "test", CreatedDate = createdDate },
                options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public void rollback_should_revert_changes_to_object()
        {
            // arrange
            var obj = new MockEditableObject() { Id = 1, FirstName = Empty, LastName = Empty };
            var createdDate = obj.CreatedDate;
            var propertyNames = new[] { nameof( MockEditableObject.FirstName ), nameof( MockEditableObject.LastName ) };
            var transaction = new PropertyTransaction( obj, propertyNames );

            // act
            transaction.Begin();
            obj.Id = 2;
            obj.FirstName = "unit";
            obj.LastName = "test";
            transaction.Rollback();

            // assert
            obj.ShouldBeEquivalentTo(
                new { Id = 2, FirstName = Empty, LastName = Empty, CreatedDate = createdDate },
                options => options.ExcludingMissingMembers() );
        }
    }
}