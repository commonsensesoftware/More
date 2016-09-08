namespace More.ComponentModel
{
    using DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using static EditRecoveryModel;
    using static System.StringComparer;

    /// <summary>
    /// Represents a base implementation of <see cref="IEditableObject"/> with support for two-way data binding.
    /// </summary>
    public abstract class EditableObject : ValidatableObject, IEditableObject, IRevertibleChangeTracking
    {
        private readonly ISet<string> uneditableMembers;
        private readonly Lazy<IEditTransaction> transaction;
        private IEditSavepoint savepoint;
        private EditRecoveryModel recoveryModel;
        private bool editing;
        private bool changed;
        private bool changedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObject"/> class.
        /// </summary>
        /// <param name="uneditableMembers">An array of uneditable member names.</param>
        protected EditableObject( params string[] uneditableMembers )
            : this( (IEnumerable<string>) uneditableMembers )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObject"/> class.
        /// </summary>
        /// <param name="uneditableMembers">A <see cref="IEnumerable{T}">sequence</see> of uneditable member names.</param>
        protected EditableObject( IEnumerable<string> uneditableMembers )
        {
            Arg.NotNull( uneditableMembers, nameof( uneditableMembers ) );

            this.uneditableMembers = new HashSet<string>( uneditableMembers, Ordinal );
            transaction = new Lazy<IEditTransaction>( CreateTransaction );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObject"/> class.
        /// </summary>
        /// <param name="validator">The associated <see cref="IValidator">validator</see>.</param>
        /// <param name="uneditableMembers">An array of uneditable member names.</param>
        protected EditableObject( IValidator validator, params string[] uneditableMembers )
            : this( validator, (IEnumerable<string>) uneditableMembers )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObject"/> class.
        /// </summary>
        /// <param name="validator">The associated <see cref="IValidator">validator</see>.</param>
        /// <param name="uneditableMembers">A <see cref="IEnumerable{T}">sequence</see> of uneditable member names.</param>
        protected EditableObject( IValidator validator, IEnumerable<string> uneditableMembers )
            : base( validator )
        {
            Arg.NotNull( uneditableMembers, nameof( uneditableMembers ) );

            this.uneditableMembers = new HashSet<string>( uneditableMembers, Ordinal );
            transaction = new Lazy<IEditTransaction>( CreateTransaction );
        }

        private IEditTransaction Transaction => transaction.Value;

        /// <summary>
        /// Gets a collection of the names of the object members that are not editable.
        /// </summary>
        /// <value>A <see cref="ISet{T}">set</see> of uneditable member names.</value>
        protected ISet<string> UneditableMembers
        {
            get
            {
                Contract.Ensures( uneditableMembers != null );
                return uneditableMembers;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is being edited.
        /// </summary>
        /// <value>True if the object is being edited; otherwise, false.</value>
        public bool IsEditing
        {
            get
            {
                return editing;
            }
            private set
            {
                if ( editing == value )
                {
                    return;
                }

                editing = value;
                OnPropertyChanged( nameof( IsEditing ), true );
            }
        }

        /// <summary>
        /// Gets or sets the edit recovery model for the object.
        /// </summary>
        /// <value>One of the <see cref="EditRecoveryModel"/> values.</value>
        /// <remarks>This property does not raise <see cref="E:PropertyChanged"/> by default.</remarks>
        public virtual EditRecoveryModel RecoveryModel
        {
            get
            {
                return recoveryModel;
            }
            set
            {
                if ( recoveryModel == value )
                {
                    return;
                }

                recoveryModel = value;
                savepoint = value == Full ? Transaction.CreateSavepoint() : null;
            }
        }

        private bool UseFullRecovery => RecoveryModel == Full;

        /// <summary>
        /// Creates and returns the type of edit transaction used to edit the current instance.
        /// </summary>
        /// <returns>An <see cref="IEditTransaction"/> object.</returns>
        /// <remarks>The default transaction used is the <see cref="PropertyTransaction"/> for all public properties.</remarks>
        protected virtual IEditTransaction CreateTransaction()
        {
            Contract.Ensures( Contract.Result<IEditTransaction>() != null );
            return new PropertyTransaction( this, p => !UneditableMembers.Contains( p.Name ) );
        }

        /// <summary>
        /// Occurs when when the changes to the object are about to be committed.
        /// </summary>
        /// <remarks>Note to inheritors: the default implementation validates the current instance.</remarks>
        protected virtual void OnBeforeEndEdit() => changedState = IsChanged;

        /// <summary>
        /// Occurs after the changes to the object have been committed.
        /// </summary>
        /// <remarks>Note to inheritors: the base class does not have an implementation and does not need to be called.</remarks>
        protected virtual void OnAfterEndEdit()
        {
        }

        /// <summary>
        /// Occurs when the changes to the object are about to be canceled.
        /// </summary>
        /// <remarks>Note to inheritors: the base class does not have an implementation and does not need to be called.</remarks>
        protected virtual void OnBeforeCancelEdit()
        {
        }

        /// <summary>
        /// Occurs after the changes to the object have been canceled.
        /// </summary>
        protected virtual void OnAfterCancelEdit() => IsChanged = changedState;

        /// <summary>
        /// Occurs when the object is about to be edited.
        /// </summary>
        protected virtual void OnBeforeBeginEdit()
        {
        }

        /// <summary>
        /// Occurs after editing on the object has begun.
        /// </summary>
        /// <remarks>Note to inheritors: the base class does not have an implementation and does not need to be called.</remarks>
        protected virtual void OnAfterBeginEdit()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.  A null or empty string indicates that all properties have changed.</param>
        /// <param name="suppressStateChange">Indicates whether the property changed event should trigger state change.</param>
        /// <remarks>The <see cref="P:IsChanged"/> property is to true whenever a property change is detected.  This overload can be used to
        /// raise <see cref="E:PropertyChanged"/> without triggering a state change.</remarks>
        protected void OnPropertyChanged( string propertyName, bool suppressStateChange ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ), suppressStateChange );

        /// <summary>
        /// Returns a value indicating whether the specific property triggers a state change.
        /// </summary>
        /// <param name="propertyName">The name of the property to evaluate.  If this parameter
        /// is null or an empty string, then all properties are considered.</param>
        /// <returns>True if the specified property causes a state change; otherwise, false.</returns>
        /// <remarks>Some properties of an editable object support change notification, but to not
        /// change the state of the object.  An editable object should not indicate it has changed
        /// state when one of these properties change.  The following properties do not trigger
        /// a state change in the default implementation: <see cref="P:IsChanged"/>, <see cref="P:IsEditing"/>,
        /// <see cref="P:RecoveryModel"/>, <see cref="P:IsValid"/>, <see cref="P:Error"/>, and
        /// <see cref="P:HasErrors"/>. When all properties change, a state change is assumed to be true.</remarks>
        protected virtual bool TriggersStateChange( string propertyName )
        {
            switch ( propertyName )
            {
                case nameof( IsChanged ):       // IChangeTracking.IsChanged
                case nameof( IsEditing ):       // EditableObject.IsEditing
                case nameof( RecoveryModel ):   // EditableObject.IsEditing
                case nameof( IsValid ):         // ValidatableObject.IsValid
                case nameof( HasErrors ):       // INotifyDataErrorInfo.HasErrors
                case "Error":                   // IDataErrorInfo.Error
                    return false;
            }

            return true;
        }

        private void OnPropertyChanged( PropertyChangedEventArgs e, bool suppressStateChange )
        {
            Contract.Requires( e != null );

            if ( TriggersStateChange( e.PropertyName ) && !suppressStateChange )
            {
                IsChanged = true;
            }

            base.OnPropertyChanged( e );
        }

        /// <summary>
        /// Overrides the default behavior when a property has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected override void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            OnPropertyChanged( e, false );
        }

        /// <summary>
        /// Begins a new edit operation.
        /// </summary>
        /// <remarks>If the object is already being edited, this method has no effect.</remarks>
        public void BeginEdit()
        {
            if ( IsEditing )
            {
                return;
            }

            IsEditing = true;
            OnBeforeBeginEdit();
            Transaction.Begin();
            OnAfterBeginEdit();
        }

        /// <summary>
        /// Commits and ends an edit operation.
        /// </summary>
        /// <remarks>If the object is not being edited, this method has no effect.</remarks>
        public void EndEdit()
        {
            if ( !IsEditing )
            {
                return;
            }

            OnBeforeEndEdit();
            Transaction.Commit();
            OnAfterEndEdit();
            IsEditing = false;
        }

        /// <summary>
        /// Cancels an edit operation.
        /// </summary>
        /// <remarks>If the object is not being edited, this method has no effect.</remarks>
        public void CancelEdit()
        {
            if ( !IsEditing )
            {
                return;
            }

            OnBeforeCancelEdit();
            Transaction.Rollback();
            OnAfterCancelEdit();
            IsEditing = false;
            OnPropertyChanged( (string) null, true );
        }

        /// <summary>
        /// Accepts any modifications made to the object and resets its state to unchanged.
        /// </summary>
        public virtual void AcceptChanges()
        {
            EndEdit();

            if ( UseFullRecovery )
            {
                savepoint = Transaction.CreateSavepoint();
            }

            IsChanged = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object has been changed.
        /// </summary>
        /// <value>True if the object has been changed; otherwise, false.</value>
        public bool IsChanged
        {
            get
            {
                return changed;
            }
            protected set
            {
                SetProperty( ref changed, value );
            }
        }

        /// <summary>
        /// Rejects any modifications made to the object and resets its state to unchanged.
        /// </summary>
        /// <remarks>All changes are rejected since the last call to <see cref="M:AcceptChanges"/>.  If <see cref="M:AcceptChanges"/> has never
        /// been called, then the state of the object reverts to its original initialization state.</remarks>
        public virtual void RejectChanges()
        {
            CancelEdit();

            if ( UseFullRecovery && savepoint != null )
            {
                Transaction.Rollback( savepoint );
            }

            IsChanged = false;
            OnPropertyChanged( (string) null, true );
        }
    }
}
