namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics.Contracts;

    /// <content>
    /// Provides implementation for all implementation expect Windows Store Apps.
    /// </content>
    public partial class ValueConversionStep
    {
        /// <summary>
        /// Gets or sets the target <see cref="Type">type</see> for the conversion step.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        [TypeConverter( typeof( TypeNameConverter ) )]
        public Type TargetType
        {
            get
            {
                Contract.Ensures( this.targetType != null );
                return this.targetType;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.targetType = value;
            }
        }
    }
}
