namespace System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel.DataAnnotations;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="System.Enum"/> type.
    /// </summary>
    public static partial class EnumExtensions
    {
        private static IEnumerable<object> GetAttributes( this Enum enumValue )
        {
            Contract.Ensures( Contract.Result<IEnumerable<object>>() != null );

            var type = enumValue.GetType();
            var name = enumValue.ToString();
            var field = type.GetRuntimeField( name );

            // look up description attribute (allow derived sub types as well)
            return field == null ? Enumerable.Empty<object>() : field.GetCustomAttributes( false );
        }

        /// <summary>
        /// Returns the display name for an enumeration value.
        /// </summary>
        /// <param name="enumValue">The <see cref="Enum">enumeration</see> to get a display name from.</param>
        /// <returns>The display name for the enumeration or the enumeration name if no display name is defined.</returns>
        /// <example>This example demonstrates to define and retrieve the display name for an enumeration.
        /// <code lang="C#"><![CDATA[
        /// using global::System;
        /// using global::System.ComponentModel;
        /// using global::System.ComponentModel.DataAnnotations;
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

            var attributes = enumValue.GetAttributes();
            var display = attributes.OfType<DisplayAttribute>().FirstOrDefault();

            if ( display != null && !string.IsNullOrEmpty( display.Name ) )
                return display.Name;

            return enumValue.ToString();
        }

        static partial void GetFromDescriptionAttribute( IEnumerable<object> attributes, ref string description );

        /// <summary>
        /// Returns the description for an enumeration value.
        /// </summary>
        /// <param name="enumValue">The <see cref="Enum">enumeration</see> to get a description from.</param>
        /// <returns>The description for the enumeration or the enumeration name if no description is defined.</returns>
        /// <remarks>The description value will first be evaluated from the <see cref="P:DisplayAttribute.Description"/> property.</remarks>
        /// <example>This example demonstrates to define and retrieve the description for an enumeration. The DescriptionAttribute can only
        /// be used in a Windows Desktop application.
        /// <code lang="C#"><![CDATA[
        /// using global::System;
        /// using global::System.ComponentModel;
        /// using global::System.ComponentModel.DataAnnotations;
        /// 
        /// public enum WeekPart
        /// {
        ///    [Description( "First Day of the Week" )]
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

            var attributes = enumValue.GetAttributes();
            var display = attributes.OfType<DisplayAttribute>().FirstOrDefault();

            if ( display != null && !string.IsNullOrEmpty( display.Description ) )
                return display.Description;

            string desc = null;
            GetFromDescriptionAttribute( attributes, ref desc );

            return string.IsNullOrEmpty( desc ) ? enumValue.ToString() : desc;
        }
    }
}
