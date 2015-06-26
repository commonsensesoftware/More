namespace More
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="WeakReference">weak</see> <see cref="Delegate">delegate</see>.
    /// </summary>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an appropriate name for a weakly referenced delegate." )]
    public class WeakDelegate : WeakReference
    {
        private static readonly TypeInfo DelegateType = typeof( Delegate ).GetTypeInfo();
        private readonly Type type;
        private readonly MethodInfo method;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakDelegate"/> class.
        /// </summary>
        /// <param name="strongDelegate">The strong <see cref="Delegate">delegate</see> to make weak.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public WeakDelegate( Delegate strongDelegate )
            : base( GetTarget( strongDelegate ) )
        {


            this.type = strongDelegate.GetType();
            this.method = strongDelegate.GetMethodInfo();
        }

        private static object GetTarget( Delegate strongDelegate )
        {
            if ( strongDelegate == null )
                throw new ArgumentException( SR.InvalidArgType.FormatDefault( typeof( Delegate ) ), "strongDelegate" );

            return strongDelegate.Target;
        }

        /// <summary>
        /// Returns a value indicating whether the specified type is a delegate.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to evaluate.</param>
        /// <returns>True if the specified <paramref name="type"/> is a <see cref="Delegate">delegate</see>; otherwise, false.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool IsDelegateType( Type type )
        {
            Arg.NotNull( type, "type" );
            return DelegateType.IsAssignableFrom( type.GetTypeInfo() );
        }

        /// <summary>
        /// Returns a value indicating whether the current weak delegate is a match (equivalent) for the specified strong delegate.
        /// </summary>
        /// <param name="strongDelegate">The <see cref="Delegate">delegate</see> to evaluate.</param>
        /// <returns>True if the current weak delegate is a match for the specified strong delegate; otherwise, false.</returns>
        public virtual bool IsMatch( Delegate strongDelegate )
        {
            if ( strongDelegate == null )
                return false;

            if ( !object.Equals( strongDelegate.Target, this.Target ) )
                return false;

            if ( !strongDelegate.GetMethodInfo().Equals( this.method ) )
                return false;

            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the current weak delegate is covariant with the
        /// specified method signature parameter types, which does not contain a return value.
        /// </summary>
        /// <param name="parameterTypes">The parameter <see cref="Type">types</see> that define
        /// the method signature to evaluate.</param>
        /// <returns>True if the current weak delegate is covariant with the specified method
        /// signature; otherwise, false.</returns>
        public bool IsCovariantWithMethod( params Type[] parameterTypes )
        {
            return this.IsCovariantWithFunction( typeof( void ), parameterTypes );
        }

        /// <summary>
        /// Returns a value indicating whether the current weak delegate is covariant with the
        /// specified method signature parameter types.
        /// </summary>
        /// <param name="returnType">The <see cref="Type">type</see> of return value.</param>
        /// <param name="parameterTypes">The parameter <see cref="Type">types</see> that define
        /// the method signature to evaluate.</param>
        /// <returns>True if the current weak delegate is covariant with the specified method
        /// signature; otherwise, false.</returns>
        public bool IsCovariantWithFunction( Type returnType, params Type[] parameterTypes )
        {
            Arg.NotNull( returnType, "returnType" );

            if ( !this.method.ReturnType.GetTypeInfo().IsAssignableFrom( returnType.GetTypeInfo() ) )
                return false;

            if ( parameterTypes == null )
                parameterTypes = new Type[0];

            var args = this.method.GetParameters();

            if ( args.Length != parameterTypes.Length )
                return false;

            for ( var i = 0; i < args.Length; i++ )
            {
                var sourceType = parameterTypes[i].GetTypeInfo();
                var targetType = args[i].ParameterType.GetTypeInfo();

                if ( !targetType.IsAssignableFrom( sourceType ) )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates and returns a strongly referenced delegate.
        /// </summary>
        /// <returns>The <see cref="Delegate">delegate</see> created or null if the delegate cannot be created.</returns>
        public virtual Delegate CreateDelegate()
        {
            object target = this.Target;

            if ( target != null )
                return this.method.CreateDelegate( this.type, target );

            if ( this.method.IsStatic )
                return this.method.CreateDelegate( this.type );

            return null;
        }
    }
}
