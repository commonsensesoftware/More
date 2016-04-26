namespace More.Composition
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Reflection;
    using static System.Globalization.CultureInfo;

    /// <summary>
    /// Represents a <see cref="ConventionBuilder">convention builder</see> with additional support for <see cref="ISetting">settings</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class SettingsConventionBuilder : ConventionBuilder
    {
        private const string ConfigKeyFormat = "{0}:{1}";

        private static IEnumerable<Attribute> SwapSettingAttributeForISetting( IEnumerable<Attribute> attributes, Lazy<string> key, object literalDefaultValue )
        {
            foreach ( var attribute in attributes )
            {
                var setting = attribute as ISetting;

                if ( setting == null )
                {
                    yield return attribute;
                }
                else
                {
                    var defaultValue = setting.DefaultValue ?? ( literalDefaultValue ?? SettingAttribute.NullValue );
                    yield return new SettingAttribute( key.Value ) { DefaultValue = defaultValue };
                }
            }
        }

        /// <summary>
        /// Provide the list of attributes applied to the specified member.
        /// </summary>
        /// <param name="reflectedType">The reflectedType the type used to retrieve the memberInfo.</param>
        /// <param name="member">The member to supply attributes for.</param>
        /// <returns>The list of applied attributes.</returns>
        public override IEnumerable<Attribute> GetCustomAttributes( Type reflectedType, MemberInfo member )
        {
            if ( !( member is PropertyInfo ) )
                return base.GetCustomAttributes( reflectedType, member );

            var key = new Lazy<string>( () => string.Format( InvariantCulture, ConfigKeyFormat, reflectedType.FullName, member.Name ) );
            return SwapSettingAttributeForISetting( base.GetCustomAttributes( reflectedType, member ), key, null );
        }

        /// <summary>
        /// Provide the list of attributes applied to the specified parameter.
        /// </summary>
        /// <param name="reflectedType">The <see cref="Type">type</see> used to retrieve the parameter information from.</param>
        /// <param name="parameter">The <see cref="ParameterInfo">parameter</see> to supply attributes for.</param>
        /// <returns>The <see cref="IEnumerable{T}">sequence</see> of applied <see cref="Attribute">attributes</see>.</returns>
        public override IEnumerable<Attribute> GetCustomAttributes( Type reflectedType, ParameterInfo parameter )
        {
            var key = new Lazy<string>( () => string.Format( InvariantCulture, ConfigKeyFormat, reflectedType.FullName, parameter.Name ) );
            var literalDefaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            return SwapSettingAttributeForISetting( base.GetCustomAttributes( reflectedType, parameter ), key, literalDefaultValue );
        }
    }
}
