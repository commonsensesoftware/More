namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a scoped context for <see cref="TypeDescriptor">type descriptors</see> that describe
    /// extension properties<seealso cref="CustomTypeDescriptor{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to provide extension properties for.</typeparam>
    public class ExtensionPropertyScope<T> : IDisposable
    {
        readonly bool allInstances;
        readonly T instance;
        readonly TypeDescriptionProvider<T> provider;
        readonly CustomTypeDescriptor<T> descriptor = new CustomTypeDescriptor<T>();
        bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        ~ExtensionPropertyScope() => Dispose( false );

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        /// <remarks>This constructor applies any defined extension properties to the all instances of type <typeparamref name="T"/>.</remarks>
        public ExtensionPropertyScope()
        {
            allInstances = true;
            provider = new TypeDescriptionProvider<T>( CreateFactory );
            TypeDescriptor.AddProvider( provider, typeof( T ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        /// <param name="instance">The instance to define extension properties for.</param>
        public ExtensionPropertyScope( T instance )
        {
            Arg.NotNull( instance, nameof( instance ) );

            this.instance = instance;
            provider = new TypeDescriptionProvider<T>( CreateFactory );
            TypeDescriptor.AddProvider( provider, this.instance );
        }

        ICustomTypeDescriptor CreateFactory( ICustomTypeDescriptor parent ) => new CustomTypeDescriptor<T>( parent, descriptor );

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            if ( allInstances )
            {
                TypeDescriptor.RemoveProvider( provider, typeof( T ) );
            }
            else
            {
                TypeDescriptor.RemoveProvider( provider, instance );
            }
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="ExtensionPropertyScope{T}"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Adds a property descriptor for the specified function representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <remarks>The specified function is typically an extension method. The name of the extension property is
        /// derived from the <see cref="MemberInfo.Name">method</see> defined by the provided <paramref name="accessor"/>.</remarks>
        /// <example>The following illustrates how to apply the LINQ extension method <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})">Any</see>
        /// as an "extension property" to a <see cref="List{T}"/>. The defined "extension property" can
        /// be used in data binding expressions or imperatively accessed via a <see cref="TypeDescriptor"/>.
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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public void Add<TValue>( Func<T, TValue> accessor )
        {
            Arg.NotNull( accessor, nameof( accessor ) );
#if NET45
            descriptor.AddExtensionProperty( accessor.Method.Name, accessor );
#else
            descriptor.AddExtensionProperty( accessor.GetMethodInfo().Name, accessor );
#endif
        }

        /// <summary>
        /// Adds a property descriptor for the specified function representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <remarks>The specified function is typically an extension method.</remarks>
        /// <example>The following illustrates how to define a function that defines the accessor for a "Name" "extension property"
        /// to a <see cref="System.Collections.Generic.List{T}"/>. The defined "extension property" can
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
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Arg.NotNull( accessor, nameof( accessor ) );
            descriptor.AddExtensionProperty( propertyName, accessor );
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
        /// to a <see cref="System.Collections.Generic.List{T}"/>. The defined "extension property" can
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
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Arg.NotNull( accessor, nameof( accessor ) );
            descriptor.AddExtensionProperty( propertyName, accessor, mutator );
        }
    }
}