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
            this.editTransaction = base.CreateTransaction();
        }

        public MockEditableObject( Func<object, IEnumerable<string>, IEditTransaction> transactionFactory )
            : base( "LastModified", "Id" )
        {
            this.editTransaction = transactionFactory( this, this.UneditableMembers );
        }

        protected override IEditTransaction CreateTransaction()
        {
            return this.editTransaction;
        }

        public DateTime LastModified = DateTime.Now;

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.SetProperty( ref this.id, value );
            }
        }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                this.SetProperty( ref this.firstName, value );
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                this.SetProperty( ref this.lastName, value );
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return this.createdDate;
            }
        }

        public void InvokeOnBeforeEndEdit()
        {
            this.OnBeforeEndEdit();
        }

        public void InvokeOnBeforeBeginEdit()
        {
            this.OnBeforeBeginEdit();
        }

        public void InvokeOnPropertyChanged( PropertyChangedEventArgs e )
        {
            this.OnPropertyChanged( e );
        }

        public void InvokeOnPropertyChanged( string propertyName, bool suppressStateChanged )
        {
            this.OnPropertyChanged( propertyName, suppressStateChanged );
        }

        public void InvokeSetProperty<TValue>( TValue backingField, TValue newValue, string propertyName, IEqualityComparer<TValue> comparer )
        {
            this.SetProperty( ref backingField, newValue, comparer, propertyName );
        }

        public void InvokeIsChangedSet( bool value )
        {
            this.IsChanged = value;
        }
    }
}
