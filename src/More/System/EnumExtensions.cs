namespace System
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="Enum"/> type.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the display name for an enumeration value.
        /// </summary>
        /// <param name="enumValue">The <see cref="Enum">enumeration</see> to get a display name from.</param>
        /// <returns>The display name for the enumeration or the enumeration name if no display name is defined.</returns>
        /// <example>This example demonstrates to define and retrieve the display name for an enumeration.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel.DataAnnotations;
        /// 
        /// public enum WeekPart
        /// {
        ///    [Display( Name = "First Day of the Week" )]
        ///    FirstDayOfWeek,
        ///    
        ///    [Display( Name = "Last Day of the Week" )]
        ///    LastDayOfWeek
        /// }
        /// 
        /// Console.WriteLine( WeekPart.FirstDayOfWeek.GetDisplayName() );
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive.  System.Enum is a value type and can never be null." )]
        public static string GetDisplayName( this Enum enumValue )
        {
            Contract.Ensures( Contract.Result<string>() != null );

            var name = enumValue.GetDisplayAttribute()?.Name;

            if ( string.IsNullOrEmpty( name ) )
            {
                name = enumValue.ToString();
            }

            return name;
        }

        /// <summary>
        /// Returns the description for an enumeration value.
        /// </summary>
        /// <param name="enumValue">The <see cref="Enum">enumeration</see> to get a description from.</param>
        /// <returns>The description for the enumeration or the enumeration name if no description is defined.</returns>
        /// <remarks>The description value will first be evaluated from the <see cref="P:DisplayAttribute.Description"/> property.</remarks>
        /// <example>This example demonstrates to define and retrieve the description for an enumeration. The DescriptionAttribute can only
        /// be used in a Windows Desktop application.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.ComponentModel.DataAnnotations;
        /// 
        /// public enum WeekPart
        /// {
        ///    [Display( Description = "First Day of the Week" )]
        ///    FirstDayOfWeek,
        ///    
        ///    [Display( Description = "Last Day of the Week" )]
        ///    LastDayOfWeek
        /// }
        /// 
        /// Console.WriteLine( WeekPart.FirstDayOfWeek.GetDescription() );
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive.  System.Enum is a value type and can never be null." )]
        public static string GetDescription( this Enum enumValue )
        {
            Contract.Ensures( Contract.Result<string>() != null );

            var description = enumValue.GetDisplayAttribute()?.Description;

            if ( string.IsNullOrEmpty( description ) )
            {
                description = enumValue.ToString();
            }

            return description;
        }

        static DisplayAttribute GetDisplayAttribute( this Enum enumValue )
        {
            var type = enumValue.GetType();
            var name = enumValue.ToString();
            var field = type.GetRuntimeField( name );

            if ( field == null )
            {
                return null;
            }

            return field.GetCustomAttributes( false ).OfType<DisplayAttribute>().SingleOrDefault();
        }
    }
}