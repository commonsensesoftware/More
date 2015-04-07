namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    internal sealed class PropertyDescriptor
    {
        private readonly PropertyInfo property;
        private readonly object[] index;

        internal PropertyDescriptor( PropertyInfo property, object[] index )
        {
            Contract.Requires( property != null );
            this.property = property;
            this.index = index;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Consistent with a property descriptor. Reserved for future use." )]
        public object GetValue( object obj )
        {
            return this.property.GetValue( obj, this.index );
        }

        public void SetValue( object obj, object value )
        {
            this.property.SetValue( obj, value, this.index );
        }
    }
}
