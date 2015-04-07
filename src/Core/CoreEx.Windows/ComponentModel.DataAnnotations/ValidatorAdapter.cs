namespace More.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Provides additional implementation specific to Windows Store applications.
    /// </summary>
    public partial class ValidatorAdapter : IValidator
    {
        /// <summary>
        /// Creates and returns a new validation context.
        /// </summary>
        /// <param name="instance">The object to create the context for.</param>
        /// <param name="items">The dictionary of key/value pairs that is associated with this context.</param>
        /// <returns>A new validation context.</returns>
        public virtual IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            var context = new ValidationContext( instance, items );
            context.InitializeServiceProvider( ServiceProvider.Current.GetService );
            return new ValidationContextAdapter( context );
        }
    }
}
