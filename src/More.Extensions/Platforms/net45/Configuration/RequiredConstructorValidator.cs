namespace More.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// A <see cref="ConfigurationValidatorBase"/> that confirms the supplied type contains a public constructor
    /// that matches the indicated signature. It optionally allows signatures that subclass the supplied parameter
    /// <see cref="Type">types</see>.
    /// </summary>
    /// <remarks>This validator only supports validation of <see cref="Type">type</see> instances.</remarks>
    public class RequiredConstructorValidator : ConfigurationValidatorBase
    {
        readonly IReadOnlyList<Type> parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredConstructorValidator"/> class.
        /// </summary>
        /// <remarks>This constructor does not allow subclassing of parameters types.</remarks>
        /// <param name="parameters">The <see cref="IEnumerable{T}">sequence</see> of parameters, in positional order, that are required.</param>
        public RequiredConstructorValidator( IEnumerable<Type> parameters ) : this( false, parameters ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredConstructorValidator"/> class.
        /// </summary>
        /// <param name="allowContravariance">Indicates if exact match of the parameter types or subclassing of types is allowed.</param>
        /// <param name="parameters">The <see cref="IEnumerable{T}">sequence</see> of parameters, in positional order, that are required.</param>
        public RequiredConstructorValidator( bool allowContravariance, IEnumerable<Type> parameters )
        {
            Arg.NotNull( parameters, nameof( parameters ) );

            AllowContravariance = allowContravariance;
            this.parameters = parameters.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Type.IsAssignableFrom">subclassed</see> types are allowed
        /// or if the constructor must exactly match the exact type.
        /// </summary>
        /// <value>True if constructors may subclass parameter types ; otherwise false.</value>
        public bool AllowContravariance { get; }

        /// <summary>
        /// Gets the list of parameter <see cref="Type">types</see> the required constructor must use.
        /// </summary>
        /// <value>The <see cref="IReadOnlyList{T}">read-only list</see> of parameter <see cref="Type">types</see> the required constructor must use.</value>
        public IReadOnlyList<Type> Parameters
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<Type>>() != null );
                return parameters;
            }
        }

        /// <summary>
        /// Determines whether the value of an object is valid.
        /// </summary>
        /// <param name="value">The object value.</param>
        public override void Validate( object value )
        {
            if ( !( value is Type typeToValidate ) )
            {
                throw new ArgumentException( SR.InvalidArgType.FormatDefault( typeof( Type ) ), nameof( value ) );
            }

            if ( AllowContravariance )
            {
                var constructors = from ctor in typeToValidate.GetConstructors()
                                   let args = ctor.GetParameters()
                                   where args.Length == Parameters.Count
                                   let argTypes = args.Select( a => a.ParameterType )
                                   where IsMatch( argTypes )
                                   select ctor;

                if ( constructors.Any() )
                {
                    return;
                }
            }
            else
            {
                var constructor = typeToValidate.GetConstructor( Parameters.ToArray() );

                if ( constructor != null )
                {
                    return;
                }
            }

            var parameterNames = string.Join( ",", Parameters.Select( item => item.FullName ) );
            var message = SR.RequiredConstructorMissing.FormatInvariant( typeToValidate.FullName, parameterNames );
            throw new ConfigurationErrorsException( message );
        }

        bool IsMatch( IEnumerable<Type> availableParameters )
        {
            Contract.Requires( availableParameters != null );
            var unmatched = availableParameters.Where( ( t, i ) => !t.IsAssignableFrom( Parameters[i] ) ).Any();
            return !unmatched;
        }

        /// <summary>
        /// Determines whether an object can be validated based on type.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>True if the type parameter value matches the expected type; otherwise, false.</returns>
        public override bool CanValidate( Type type ) => type == typeof( Type );
    }
}