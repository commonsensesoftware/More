namespace More.Composition
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Convention;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using static System.Globalization.CultureInfo;

    /// <summary>
    /// Represents a <see cref="ConventionBuilder">convention builder</see> with additional support for <see cref="ISetting">settings</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class SettingsConventionBuilder : ConventionBuilder
    {
        const string ConfigKeyFormat = "{0}:{1}";

        /// <summary>
        /// Provide the list of attributes applied to the specified member.
        /// </summary>
        /// <param name="reflectedType">The reflectedType the type used to retrieve the memberInfo.</param>
        /// <param name="member">The member to supply attributes for.</param>
        /// <returns>The list of applied attributes.</returns>
        public override IEnumerable<Attribute> GetCustomAttributes( Type reflectedType, MemberInfo member )
        {
            var attributes = base.GetCustomAttributes( reflectedType, member );

            if ( !( member is PropertyInfo ) || reflectedType == null )
            {
                return attributes;
            }

            var key = new Lazy<string>( () => string.Format( InvariantCulture, ConfigKeyFormat, ( reflectedType ?? member.DeclaringType ).FullName, member.Name ) );
            return ReplaceImportAttributeWithSettingAttribute( attributes, key, null );
        }

        /// <summary>
        /// Provide the list of attributes applied to the specified parameter.
        /// </summary>
        /// <param name="reflectedType">The <see cref="Type">type</see> used to retrieve the parameter information from.</param>
        /// <param name="parameter">The <see cref="ParameterInfo">parameter</see> to supply attributes for.</param>
        /// <returns>The <see cref="IEnumerable{T}">sequence</see> of applied <see cref="Attribute">attributes</see>.</returns>
        public override IEnumerable<Attribute> GetCustomAttributes( Type reflectedType, ParameterInfo parameter )
        {
            var key = new Lazy<string>( () => string.Format( InvariantCulture, ConfigKeyFormat, ( reflectedType ?? parameter.Member.DeclaringType ).FullName, parameter.Name ) );
            var literalDefaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            return ReplaceImportAttributeWithSettingAttribute( base.GetCustomAttributes( reflectedType, parameter ), key, literalDefaultValue );
        }

        static IEnumerable<Attribute> ReplaceImportAttributeWithSettingAttribute( IEnumerable<Attribute> attributes, Lazy<string> defaultKey, object literalDefaultValue )
        {
            Contract.Requires( attributes != null );
            Contract.Requires( defaultKey != null );
            Contract.Ensures( Contract.Result<IEnumerable<Attribute>>() != null );

            var hasImport = false;
            var import = default( Attribute );

            foreach ( var attribute in attributes )
            {
                var setting = attribute as ISetting;

                if ( setting == null )
                {
                    if ( attribute is ImportAttribute )
                    {
                        import = attribute;
                    }
                    else
                    {
                        yield return attribute;
                    }
                }
                else
                {
                    hasImport = true;

                    var defaultValue = setting.DefaultValue ?? ( literalDefaultValue ?? SettingAttribute.NullValue );
                    var key = setting.Key;

                    if ( string.IsNullOrEmpty( key ) )
                    {
                        key = defaultKey.Value;
                    }

                    yield return new SettingAttribute( key ) { DefaultValue = defaultValue };
                }
            }

            if ( !hasImport && import != null )
            {
                yield return import;
            }
        }
    }
}