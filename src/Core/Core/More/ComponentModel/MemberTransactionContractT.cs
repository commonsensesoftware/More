namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    [ContractClassFor( typeof( MemberTransaction<> ) )]
    internal abstract class MemberTransactionContract<T> : MemberTransaction<T> where T : MemberInfo
    {
        protected override Type GetMemberType( T member )
        {
            Contract.Requires<ArgumentNullException>( member != null, "member" );
            Contract.Ensures( Contract.Result<Type>() != null );
            return default( Type );
        }

        protected override object GetMemberValue( T member )
        {
            Contract.Requires<ArgumentNullException>( member != null, "member" );
            return default( object );
        }

        protected override void SetMemberValue( T member, object value )
        {
            Contract.Requires<ArgumentNullException>( member != null, "member" );
        }

        protected MemberTransactionContract()
            : base( default( object ) )
        {
        }
    }
}
