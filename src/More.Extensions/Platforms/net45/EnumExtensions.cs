namespace System
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <content>
    /// Provides additional implementation specific to the Windows Desktop.
    /// </content>
    public static partial class EnumExtensions
    {
        static partial void GetFromDescriptionAttribute( IEnumerable<object> attributes, ref string description )
        {
            Contract.Requires( attributes != null );

            var attribute = attributes.OfType<DescriptionAttribute>().FirstOrDefault();

            if ( attribute != null )
            {
                description = attribute.Description;
            }
        }
    }
}