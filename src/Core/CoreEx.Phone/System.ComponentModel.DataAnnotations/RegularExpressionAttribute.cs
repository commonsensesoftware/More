namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Globalization;
    using global::System.Text.RegularExpressions;

    /// <summary>
    /// Specifies that a data field value must match the specified regular expression.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.RegularExpressionAttribute.</remarks>
    [AttributeUsage( AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class RegularExpressionAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegularExpressionAttribute"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression that is used to validate the data field value.</param>
        public RegularExpressionAttribute( string pattern )
            : base( (Func<string>) ( () => DataAnnotationsResources.RegexAttribute_ValidationError ) )
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        /// <value>The pattern to match.</value>
        public string Pattern
        {
            get;
            private set;
        }

        private Regex Regex
        {
            get;
            set;
        }

        private void SetupRegex()
        {
            if ( this.Regex == null )
            {
                if ( string.IsNullOrEmpty( this.Pattern ) )
                {
                    throw new InvalidOperationException( DataAnnotationsResources.RegularExpressionAttribute_Empty_Pattern );
                }
                this.Regex = new Regex( this.Pattern );
            }
        }

        /// <summary>
        /// Formats the error message to display if the regular expression validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage( string name )
        {
            this.SetupRegex();
            return string.Format( CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name, this.Pattern } );
        }

        /// <summary>
        /// Checks whether the value entered by the user matches the regular expression pattern.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>True if validation is successful; otherwise, false.</returns>
        public override bool IsValid( object value )
        {
            this.SetupRegex();
            var str = Convert.ToString( value, CultureInfo.CurrentCulture );

            if ( string.IsNullOrEmpty( str ) )
            {
                return true;
            }

            var match = this.Regex.Match( str );
            return ( ( match.Success && ( match.Index == 0 ) ) && ( match.Length == str.Length ) );
        }
    }
}
