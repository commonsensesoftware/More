namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal sealed class HostAssemblySpecification : SpecificationBase<Type>
    {
        private readonly Lazy<byte[]> publicKeyToken = new Lazy<byte[]>( GetPublicKeyToken );

        private static byte[] GetPublicKeyToken()
        {
            var assembly = typeof( HostAssemblySpecification ).GetTypeInfo().Assembly;
            var publicKeyToken = assembly.GetName().GetPublicKeyToken();

            return publicKeyToken;
        }

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            var otherAssembly = item.GetTypeInfo().Assembly;
            var otherPublicKeyToken = otherAssembly.GetName().GetPublicKeyToken();

            if ( otherPublicKeyToken == null )
                return false;

            // match an assembly that has the same public key token
            return this.publicKeyToken.Value.SequenceEqual( otherPublicKeyToken );
        }
    }
}
