namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.Contacts;

    /// <summary>
    /// Represents an interaction request to select one or more contacts.
    /// </summary>
    public class SelectContactInteraction : Interaction
    {
        private readonly ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();
        private readonly ObservableCollection<ContactFieldType> desiredFields = new ObservableCollection<ContactFieldType>();
        private bool multiselect;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectContactInteraction"/> class.
        /// </summary>
        public SelectContactInteraction()
            : this( string.Empty, false )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectContactInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="multiselect">Indicates whether multiple selections are allowed.</param>
        public SelectContactInteraction( string title, bool multiselect )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            this.multiselect = multiselect;
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple files can be selected.
        /// </summary>
        /// <value>True if multiple selections are allowed; otherwise, false. The default is false.</value>
        public bool Multiselect
        {
            get
            {
                return this.multiselect;
            }
            set
            {
                this.SetProperty( ref this.multiselect, value );
            }
        }

        /// <summary>
        /// Gets a list of selected contacts.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of selected <see cref="Contact">contacts</see>.</value>
        public virtual IList<Contact> Contacts
        {
            get
            {
                Contract.Ensures( this.contacts != null );
                Contract.Ensures( !Contract.Result<IList<Contact>>().IsReadOnly );
                return this.contacts;
            }
        }

        /// <summary>
        /// Gets a list of the requested contact fields.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of the requested <see cref="ContactFieldType">contact fields</see>.</value>
        public IList<ContactFieldType> DesiredFields
        {
            get
            {
                Contract.Ensures( this.desiredFields != null );
                Contract.Ensures( !Contract.Result<IList<ContactFieldType>>().IsReadOnly );
                return this.desiredFields;
            }
        }
    }
}
