namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Reflection;

    /// <summary>
    /// Represents a edit transation for object field members.
    /// </summary>
    public class PropertyTransaction : MemberTransaction<PropertyInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <remarks>All public, supported properties are enlisted.</remarks>
        public PropertyTransaction( object target )
            : base( target )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        /// <remarks>All public, supported properties are enlisted.</remarks>
        public PropertyTransaction( object target, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="propertyNames">A <see cref="IEnumerable{PropertyInfo}">sequence</see> of property names to enlist in the transaction.</param>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "propertyNames", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "propertyNames[]", Justification = "False positive" )]
        public PropertyTransaction( object target, IEnumerable<string> propertyNames )
            : base( target, propertyNames )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( propertyNames != null, "propertyNames" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="propertyNames">A <see cref="IEnumerable{PropertyInfo}">sequence</see> of property names to enlist in the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        public PropertyTransaction( object target, IEnumerable<string> propertyNames, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, propertyNames, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( propertyNames != null, "propertyNames" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="filter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted properties.</param>
        public PropertyTransaction( object target, Func<PropertyInfo, bool> filter )
            : base( target, filter )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( filter != null, "filter" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="filter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted properties.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        public PropertyTransaction( object target, Func<PropertyInfo, bool> filter, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, filter, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( filter != null, "memberFilter" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Returns the type for the specified property.
        /// </summary>
        /// <param name="member">The <see cref="PropertyInfo">property</see> to get the <see cref="Type">type</see> for.</param>
        /// <returns>A <see cref="Type">type</see> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override Type GetMemberType( PropertyInfo member )
        {
            return member.PropertyType;
        }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="member">The <see cref="PropertyInfo">property</see> to get the value for.</param>
        /// <returns>The value of the member.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override object GetMemberValue( PropertyInfo member )
        {
            return member.GetValue( this.Instance, null );
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="member">The <see cref="PropertyInfo">property</see> to set the value for.</param>
        /// <param name="value">The value to set.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override void SetMemberValue( PropertyInfo member, object value )
        {
            member.SetValue( this.Instance, value, null );
        }
    }
}
