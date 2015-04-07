namespace More.ComponentModel
{
    using More.Collections.Generic;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Reflection;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a edit transation for object members.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="MemberInfo">member</see> the transaction is for.</typeparam>
    [ContractClass( typeof( MemberTransactionContract<> ) )]
    public abstract partial class MemberTransaction<T> : IEditTransaction where T : MemberInfo
    {
        /// <summary>
        /// Represents an edit transaction savepoint.
        /// </summary>
        private sealed class EditSavePoint : IEditSavepoint
        {
            private readonly IEditTransaction transaction;
            private readonly IDictionary<string, object> state;

            internal EditSavePoint( IEditTransaction transaction, IDictionary<string, object> state )
            {
                Contract.Requires( transaction != null, "transaction" );
                Contract.Requires( state != null, "state" );
                this.transaction = transaction;
                this.state = state;
            }

            public IEditTransaction Transaction
            {
                get
                {
                    return this.transaction;
                }
            }

            public IDictionary<string, object> State
            {
                get
                {
                    return this.state;
                }
            }
        }

        /// <summary>
        /// Represents the default strategy used to take the snapshot of an editable object.
        /// </summary>
        private sealed partial class DefaultEditSnapshotStrategy : IEditSnapshotStrategy
        {
            private readonly static Lazy<DefaultEditSnapshotStrategy> instance = new Lazy<DefaultEditSnapshotStrategy>( () => new DefaultEditSnapshotStrategy() );

            private DefaultEditSnapshotStrategy()
            {
            }

            internal static DefaultEditSnapshotStrategy Instance
            {
                get
                {
                    return instance.Value;
                }
            }

            public object GetSnapshot( object value )
            {
                // REF: http://blogs.msdn.com/b/brada/archive/2004/05/03/125427.aspx
                // Silverlight, Windows Store Apps, and Windows Phone don't support ICloneable (it was explicitly excluded).
                // We also cannot use automatic deep cloning because binary serialization is not supported.

                // the value is null so nothing to do
                if ( value == null )
                    return null;

                // if cloning is supported, it's up to the implementor whether they supported shallow or deep cloning
                var cloneable = value.AsICloneable();

                if ( cloneable != null )
                    return cloneable.Clone();

                var cloneableOfT = value as ICloneable<object>;

                if ( cloneableOfT != null )
                    return cloneableOfT.Clone<object>();

                // the value doesn't support cloning or serialization so we have to take it as is
                return value;
            }
        }

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
            : this( target, member => true, DefaultEditSnapshotStrategy.Instance )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
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
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberNames">A <see cref="IEnumerable{T}">sequence</see> of member names to enlist in the transaction.</param>
        protected MemberTransaction( object target, IEnumerable<string> memberNames )
            : this( target, member => memberNames.Contains( member.Name ), DefaultEditSnapshotStrategy.Instance )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( memberNames != null, "memberNames" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberNames">A <see cref="IEnumerable{T}">sequence</see> of member names to enlist in the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        protected MemberTransaction( object target, IEnumerable<string> memberNames, IEditSnapshotStrategy editSnapshotStrategy )
            : this( target, member => memberNames.Contains( member.Name ), editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( memberNames != null, "memberNames" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTransaction{T}"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="memberFilter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted members.</param>
        protected MemberTransaction( object target, Func<T, bool> memberFilter )
            : this( target, memberFilter, DefaultEditSnapshotStrategy.Instance )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( memberFilter != null, "memberFilter" );
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
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( memberFilter != null, "memberFilter" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );

            this.instance = target;
            this.type = target.GetType();
            this.snapshotStrategy = editSnapshotStrategy;

            var allMembers = this.type.GetRuntimeFields().Cast<MemberInfo>().Union( this.type.GetRuntimeProperties() );
            var filteredMembers = allMembers.OfType<T>().Where( m => memberFilter( m ) && ShouldManage( m ) );

            this.members.AddRange( filteredMembers );
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
                return this.instance;
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
                return this.type;
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
                return this.members;
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
                return this.state;
            }
        }

        /// <summary>
        /// Gets the strategy used for snapshots.
        /// </summary>
        /// <value>An <see cref="IEditSnapshotStrategy"/> object.</value>
        protected IEditSnapshotStrategy EditSnapshotStrategy
        {
            get
            {
                return this.snapshotStrategy;
            }
        }

        private static bool ShouldManage( MemberInfo member )
        {
            Contract.Requires( member != null, "member" );

            var field = member as FieldInfo;

            if ( field != null )
            {
                // exclude read-only, constant, and non-public fields
                if ( field.IsInitOnly || field.IsLiteral || !field.IsPublic )
                    return false;
            }
            else
            {
                var property = member as PropertyInfo;

                if ( property != null )
                {
                    // make sure there is a get and set method; otherwise, the property is read-only
                    // check the setter first as write-only properties are extremely rare
                    if ( property.SetMethod == null || property.GetMethod == null )
                        return false;
                }
            }

            return IsEditable( member );
        }

        /// <summary>
        /// Returns a value indicating whether the specified member is editable.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo">member</see> to evaluate.</param>
        /// <returns>True if <paramref name="member"/> is editable; otherwise, false.</returns>
        /// <remarks>This property returns true if <paramref name="member"/> does not have the EditableAttribute applied or
        /// EditableAttribute is applied and the AllowEdit property is true.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected static bool IsEditable( MemberInfo member )
        {
            Contract.Requires<ArgumentNullException>( member != null, "member" );

            // UNDONE: System.ComponentModel.DataAnnotations is not currently portable; revert when possible
            //var attribute = member.GetCustomAttribute<EditableAttribute>( false );
            //return attribute == null || attribute.AllowEdit;

            dynamic attribute = member.GetCustomAttributes( false ).SingleOrDefault( a => a.GetType().Name == "System.ComponentModel.DataAnnotations.EditableAttribute" );
            return attribute == null || attribute.AllowEdit;
        }

        /// <summary>
        /// Creates and returns a state key for the specified member.
        /// </summary>
        /// <param name="member">The member of type <typeparamref name="T"/> to create a state key for.</param>
        /// <returns>A <see cref="String">string</see> representing the state key.</returns>
        protected virtual string CreateStateKey( T member )
        {
            Contract.Requires<ArgumentNullException>( member != null, "member" );
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
            foreach ( var member in this.Members )
            {
                var value = this.GetMemberValue( member );
                var key = this.CreateStateKey( member );
                var editable = value as IEditableObject;

                if ( editable != null )
                    editable.BeginEdit();

                this.State[key] = value;
            }
        }

        /// <summary>
        /// Occurs when a transaction is committed.
        /// </summary>
        protected virtual void OnCommit()
        {
            var editableType = typeof( IEditableObject ).GetTypeInfo();
            var editableMembers = from member in this.Members
                                  where editableType.IsAssignableFrom( this.GetMemberType( member ).GetTypeInfo() )
                                  let key = this.CreateStateKey( member )
                                  let editable = (IEditableObject) this.State[key]
                                  where editable != null
                                  select editable;

            editableMembers.ForEach( e => e.EndEdit() );
            this.State.Clear();
        }

        /// <summary>
        /// Occurs when a transaction is rolled back.
        /// </summary>
        protected virtual void OnRollback()
        {
            foreach ( var member in this.Members )
            {
                var key = this.CreateStateKey( member );
                var value = this.State[key];
                var editable = value as IEditableObject;

                if ( editable != null )
                    editable.CancelEdit();

                this.SetMemberValue( member, value );
            }

            this.State.Clear();
        }

        /// <summary>
        /// Begins an edit transaction.
        /// </summary>
        /// <remarks>Nested transactions are not supported.</remarks>
        /// <exception cref="InvalidOperationException">A transaction has already been started.</exception>
        public void Begin()
        {
            if ( this.TransactionState == EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.NestedEditTransactionDetected );

            this.TransactionState = EditTransactionState.Started;
            this.OnBegin();
        }

        /// <summary>
        /// Commits an edit transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction has not been started.</exception>
        public void Commit()
        {
            if ( this.TransactionState != EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.EditTransactionNotStarted );

            this.OnCommit();
            this.TransactionState = EditTransactionState.Committed;
        }

        /// <summary>
        /// Creates an edit savepoint.
        /// </summary>
        /// <returns>An <see cref="IEditSavepoint"/> object.</returns>
        /// <remarks>This method can be invoked inside or outside of an edit transaction.</remarks>
        public virtual IEditSavepoint CreateSavepoint()
        {
            var stateBag = new Dictionary<string, object>();
            var autoCommit = this.TransactionState != EditTransactionState.Started;

            // force transaction to populate state bag
            if ( autoCommit )
                this.OnBegin();

            // snapshot values in current state bag so that the object can be properly restored
            foreach ( var item in this.State )
            {
                var value = this.EditSnapshotStrategy.GetSnapshot( item.Value );
                stateBag.Add( item.Key, value );
            }

            // commit transaction if necessary (this has no real net effect)
            if ( autoCommit )
                this.OnCommit();

            return new EditSavePoint( this, stateBag.AsReadOnly() );
        }

        /// <summary>
        /// Rolls back an edit transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction has not been started.</exception>
        public void Rollback()
        {
            if ( this.TransactionState != EditTransactionState.Started )
                throw new InvalidOperationException( ExceptionMessage.EditTransactionNotStarted );

            this.OnRollback();
            this.TransactionState = EditTransactionState.RolledBack;
        }

        /// <summary>
        /// Rolls back an edit transaction to the specified savepoint.
        /// </summary>
        /// <param name="savepoint">The <see cref="IEditSavepoint">savepoint</see> to roll back to.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public void Rollback( IEditSavepoint savepoint )
        {
            this.State.ReplaceAll( savepoint.State );
            this.OnRollback();
            this.TransactionState = EditTransactionState.RolledBack;
        }
    }
}
