namespace More.Mocks
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a mock object that can be used with the <see cref="FieldTransaction"/> or <see cref="PropertyTransaction"/> classes.
    /// </summary>
    public class MockEditableObject : EditableObject
    {
        private readonly IEditTransaction editTransaction;
        private int id;
        private string firstName;
        private string lastName;
        private DateTime createdDate = DateTime.Now;
        public string StreetAddress;
        public string City;
        public string State;
        public string PostalCode;

        public MockEditableObject()
            : base( "LastModified", "Id" )
        {
            editTransaction = base.CreateTransaction();
        }

        public MockEditableObject( Func<object, IEnumerable<string>, IEditTransaction> transactionFactory )
            : base( "LastModified", "Id" )
        {
            editTransaction = transactionFactory( this, UneditableMembers );
        }

        protected override IEditTransaction CreateTransaction()
        {
            return editTransaction;
        }

        public DateTime LastModified = DateTime.Now;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                SetProperty( ref id, value );
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                SetProperty( ref firstName, value );
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                SetProperty( ref lastName, value );
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return createdDate;
            }
        }

        public void InvokeOnBeforeEndEdit()
        {
            OnBeforeEndEdit();
        }

        public void InvokeOnBeforeBeginEdit()
        {
            OnBeforeBeginEdit();
        }

        public void InvokeOnPropertyChanged( PropertyChangedEventArgs e )
        {
            OnPropertyChanged( e );
        }

        public void InvokeOnPropertyChanged( string propertyName, bool suppressStateChanged )
        {
            OnPropertyChanged( propertyName, suppressStateChanged );
        }

        public void InvokeSetProperty<TValue>( TValue backingField, TValue newValue, string propertyName, IEqualityComparer<TValue> comparer )
        {
            SetProperty( ref backingField, newValue, comparer, propertyName );
        }

        public void InvokeIsChangedSet( bool value )
        {
            IsChanged = value;
        }
    }
}
