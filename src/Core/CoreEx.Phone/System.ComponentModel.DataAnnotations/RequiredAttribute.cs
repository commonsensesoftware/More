namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies that a data field value is required.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.RequiredAttribute.</remarks>
    [SuppressMessage( "Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Ported from System.ComponentModel.DataAnnotations." )]
    [AttributeUsage( AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class RequiredAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttribute" /> class.
        /// </summary>
        public RequiredAttribute()
            : base( (Func<string>) ( () => DataAnnotationsResources.RequiredAttribute_ValidationError ) )
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether an empty string is allowed.
        /// </summary>
        /// <value>True if an empty string is allowed; otherwise, false. The default value is false.</value>
        public bool AllowEmptyStrings
        {
            get;
            set;
        }

        /// <summary>
        /// Checks that the value of the required data field is not empty.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>True if validation is successful; otherwise, false.</returns>
        public override bool IsValid( object value )
        {
            if ( value == null )
                return false;
            
            var str = value as string;

            if ( ( str != null ) && !AllowEmptyStrings )
                return ( str.Trim().Length != 0 );

            return true;
        }
    }
}
