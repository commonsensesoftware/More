namespace System.ComponentModel.DataAnnotations
{
    using global::System;

    /// <summary>
    /// Indicates whether a data field is editable.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.EditableAttribute.</remarks>
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true )]
    public sealed class EditableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableAttribute" /> class.
        /// </summary>
        /// <param name="allowEdit">True to specify that field is editable; otherwise, false.</param>
        public EditableAttribute( bool allowEdit )
        {
            AllowEdit = allowEdit;
            AllowInitialValue = allowEdit;
        }

        /// <summary>
        /// Gets a value indicating whether a field is editable.
        /// </summary>
        /// <value>True if the field is editable; otherwise, false.</value>
        public bool AllowEdit
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an initial value is enabled.
        /// </summary>
        /// <value>True if an initial value is enabled; otherwise, false.</value>
        public bool AllowInitialValue
        {
            get;
            set;
        }
    }
}
