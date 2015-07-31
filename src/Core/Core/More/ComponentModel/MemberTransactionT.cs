namespace More.ComponentModel
{
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a edit transation for object members.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="MemberInfo">member</see> the transaction is for.</typeparam>
    [ContractClass( typeof( MemberTransactionContract<> ) )]
    public abstract class MemberTransaction<T> : IEditTransaction where T : MemberInfo
    {
        private readonly object instance;
        private readonly Type type;
        private readonly KeyedCollection<string, T> members = new ObservableKeyedCollection<string, T>( m => m.Name );
        private readonly Dictionary<string, object> state = new Dictionary<string, object>();
        private readonly IEditSnapshotStrategy snapshotStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <remarks>All public, supported members are enlisted.</remarks>
        protected MemberTransaction( object target )
            : this( target, member => true, SnapshotStrategy.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        /// <remarks>All public, supported members are enlisted.</remarks>
        protected MemberTransaction( object target, IEditSnapshotStrategy editSnapshotStrategy )
            : this( target, member => true, editSnapshotStrategy )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberNames">A <see cref="IEnumerable{T}">sequence</see> of member names to enlist in the transaction.</param>
        protected MemberTransaction( object target, IEnumerable<string> memberNames )
            : this( target, member => ( memberNames ?? Enumerable.Empty<string>() ).Contains( member.Name ), SnapshotStrategy.Default )
        {
            Arg.NotNull( memberNames, nameof( memberNames ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberNames">A <see cref="IEnumerable{T}">sequence</see> of member names to enlist in the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        protected MemberTransaction( object target, IEnumerable<string> memberNames, IEditSnapshotStrategy editSnapshotStrategy )
            : this( target, member => ( memberNames ?? Enumerable.Empty<string>() ).Contains( member.Name ), editSnapshotStrategy )
        {
            Arg.NotNull( memberNames, nameof( memberNames ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberFilter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted members.</param>
        protected MemberTransaction( object target, Func<T, bool> memberFilter )
            : this( target, memberFilter, SnapshotStrategy.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberFilter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted members.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        protected MemberTransaction( object target, Func<T, bool> memberFilter, IEditSnapshotStrategy editSnapshotStrategy )
        {
            Arg.NotNull( target, nameof( target ) );
            Arg.NotNull( memberFilter, nameof( memberFilter ) );
            Arg.NotNull( editSnapshotStrategy, nameof( editSnapshotStrategy ) );

            instance = target;
            type = target.GetType();
            snapshotStrategy = editSnapshotStrategy;

            var allMembers = type.GetRuntimeFields().Cast<MemberInfo>().Union( type.GetRuntimeProperties() );
            var filteredMembers = allMembers.OfType<T>().Where( m => memberFilter( m ) && CanManage( m ) );

            members.AddRange( filteredMembers );
        }

        /// <summary>
        /// Gets the state of the transaction.
        /// </summary>
        /// <value>One of the <see cref="EditTransactionState"/> values.</value>
        protected EditTransactionState TransactionState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the object this instance is a transaction for.
        /// </summary>
        /// <value>The transacted <see cref="Object">object</see>.</value>
        protected object Instance
        {
            get
            {
                Contract.Ensures( Contract.Result<object>() != null );
                return instance;
            }
        }

        /// <summary>
        /// Gets the type of object this instance is a transaction for.
        /// </summary>
        /// <value>The transacted object <see cref="Type">type</see>.</value>
        protected Type InstanceType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null );
                return type;
            }
        }

        /// <summary>
        /// Gets the members this transaction manages.
        /// </summary>
        /// <value>A <see cref="KeyedCollection{TKey,TValue}"/> object.</value>
        protected KeyedCollection<string, T> Members
        {
            get
            {
                Contract.Ensures( Contract.Result<KeyedCollection<string, T>>() != null );
                return members;
            }
        }

        /// <summary>
        /// Gets the state of the members managed by this transaction.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
        protected virtual IDictionary<string, object> State
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );
                return state;
            }
        }

        /// <summary>
        /// Gets the strategy used for snapshots.
        /// </summary>
        /// <value>An <see cref="IEditSnapshotStrategy"/> object.</value>
        protected virtual IEditSnapshotStrategy EditSnapshotStrategy
        {
            get
            {
                Contract.Ensures( Contract.Result<IEditSnapshotStrategy>() != null );
                return snapshotStrategy;
            }
        }

        private static bool CanManage( MemberInfo member )
        {
            Contract.Requires( member != null );

            var field = member as FieldInfo;

            if ( field == null )
            {
                var property = member as PropertyInfo;

                // make sure there is a get and set method; otherwise, the property is read-only
                // check the setter first as write-only properties are extremely rare
                return property != null && property.SetMethod != null && property.GetMethod != null;
            }

            // exclude read-only, constant, and non-public fields
            return field.IsPublic && !field.IsInitOnly && !field.IsLiteral;
        }

        /// <summary>
        /// Creates and returns a state key for the specified member.
        /// </summary>
        /// <param name="member">The member of type <typeparamref name="T"/> to create a state key for.</param>
        /// <returns>A <see cref="String">string</see> representing the state key.</returns>
        protected virtual string CreateStateKey( T member )
        {
            Arg.NotNull( member, nameof( member ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            return string.Format( null, "{0}.{1}", member.DeclaringType.FullName, member.Name );
        }

        /// <summary>
        /// Returns the type for the specified member.
        /// </summary>
        /// <param name="member">The member of <typeparamref name="T"/> to get the <see cref="Type">type</see> for.</param>
        /// <returns>A <see cref="Type">type</see> object.</returns>
        protected abstract Type GetMemberType( T member );

        /// <summary>
        /// Gets the value of the specified member.
        /// </summary>
        /// <param name="member">The member of <typeparamref name="T"/> to get the value for.</param>
        /// <returns>The value of the member.</returns>
        protected abstract object GetMemberValue( T member );

        /// <summary>
        /// Sets the value of the specified member.
        /// </summary>
        /// <param name="member">The member of <typeparamref name="T"/> to set the value for.</param>
        /// <param name="value">The value to set.</param>
        protected abstract void SetMemberValue( T member, object value );

        /// <summary>
        /// Occurs when a transaction is started.
        /// </summary>
        protected virtual void OnBegin()
        {
            foreach ( var member in Members )
            {
                var value = GetMemberValue( member );
                var key = CreateStateKey( member );
                var editable = value as IEditableObject;

                if ( editable != null )
                    editable.BeginEdit();

                State[key] = value;
            }
        }

        /// <summary>
        /// Occurs when a transaction is committed.
        /// </summary>
        protected virtual void OnCommit()
        {
            var editableType = typeof( IEditableObject ).GetTypeInfo();
            var editableMembers = from member in Members
                                  where editableType.IsAssignableFrom( GetMemberType( member ).GetTypeInfo() )
                                  let key = CreateStateKey( member )
                                  let editable = (IEditableObject) State[key]
                                  where editable != null
                                  select editable;

            editableMembers.ForEach( e => e.EndEdit() );
            State.Clear();
        }

        /// <summary>
        /// Occurs when a transaction is rolled back.
        /// </summary>
        protected virtual void OnRollback()
        {
            foreach ( var member in Members )
            {
                var key = CreateStateKey( member );
                var value = State[key];
                var editable = value as IEditableObject;

                if ( editable != null )
                    editable.CancelEdit();

                SetMemberValue( member, value );
            }

            State.Clear();
        }

        /// <summary>
        /// Begins an edit transaction.
        /// </summary>
        /// <remarks>Nested transactions are not supported.</remarks>
        /// <exception cref="InvalidOperationException">A transaction has already been started.</exception>
        public void Begin()
        {
            if ( TransactionState == EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.NestedEditTransactionDetected );

            TransactionState = EditTransactionState.Started;
            OnBegin();
        }

        /// <summary>
        /// Commits an edit transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction has not been started.</exception>
        public void Commit()
        {
            if ( TransactionState != EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.EditTransactionNotStarted );

            OnCommit();
            TransactionState = EditTransactionState.Committed;
        }

        /// <summary>
        /// Creates an edit savepoint.
        /// </summary>
        /// <returns>An <see cref="IEditSavepoint"/> object.</returns>
        /// <remarks>This method can be invoked inside or outside of an edit transaction.</remarks>
        public virtual IEditSavepoint CreateSavepoint()
        {
            var stateBag = new Dictionary<string, object>();
            var autoCommit = TransactionState != EditTransactionState.Started;

            // force transaction to populate state bag
            if ( autoCommit )
                OnBegin();

            // snapshot values in current state bag so that the object can be properly restored
            foreach ( var item in State )
            {
                var value = EditSnapshotStrategy.GetSnapshot( item.Value );
                stateBag.Add( item.Key, value );
            }

            // commit transaction if necessary (this has no real net effect)
            if ( autoCommit )
                OnCommit();

            return new EditSavePoint( this, stateBag );
        }

        /// <summary>
        /// Rolls back an edit transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction has not been started.</exception>
        public void Rollback()
        {
            if ( TransactionState != EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.EditTransactionNotStarted );

            OnRollback();
            TransactionState = EditTransactionState.RolledBack;
        }

        /// <summary>
        /// Rolls back an edit transaction to the specified savepoint.
        /// </summary>
        /// <param name="savepoint">The <see cref="IEditSavepoint">savepoint</see> to roll back to.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public void Rollback( IEditSavepoint savepoint )
        {
            Arg.NotNull( savepoint, nameof( savepoint ) );

            State.ReplaceAll( savepoint.State );
            OnRollback();
            TransactionState = EditTransactionState.RolledBack;
        }
    }
}
