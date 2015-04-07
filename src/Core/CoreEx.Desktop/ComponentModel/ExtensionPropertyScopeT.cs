namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a scoped context for <see cref="TypeDescriptor">type descriptors</see> that describe
    /// extension properties<seealso cref="CustomTypeDescriptor{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to provide extension properties for.</typeparam>
    public class ExtensionPropertyScope<T> : IDisposable
    {
        private readonly bool allInstances;
        private readonly T instance;
        private readonly TypeDescriptionProvider<T> provider;
        private readonly CustomTypeDescriptor<T> descriptor = new CustomTypeDescriptor<T>();
        private bool disposed;

        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        ~ExtensionPropertyScope()
        {
            this.Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        /// <remarks>This constructor applies any defined extension properties to the all instances of type <typeparamref name="T"/>.</remarks>
        public ExtensionPropertyScope()
        {
            this.allInstances = true;
            this.provider = new TypeDescriptionProvider<T>( this.CreateFactory );
            TypeDescriptor.AddProvider( this.provider, typeof( T ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        /// <param name="instance">The instance to define extension properties for.</param>
        public ExtensionPropertyScope( T instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            this.instance = instance;
            this.provider = new TypeDescriptionProvider<T>( this.CreateFactory );
            TypeDescriptor.AddProvider( this.provider, this.instance );
        }

        private ICustomTypeDescriptor CreateFactory( ICustomTypeDescriptor parent )
        {
            return new CustomTypeDescriptor<T>( parent, this.descriptor );
        }

        private void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            if ( this.allInstances )
                TypeDescriptor.RemoveProvider( this.provider, typeof( T ) );
            else
                TypeDescriptor.RemoveProvider( this.provider, this.instance );
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Adds a property descriptor for the specified function representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <remarks>The specified function is typically an extension method. The name of the extension property is
        /// derived from the <see cref="P:MethodInfo.Name">method</see> defined by the provided <paramref name="accessor"/>.</remarks>
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
        ///         list.Add( new object() );
        ///         
        ///         using ( var properties = new ExtensionPropertyScope<List<object>>( list ) )
        ///         {
        ///             properties.Add( Enumerable.Any );
        ///             
        ///             var provider = TypeDescriptor.GetProvider( list );
        ///             var descriptor = provider.GetTypeDescriptor( list );
        ///             var any = descriptor.GetProperties()["Any"];
        ///             
        ///             // outputs 'True'
        ///             Console.WriteLine( any.GetValue( list ) );
        ///         }
        ///     }
        /// }
        /// ]]></code></example>
        public void Add<TValue>( Func<T, TValue> accessor )
        {
            Contract.Requires<ArgumentNullException>( accessor != null, "accessor" );
            this.descriptor.AddExtensionProperty( accessor.Method.Name, accessor );
        }

        /// <summary>
        /// Adds a property descriptor for the specified function representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <remarks>The specified function is typically an extension method.</remarks>
        /// <example>The following illustrates how to define a function that defines the accessor for a "Name" "extension property"
        /// to a <see cref="T:System.Collections.Generic.List{T}"/>. The defined "extension property" can
        /// be used in data binding expressions or imperatively accessed via a <see cref="System.ComponentModel.TypeDescriptor"/>.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.ComponentModel;
        /// 
        /// public class Program
        /// {
        ///     public static void Main( string[] args )
        ///     {
        ///         var list = new List<object>();
        ///         
        ///         using ( var properties = new ExtensionPropertyScope<List<object>>( list ) )
        ///         {
        ///             properties.Add( "Name", l => l.GetType().Name );
        ///             
        ///             var provider = TypeDescriptor.GetProvider( list );
        ///             var descriptor = provider.GetTypeDescriptor( list );
        ///             var name = descriptor.GetProperties()["Name"];
        ///             
        ///             // outputs 'List`1'
        ///             Console.WriteLine( name.GetValue( list ) );
        ///         }
        ///     }
        /// }
        /// ]]></code></example>
        public void Add<TValue>( string propertyName, Func<T, TValue> accessor )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Requires<ArgumentNullException>( accessor != null, "accessor" );
            this.descriptor.AddExtensionProperty( propertyName, accessor );
        }

        /// <summary>
        /// Adds a property descriptor for the specified function representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <param name="mutator">The <see cref="Action{T1,T2}">function</see> representing the property mutator. This parameter
        /// can be <c>null</c> if the property is meant to be read-only.</param>
        /// <remarks>The specified functions are typically extension methods.</remarks>
        /// <example>The following illustrates how to define a function that defines the accessor for a "Name" "extension property"
        /// to a <see cref="T:System.Collections.Generic.List{T}"/>. The defined "extension property" can
        /// be used in data binding expressions or imperatively accessed via a <see cref="System.ComponentModel.TypeDescriptor"/>.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.ComponentModel;
        /// 
        /// public class Person
        /// {
        ///     public string FirstName { get; set; }
        ///     public string LastName { get; set; }
        /// }
        /// 
        /// public static class PersonExtensions
        /// {
        ///     public static string GetFullName( this Person person )
        ///     {
        ///         return person.FirstName + " " + person.LastName;
        ///     }
        ///     
        ///     public static void SetFullName( this Person person, string value )
        ///     {
        ///         var parts = value.Split( " " );
        ///         person.FirstName = parts[0];
        ///         person.LastName = parts[1];
        ///     }
        /// }
        /// 
        /// public class Program
        /// {
        ///     public static void Main( string[] args )
        ///     {
        ///         var person = new Person()
        ///         {
        ///             FirstName = "John",
        ///             LastName = "Doe"
        ///         };
        ///         
        ///         using ( var properties = new ExtensionPropertyScope<Person>( person ) )
        ///         {
        ///             properties.Add( "FullName", PersonExtensions.GetFullName, PersonExtensions.SetFullName );
        ///             
        ///             var provider = TypeDescriptor.GetProvider( person );
        ///             var descriptor = provider.GetTypeDescriptor( person );
        ///             var fullName = descriptor.GetProperties()["FullName"];
        ///             
        ///             // outputs 'John Doe'
        ///             Console.WriteLine( fullName.GetValue( person ) );
        ///             
        ///             fullName.SetValue( person, "Jane Doe" );
        ///             
        ///             // outputs 'Jane'
        ///             Console.WriteLine( person.FirstName );
        ///         }
        ///     }
        /// }
        /// ]]></code></example>
        public void Add<TValue>( string propertyName, Func<T, TValue> accessor, Action<T, TValue> mutator )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Requires<ArgumentNullException>( accessor != null, "accessor" );
            this.descriptor.AddExtensionProperty( propertyName, accessor, mutator );
        }
    }
}
