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
    /// according to their <see cref="M:AssemblyName.GetPublicKeyToken">public key token</see>.
    /// </summary>
    public class PublicKeyTokenSpecification : SpecificationBase<AssemblyName>
    {
        private readonly byte[] publicKeyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        public PublicKeyTokenSpecification()
        {
            publicKeyToken = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="publicKeyToken">The public key token in hexadecimal form.</param>
        public PublicKeyTokenSpecification( string publicKeyToken )
        {
            if ( !string.IsNullOrEmpty( publicKeyToken ) )
                this.publicKeyToken = FromHex( publicKeyToken );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyTokenSpecification"/> class.
        /// </summary>
        /// <param name="publicKeyToken">The public key token in binary form.</param>
        public PublicKeyTokenSpecification( byte[] publicKeyToken )
        {
            this.publicKeyToken = publicKeyToken;
        }

        private static bool IsNull( byte[] array ) => array?.Length == 0;

        private static byte[] FromHex( string token )
        {
            Contract.Requires( !string.IsNullOrEmpty( token ) );
            Contract.Requires( token.Length % 2 == 0 );
            Contract.Ensures( Contract.Result<byte[]>() != null );

            var bytes = new List<byte>();
            var culture = CurrentCulture;

            for ( var i = 0; i < token.Length; i += 2 )
                bytes.Add( Parse( token.Substring( i, 2 ), HexNumber, culture ) );

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

            if ( IsNull( publicKeyToken ) )
                return IsNull( otherPublicKeyToken );

            if ( IsNull( otherPublicKeyToken ) )
                return false;

            return publicKeyToken.SequenceEqual( otherPublicKeyToken );
        }
    }
}
