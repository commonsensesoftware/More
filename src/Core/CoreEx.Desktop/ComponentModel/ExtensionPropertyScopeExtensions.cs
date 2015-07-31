namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ExtensionPropertyScope{T}"/> class.
    /// </summary>
    public static class ExtensionPropertyScopeExtensions
    {
        /// <summary>
        /// Creates and returns a new extension property scope.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of object to define extension properties for.</typeparam>
        /// <param name="instance">The extended object to provide extension properties for.</param>
        /// <returns>A <see cref="ExtensionPropertyScope{T}"/> object.</returns>
        /// <example>The following illustrates how to apply the LINQ extension method <see cref="M:System.Linq.Enumerable.Any{T}">Any</see>
        /// as an "extension property" to a <see cref="T:System.Collections.Generic.List{T}"/>. The defined "extension property" can
        /// be used in data binding expressions or imperatively accessed via a <see cref="System.ComponentModel.TypeDescriptor"/>.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.ComponentModel;
        /// using System.Linq;
        /// 
        /// public class Program
        /// {
        ///     public static void Main( string[] args )
        ///     {
        ///         var list = new List<object>();
        ///         
        ///         using ( var properties = list.ExtensionProperties() )
        ///         {
        ///             properties.Add( Enumerable.Any );
        ///             
        ///             var provider = TypeDescriptor.GetProvider( list );
        ///             var descriptor = provider.GetTypeDescriptor( list );
        ///             var any = descriptor.GetProperties()["Any"];
        ///             
        ///             // outputs 'False+'
        ///             Console.WriteLine( any.GetValue( list ) );
        ///         }
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller." )]
        public static ExtensionPropertyScope<T> ExtensionProperties<T>( this T instance )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Contract.Ensures( Contract.Result<ExtensionPropertyScope<T>>() != null );
            return new ExtensionPropertyScope<T>( instance );
        }
    }
}
