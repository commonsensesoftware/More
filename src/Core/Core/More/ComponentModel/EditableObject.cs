namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a base implementation of <see cref="IEditableObject"/> with support for two-way data binding.
    /// </summary>
    public abstract class EditableObject : ValidatableObject, IEditableObject, IRevertibleChangeTracking
    {
        private IEditTransaction transaction;
        private IEditSavepoint savepoint;
        private EditRecoveryModel recoveryModel;
        private bool editing;
        private bool changed;
        private bool changedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableObject"/> class.
        /// </summary>
        protected EditableObject()
        {
        }

        private IEditTransaction Transaction
        {
            get
            {
                Contract.Ensures( Contract.Result<IEditTransaction>() != null );

                if ( this.transaction == null )
                    this.transaction = this.CreateTransaction();

                return this.transaction;
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
                return this.editing;
            }
            private set
            {
                if ( this.editing == value )
                    return;

                this.editing = value;
                this.OnPropertyChanged( "IsEditing", true );
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
                return this.recoveryModel;
            }
            set
            {
                if ( this.recoveryModel == value )
                    return;

                this.recoveryModel = value;
                this.savepoint = value == EditRecoveryModel.Full ? this.Transaction.CreateSavepoint() : null;
            }
        }

        private bool UseFullRecovery
        {
            get
            {
                return this.RecoveryModel == EditRecoveryModel.Full;
            }
        }

        /// <summary>
        /// Creates and returns the type of edit transaction used to edit the current instance.
        /// </summary>
        /// <returns>An <see cref="IEditTransaction"/> object.</returns>
        /// <remarks>The default transaction used is the <see cref="PropertyTransaction"/> for all public properties.</remarks>
        protected virtual IEditTransaction CreateTransaction()
        {
            Contract.Ensures( Contract.Result<IEditTransaction>() != null );
            return new PropertyTransaction( this );
        }

        /// <summary>
        /// Occurs when when the changes to the object are about to be committed.
        /// </summary>
        /// <remarks>Note to inheritors: the default implementation validates the current instance.</remarks>
        protected virtual void OnBeforeEndEdit()
        {
            this.changedState = this.IsChanged;
        }

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
        protected virtual void OnAfterCancelEdit()
        {
            this.IsChanged = this.changedState;
        }

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
        protected void OnPropertyChanged( string propertyName, bool suppressStateChange )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ), suppressStateChange );
        }

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
                case "IsChanged":       // IChangeTracking.IsChanged
                case "IsEditing":       // EditableObject.IsEditing
                case "RecoveryModel":   // EditableObject.IsEditing
                case "IsValid":         // ValidatableObject.IsValid
                case "Error":           // IDataErrorInfo.Error
                case "HasErrors":       // INotifyDataErrorInfo.HasErrors
                    return false;
            }

            return true;
        }

        private void OnPropertyChanged( PropertyChangedEventArgs e, bool suppressStateChange )
        {
            Contract.Requires( e != null, "e" );

            // if not suppressed, change the state
            if ( this.TriggersStateChange( e.PropertyName ) && !suppressStateChange )
                this.IsChanged = true;

            base.OnPropertyChanged( e );
        }

        /// <summary>
        /// Overrides the default behavior when a property has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected override void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            this.OnPropertyChanged( e, false );
        }

        /// <summary>
        /// Begins a new edit operation.
        /// </summary>
        /// <remarks>If the object is already being edited, this method has no effect.</remarks>
        public void BeginEdit()
        {
            if ( this.IsEditing )
                return;

            this.IsEditing = true;
            this.OnBeforeBeginEdit();
            this.Transaction.Begin();
            this.OnAfterBeginEdit();
        }

        /// <summary>
        /// Commits and ends an edit operation.
        /// </summary>
        /// <remarks>If the object is not being edited, this method has no effect.</remarks>
        public void EndEdit()
        {
            if ( !this.IsEditing )
                return;

            this.OnBeforeEndEdit();
            this.Transaction.Commit();
            this.OnAfterEndEdit();
            this.IsEditing = false;
        }

        /// <summary>
        /// Cancels an edit operation.
        /// </summary>
        /// <remarks>If the object is not being edited, this method has no effect.</remarks>
        public void CancelEdit()
        {
            if ( !this.IsEditing )
                return;

            this.OnBeforeCancelEdit();
            this.Transaction.Rollback();
            this.OnAfterCancelEdit();
            this.IsEditing = false;
            this.OnPropertyChanged( (string) null, true );
        }

        /// <summary>
        /// Accepts any modifications made to the object and resets its state to unchanged.
        /// </summary>
        public virtual void AcceptChanges()
        {
            this.EndEdit();

            if ( this.UseFullRecovery )
                this.savepoint = this.Transaction.CreateSavepoint();

            this.IsChanged = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object has been changed.
        /// </summary>
        /// <value>True if the object has been changed; otherwise, false.</value>
        public bool IsChanged
        {
            get
            {
                return this.changed;
            }
            protected set
            {
                this.SetProperty( ref this.changed, value );
            }
        }

        /// <summary>
        /// Rejects any modifications made to the object and resets its state to unchanged.
        /// </summary>
        /// <remarks>All changes are rejected since the last call to <see cref="M:AcceptChanges"/>.  If <see cref="M:AcceptChanges"/> has never
        /// been called, then the state of the object reverts to its original initialization state.</remarks>
        public virtual void RejectChanges()
        {
            this.CancelEdit();

            if ( this.UseFullRecovery && this.savepoint != null )
                this.Transaction.Rollback( this.savepoint );

            this.IsChanged = false;
            this.OnPropertyChanged( (string) null, true );
        }
    }
}
