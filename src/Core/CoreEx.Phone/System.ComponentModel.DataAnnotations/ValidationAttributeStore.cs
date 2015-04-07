namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationAttributeStore.
    /// </summary>
    internal class ValidationAttributeStore
    {
        private abstract class StoreItem
        {
            private readonly IEnumerable<ValidationAttribute> validationAttributes;

            internal StoreItem( IEnumerable<Attribute> attributes )
            {
                this.validationAttributes = attributes.OfType<ValidationAttribute>();
                this.DisplayAttribute = attributes.OfType<DisplayAttribute>().SingleOrDefault<DisplayAttribute>();
            }

            internal IEnumerable<ValidationAttribute> ValidationAttributes
            {
                get
                {
                    return this.validationAttributes;
                }
            }

            internal DisplayAttribute DisplayAttribute
            {
                get;
                set;
            }
        }

        /// <summary>
        /// This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationAttributeStore+TypeStoreItem.
        /// </summary>
        private sealed partial class TypeStoreItem : StoreItem
        {
            private readonly object syncRoot = new object();
            private readonly Type type;
            private Dictionary<string, PropertyStoreItem> propertyStoreItems;

            internal TypeStoreItem( Type type, IEnumerable<Attribute> attributes )
                : base( attributes )
            {
                this.type = type;
            }

            internal PropertyStoreItem GetPropertyStoreItem( string propertyName )
            {
                PropertyStoreItem result = null;

                if ( !this.TryGetPropertyStoreItem( propertyName, out result ) )
                {
                    var message = DataAnnotationsResources.UnknownProperty.FormatDefault( this.type.Name, propertyName );
                    throw new ArgumentException( message, "propertyName" );
                }

                return result;
            }

            internal bool TryGetPropertyStoreItem( string propertyName, out PropertyStoreItem item )
            {
                if ( string.IsNullOrEmpty( propertyName ) )
                    throw new ArgumentNullException( "propertyName" );

                if ( this.propertyStoreItems == null )
                {
                    lock ( this.syncRoot )
                    {
                        if ( this.propertyStoreItems == null )
                            this.propertyStoreItems = this.CreatePropertyStoreItems();
                    }
                }
                return this.propertyStoreItems.TryGetValue( propertyName, out item );
            }

            internal static IEnumerable<Attribute> GetExplicitAttributes( PropertyInfo propertyDescriptor )
            {
                var list = new List<Attribute>( propertyDescriptor.GetCustomAttributes<Attribute>( true ) );
                var enumerable = propertyDescriptor.PropertyType.GetTypeInfo().GetCustomAttributes<Attribute>();

                foreach ( var current in enumerable )
                {
                    for ( int i = list.Count - 1; i >= 0; i-- )
                    {
                        if ( object.ReferenceEquals( current, list[i] ) )
                        {
                            list.RemoveAt( i );
                        }
                    }
                }

                return list;
            }

            private Dictionary<string, PropertyStoreItem> CreatePropertyStoreItems()
            {
                var dictionary = new Dictionary<string, PropertyStoreItem>();
                var properties = this.type.GetRuntimeProperties();

                foreach ( var propertyDescriptor in properties )
                {
                    var attribues = TypeStoreItem.GetExplicitAttributes( propertyDescriptor ).Cast<Attribute>();
                    var value = new ValidationAttributeStore.PropertyStoreItem( propertyDescriptor.PropertyType, attribues );
                    dictionary[propertyDescriptor.Name] = value;
                }

                return dictionary;
            }
        }

        private sealed class PropertyStoreItem : StoreItem
        {
            private readonly Type propertyType;

            internal PropertyStoreItem( Type propertyType, IEnumerable<Attribute> attributes )
                : base( attributes )
            {
                this.propertyType = propertyType;
            }

            internal Type PropertyType
            {
                get
                {
                    return this.propertyType;
                }
            }
        }

        private static readonly ValidationAttributeStore singleton = new ValidationAttributeStore();
        private readonly Dictionary<Type, TypeStoreItem> typeStoreItems = new Dictionary<Type, TypeStoreItem>();

        internal static ValidationAttributeStore Instance
        {
            get
            {
                return singleton;
            }
        }

        private ValidationAttributeStore.TypeStoreItem GetTypeStoreItem( Type type )
        {
            if ( type == null )
                throw new ArgumentNullException( "type" );

            TypeStoreItem result;

            lock ( this.typeStoreItems )
            {
                TypeStoreItem typeStoreItem = null;

                if ( !this.typeStoreItems.TryGetValue( type, out typeStoreItem ) )
                {
                    var attributes = type.GetTypeInfo().GetCustomAttributes<Attribute>();
                    typeStoreItem = new TypeStoreItem( type, attributes );
                    this.typeStoreItems[type] = typeStoreItem;
                }

                result = typeStoreItem;
            }

            return result;
        }

        internal IEnumerable<ValidationAttribute> GetTypeValidationAttributes( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            return typeStoreItem.ValidationAttributes;
        }

        internal DisplayAttribute GetTypeDisplayAttribute( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            return typeStoreItem.DisplayAttribute;
        }

        internal IEnumerable<ValidationAttribute> GetPropertyValidationAttributes( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            var propertyStoreItem = typeStoreItem.GetPropertyStoreItem( validationContext.MemberName );
            return propertyStoreItem.ValidationAttributes;
        }

        internal DisplayAttribute GetPropertyDisplayAttribute( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            var propertyStoreItem = typeStoreItem.GetPropertyStoreItem( validationContext.MemberName );
            return propertyStoreItem.DisplayAttribute;
        }

        internal Type GetPropertyType( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            var propertyStoreItem = typeStoreItem.GetPropertyStoreItem( validationContext.MemberName );
            return propertyStoreItem.PropertyType;
        }

        internal bool IsPropertyContext( ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var typeStoreItem = this.GetTypeStoreItem( validationContext.ObjectType );
            PropertyStoreItem propertyStoreItem = null;
            return typeStoreItem.TryGetPropertyStoreItem( validationContext.MemberName, out propertyStoreItem );
        }
    }
}
