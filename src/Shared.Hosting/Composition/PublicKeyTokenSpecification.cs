namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using static System.Byte;
    using static System.Globalization.CultureInfo;
    using static System.Globalization.NumberStyles;

    /// <summary>
    /// Represents a <see cref="ISpecification{T}">specification</see> for matching <see cref="AssemblyName">assembly names</see>
    /// according to their <see cref="AssemblyName.GetPublicKeyToken">public key token</see>.
    /// </summary>
    public class PublicKeyTokenSpecification : SpecificationBase<AssemblyName>, ISpecification<Type>
    {
        readonly Lazy<byte[]> publicKeyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        public PublicKeyTokenSpecification() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="publicKeyToken">The public key token in hexadecimal form.</param>
        public PublicKeyTokenSpecification( string publicKeyToken )
        {
            if ( !string.IsNullOrEmpty( publicKeyToken ) )
            {
                this.publicKeyToken = new Lazy<byte[]>( () => FromHex( publicKeyToken ) );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="publicKeyToken">The public key token in binary form.</param>
        public PublicKeyTokenSpecification( byte[] publicKeyToken ) => this.publicKeyToken = new Lazy<byte[]>( () => publicKeyToken );

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly">assembly</see> to get the public key token from.</param>
        public PublicKeyTokenSpecification( Assembly assembly )
        {
            Arg.NotNull( assembly, nameof( assembly ) );
            publicKeyToken = new Lazy<byte[]>( () => assembly.GetName().GetPublicKeyToken() );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to get the public key token from.</param>
        public PublicKeyTokenSpecification( Type type )
        {
            Arg.NotNull( type, nameof( type ) );
            publicKeyToken = new Lazy<byte[]>( () => type.GetTypeInfo().Assembly.GetName().GetPublicKeyToken() );
        }

        static byte[] FromHex( string token )
        {
            Contract.Requires( !string.IsNullOrEmpty( token ) );
            Contract.Requires( token.Length % 2 == 0 );
            Contract.Ensures( Contract.Result<byte[]>() != null );

            var bytes = new List<byte>();
            var culture = CurrentCulture;

            for ( var i = 0; i < token.Length; i += 2 )
            {
                bytes.Add( Parse( token.Substring( i, 2 ), HexNumber, culture ) );
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Evaluates whether the specified item matches the specification.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the specified <paramref name="item"/> matches the specification; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public override bool IsSatisfiedBy( AssemblyName item )
        {
            Arg.NotNull( item, nameof( item ) );

            var otherPublicKeyToken = item.GetPublicKeyToken();
            var otherIsEmptyOrNull = ( otherPublicKeyToken?.Length ?? 0 ) == 0;

            if ( ( publicKeyToken.Value?.Length ?? 0 ) == 0 )
            {
                return otherIsEmptyOrNull;
            }

            if ( otherIsEmptyOrNull )
            {
                return false;
            }

            return publicKeyToken.Value.SequenceEqual( otherPublicKeyToken );
        }

        /// <summary>
        /// Evaluates whether the specified item matches the specification.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the specified <paramref name="item"/> matches the specification; otherwise, false.</returns>
        public bool IsSatisfiedBy( Type item ) => IsSatisfiedBy( item.GetTypeInfo().Assembly.GetName() );

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'And' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        public ISpecification<Type> And( ISpecification<Type> other )
        {
            Arg.NotNull( other, nameof( other ) );
            return new LogicalAndSpecification<Type>( this, other );
        }

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'Or' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        public ISpecification<Type> Or( ISpecification<Type> other )
        {
            Arg.NotNull( other, nameof( other ) );
            return new LogicalOrSpecification<Type>( this, other );
        }

        /// <summary>
        /// Returns the logical complement of the specification.
        /// </summary>
        /// <returns>A <see cref="ISpecification{T}">specification</see> object.</returns>
        ISpecification<Type> ISpecification<Type>.Not() => new LogicalNotSpecification<Type>( this );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "A specification is a specialized rule. The interface is intentionally hidden." )]
        bool IRule<Type, bool>.Evaluate( Type item ) => IsSatisfiedBy( item );
    }
}