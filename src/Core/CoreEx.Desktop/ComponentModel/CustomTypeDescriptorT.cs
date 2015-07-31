namespace More.ComponentModel
{
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a generic <see cref="CustomTypeDescriptor">custom type descriptor</see>
    /// <seealso cref="ExtensionMethodToPropertyDescriptor{TObject,TValue}"/>
    /// <seealso cref="ExtensionPropertyScope{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> to create a custom type descriptor for.</typeparam>
    public class CustomTypeDescriptor<T> : CustomTypeDescriptor
    {
        private readonly ObservableKeyedCollection<string, PropertyDescriptor> extensionProperties = new ObservableKeyedCollection<string, PropertyDescriptor>( p => p.Name );

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTypeDescriptor{T}"/> class.
        /// </summary>
        public CustomTypeDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTypeDescriptor{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="ICustomTypeDescriptor">type descriptor</see>.</param>
        public CustomTypeDescriptor( ICustomTypeDescriptor parent )
            : base( parent )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTypeDescriptor{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="ICustomTypeDescriptor">type descriptor</see>.</param>
        /// <param name="other">The other <see cref="CustomTypeDescriptor{T}">type descriptor</see> to initialize the new instance from.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public CustomTypeDescriptor( ICustomTypeDescriptor parent, CustomTypeDescriptor<T> other )
            : base( parent )
        {
            Arg.NotNull( other, nameof( other ) );
            extensionProperties.AddRange( other.extensionProperties );
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
        ///         Func<ICustomTypeDescriptor, ICustomTypeDescriptor> factory = parent =>
        ///         {
        ///             var customDescriptor = new CustomTypeDescriptor<T>( parent );
        ///             customDescriptor.AddExtensionProperty( Enumerable.Any );
        ///             return customDescriptor;
        ///         };
        ///         var customProvider = new TypeDescriptionProvider<List<object>>( factory );
        ///         TypeDescriptor.AddProvider( customProvider, list );
        ///
        ///         var provider = TypeDescriptor.GetProvider( list );
        ///         var descriptor = provider.GetTypeDescriptor( list );
        ///         var any = descriptor.GetProperties()["Any"];
        ///             
        ///         // outputs 'True'
        ///         Console.WriteLine( any.GetValue( list ) );
        ///         
        ///         TypeDescriptor.RemoveProvider( customProvider, list );
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public void AddExtensionProperty<TValue>( Func<T, TValue> accessor )
        {
            Arg.NotNull( accessor, nameof( accessor ) );
            AddExtensionProperty( accessor.Method.Name, accessor );
        }

        /// <summary>
        /// Adds a property descriptor for extensions methods representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
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
        ///         Func<ICustomTypeDescriptor, ICustomTypeDescriptor> factory = parent =>
        ///         {
        ///             var customDescriptor = new CustomTypeDescriptor<T>( parent );
        ///             customDescriptor.AddExtensionProperty( "Name", l => l.GetType().Name );
        ///             return customDescriptor;
        ///         };
        ///         var customProvider = new TypeDescriptionProvider<List<object>>( factory );
        ///         TypeDescriptor.AddProvider( customProvider, list );
        ///
        ///         var provider = TypeDescriptor.GetProvider( list );
        ///         var descriptor = provider.GetTypeDescriptor( list );
        ///         var name = descriptor.GetProperties()["Name"];
        ///             
        ///         // outputs 'List`1'
        ///         Console.WriteLine( name.GetValue( list ) );
        ///         
        ///         TypeDescriptor.RemoveProvider( customProvider, list );
        ///     }
        /// }
        /// ]]></code></example>
        public void AddExtensionProperty<TValue>( string propertyName, Func<T, TValue> accessor )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Arg.NotNull( accessor, nameof( accessor ) );
            AddExtensionProperty( propertyName, accessor, null );
        }

        /// <summary>
        /// Adds a property descriptor for extensions methods representing a property.
        /// </summary>
        /// <typeparam name="TValue">The property <see cref="Type">type</see>.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <param name="mutator">The <see cref="Action{T1,T2}">function</see> representing the property mutator. This parameter
        /// can be null if the property is meant to be read-only.</param>
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
        ///         Func<ICustomTypeDescriptor, ICustomTypeDescriptor> factory = parent =>
        ///         {
        ///             var customDescriptor = new CustomTypeDescriptor<T>( parent );
        ///             customDescriptor.AddExtensionProperty( "Name", l => l.GetType().Name );
        ///             return customDescriptor;
        ///         };
        ///         var customProvider = new TypeDescriptionProvider<List<object>>( factory );
        ///         TypeDescriptor.AddProvider( customProvider, list );
        ///         
        ///         var provider = TypeDescriptor.GetProvider( person );
        ///         var descriptor = provider.GetTypeDescriptor( person );
        ///         var fullName = descriptor.GetProperties()["FullName"];
        ///         
        ///         // outputs 'John Doe'
        ///         Console.WriteLine( fullName.GetValue( person ) );
        ///             
        ///         fullName.SetValue( person, "Jane Doe" );
        ///             
        ///         // outputs 'Jane'
        ///         Console.WriteLine( person.FirstName );
        ///         
        ///         TypeDescriptor.RemoveProvider( customProvider, list );
        ///     }
        /// }
        /// ]]></code></example>
        public virtual void AddExtensionProperty<TValue>( string propertyName, Func<T, TValue> accessor, Action<T, TValue> mutator )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Arg.NotNull( accessor, nameof( accessor ) );

            if ( extensionProperties.Contains( propertyName ) )
                extensionProperties.Remove( propertyName );

            var property = new ExtensionMethodToPropertyDescriptor<T, TValue>( propertyName, accessor, mutator );
            extensionProperties.Add( property );
        }

        /// <summary>
        /// Gets the name of the type descriptor is for.
        /// </summary>
        /// <returns>The type name represented by <typeparamref name="T"/>.</returns>
        public override string GetClassName()
        {
            return TypeDescriptor.GetClassName( typeof( T ) );
        }

        /// <summary>
        /// Gets the properties defined by the type descriptor.
        /// </summary>
        /// <returns>A <see cref="PropertyDescriptorCollection"/> object.</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties( null );
        }

        /// <summary>
        /// Gets the properties defined by the type descriptor.
        /// </summary>
        /// <param name="attributes">The <see cref="Attribute">attributes</see> used to filter properties.</param>
        /// <returns>A <see cref="PropertyDescriptorCollection"/> object.</returns>
        public override PropertyDescriptorCollection GetProperties( Attribute[] attributes )
        {
            var properties = new List<PropertyDescriptor>( base.GetProperties( attributes ).Cast<PropertyDescriptor>() );
            properties.AddRange( extensionProperties );
            return new PropertyDescriptorCollection( properties.ToArray(), true );
        }

        /// <summary>
        /// Gets the owner of the property.
        /// </summary>
        /// <param name="pd">The <see cref="PropertyDescriptor"/> to get the owner for.</param>
        /// <returns>This method always returns the current type descriptor instance.</returns>
        public override object GetPropertyOwner( PropertyDescriptor pd )
        {
            return this;
        }
    }
}
