namespace More.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Security.Permissions;

    /// <summary>
    /// Represents a <see cref="ConfigurationValidatorAttribute"/> that can be used to support multiple
    /// <see cref="ConfigurationValidatorBase">configuration validators</see> for a single property.
    /// </summary>
    /// <remarks>
    /// The <see cref="ConfigurationValidatorAttribute"/> class does not support multiple 
    /// <see cref="ConfigurationValidatorBase">configuration validators</see> for a single configuration property.
    /// This attribute overcomes that limitation. This attribute requires a <see cref="ConfigurationValidatorBase">configuration validator</see>
    /// <see cref="Type">type</see> and the name of a public, static method on that <see cref="Type">type</see> that will be used as a
    /// factory method to create the <see cref="ConfigurationValidatorBase">validator instances</see>. 
    /// The <see cref="ConfigurationValidatorBase">validator instances</see> will be supplied to a <see cref="CompositeValidator">composite validator</see>
    /// to be composed into a single <see cref="ConfigurationValidatorBase">validator</see> which the configuration system can leverage. The supplied
    /// factory method must be assignable to <see cref="Func{T}">Func&lt;IEnumerable&lt;ConfigurationValidatorBase&gt;&gt;</see>.
    /// </remarks>
    /// <example>The following example demonstrates who to create a composite validator.
    /// <code lang="C#"><![CDATA[
    /// public class MySettings : ConfigurationElement
    /// {
    ///   [CompositeValidator( typeof( MySettings ), "CreateMySettingValidator" )]
    ///   public int MySetting
    ///   {
    ///     get
    ///     {
    ///         return (int) base["MySetting"];
    ///     }
    ///   }
    /// 
    ///   public static IEnumerable<ConfigurationValidatorBase> CreateMySettingValidator()
    ///   {
    ///     yield return new Validator1();
    ///     yield return new Validator2();
    ///   }
    /// }
    /// ]]></code>
    /// </example>
    [Serializable]
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class CompositeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private readonly Func<IEnumerable<ConfigurationValidatorBase>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidatorAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// The indicated factory method must match a  <see cref="Func{T}">Func&lt;IEnumerable&lt;ConfigurationValidatorBase&gt;&gt;</see>
        /// delegate signature, be public visibility, and be static.
        /// </remarks>
        /// <param name="factoryType">The <see cref="Type">type</see> that the factory method is declared on.</param>
        /// <param name="methodName">The name of the public, static method that can be used as a factory for creating the required validator instances.</param>
        /// <exception cref="ArgumentException">
        /// <para>The method in <paramref name="methodName"/> is not public, not static, or not declared on the <paramref name="factoryType">factory type</paramref>.</para>
        /// <para>- or -</para>
        /// <para>The method doesn't have the correct signature of <see cref="Func{IEnumerable}">Func&lt;IEnumerable&lt;ConfigurationValidatorBase&gt;&gt;</see> as required.</para>
        /// </exception>
        [SuppressMessage( "Microsoft.Security", "CA2107:ReviewDenyAndPermitOnlyUsage", Justification = @"Reviewed by REDMOND\JimmyZ.  By design" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is validated with a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This is validated with a code contract" )]
        [ReflectionPermission( SecurityAction.PermitOnly, Flags = ReflectionPermissionFlag.NoFlags )]
        public CompositeValidatorAttribute( Type factoryType, string methodName )
        {
            Arg.NotNull( factoryType, "factoryType" );
            Arg.NotNullOrEmpty( methodName, "methodName" );
            
            var flags = BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            var method = factoryType.GetMethod( methodName, flags );

            if ( method == null )
                throw new MissingMethodException( factoryType.AssemblyQualifiedName, methodName );

            if ( !method.IsStatic )
                throw new ArgumentException( SR.ExpectedStaticMethod.FormatDefault( methodName, factoryType.FullName ), "methodName" );

            this.factory = (Func<IEnumerable<ConfigurationValidatorBase>>) Delegate.CreateDelegate( typeof( Func<IEnumerable<ConfigurationValidatorBase>> ), method );
        }

        /// <summary>
        /// Gets the <see cref="Type">type</see> that provides the factory method.
        /// </summary>
        /// <remarks>The <see cref="Type">type</see> does not have to be declared on the configuration class being validated.</remarks>
        /// <value>The <see cref="Type">type</see> that provides the factory method.</value>
        public Type FactoryType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null );
                return this.factory.Method.DeclaringType;
            }
        }

        /// <summary>
        /// Gets the name of factory method.
        /// </summary>
        /// <value>The factory method name.</value>
        public string MethodName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return this.factory.Method.Name;
            }
        }

        /// <summary>
        /// Gets a new, initialized <see cref="CompositeValidator"/> instance.
        /// </summary>
        /// <value>A <see cref="CompositeValidator"/> object.</value>
        /// <remarks>The underlying <see cref="CompositeValidator"/> instance is transient. Each call to this property will
        /// create a new <see cref="CompositeValidator"/> instance.</remarks>
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                var validators = this.factory();
                var validator = new CompositeValidator( validators );
                return validator;
            }
        }
    }
}