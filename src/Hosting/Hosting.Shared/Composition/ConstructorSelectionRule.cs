namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="IRule{T,T}">rule</see> used to select the <see cref="ConstructorInfo">constructor</see> of composed objects.
    /// </summary>
    public class ConstructorSelectionRule : IRule<IEnumerable<ConstructorInfo>, ConstructorInfo>
    {
        private sealed class TypeComparer : IEqualityComparer<Type>
        {
            private TypeComparer()
            {
            }

            internal static IEqualityComparer<Type> Instance
            {
                get;
            } = new TypeComparer();

            private static Type UnwrapType( Type type ) => type == null || !type.GetTypeInfo().IsGenericType ? type : type.GetGenericTypeDefinition();

            public bool Equals( Type x, Type y )
            {
                var a = UnwrapType( x );
                var b = UnwrapType( y );

                if ( a == null )
                    return b == null;

                return a.Equals( b );
            }

            public int GetHashCode( Type obj ) => UnwrapType( obj )?.GetHashCode() ?? 0;
        }

        private static readonly Lazy<ConstructorSelectionRule> parameterless = new Lazy<ConstructorSelectionRule>( CreateParameterlessRule );
        private readonly Func<IEnumerable<ConstructorInfo>, ConstructorInfo> selector;
        private readonly Type[] parameterTypes;

        private ConstructorSelectionRule( Func<IEnumerable<ConstructorInfo>, ConstructorInfo> selector )
        {
            Contract.Requires( selector != null );
            this.selector = selector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorSelectionRule"/> class.
        /// </summary>
        public ConstructorSelectionRule()
        {
            selector = DefaultSelector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorSelectionRule"/> class.
        /// </summary>
        /// <param name="parameterTypes">A <see cref="IEnumerable{T}">sequence</see> of parameter <see cref="Type">types</see> used to match a constructor.</param>
        public ConstructorSelectionRule( IEnumerable<Type> parameterTypes )
        {
            Arg.NotNull( parameterTypes, nameof( parameterTypes ) );

            this.parameterTypes = parameterTypes.ToArray();
            selector = MatchingTypesSelector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorSelectionRule"/> class.
        /// </summary>
        /// <param name="parameterTypes">An <see cref="Array">array</see> of parameter <see cref="Type">types</see> used to match a constructor.</param>
        public ConstructorSelectionRule( params Type[] parameterTypes )
        {
            Arg.NotNull( parameterTypes, nameof( parameterTypes ) );

            this.parameterTypes = parameterTypes;
            selector = MatchingTypesSelector;
        }

        /// <summary>
        /// Gets a selection rule which always matches a parameterless constructor.
        /// </summary>
        /// <value>A specialized <see cref="ConstructorSelectionRule">constructor selection rule</see> for parameterless <see cref="ConstructorInfo">constructors</see>.</value>
        public static ConstructorSelectionRule Parameterless
        {
            get
            {
                Contract.Ensures( Contract.Result<ConstructorSelectionRule>() != null );
                return parameterless.Value;
            }
        }

        private static ConstructorSelectionRule CreateParameterlessRule() =>
            new ConstructorSelectionRule( ctors => ctors.Single( ctor => ctor.GetParameters().Length == 0 ) );

        private static ConstructorInfo DefaultSelector( IEnumerable<ConstructorInfo> constructors )
        {
            // duplicate the default behavior MEF uses
            ConstructorInfo constructor = null;
            var attributeType = typeof( ImportingConstructorAttribute );

            // find the constructor with the greatest number of parameters
            using ( var iterator = constructors.OrderByDescending( c => c.GetParameters().Length ).GetEnumerator() )
            {
                // if there's no constructors, we're done
                if ( !iterator.MoveNext() )
                    return constructor;

                constructor = iterator.Current;

                // honor ImportingConstructorAttribute. if this constructor has it applied, we're done
                if ( constructor.CustomAttributes.Any( c => c.AttributeType == attributeType ) )
                    return constructor;

                // enumerate remaining constructors and make sure we honor ImportingConstructorAttribute
                // if it was explicitly applied to a constructor
                while ( iterator.MoveNext() )
                {
                    var current = iterator.Current;

                    if ( current.CustomAttributes.Any( c => c.AttributeType == attributeType ) )
                    {
                        constructor = current;
                        break;
                    }
                }
            }

            return constructor;
        }

        private ConstructorInfo MatchingTypesSelector( IEnumerable<ConstructorInfo> constructors )
        {
            Contract.Requires( constructors != null );
            Contract.Ensures( Contract.Result<ConstructorInfo>() != null );

            var matches = from constructor in constructors
                          let paramTypes = constructor.GetParameters().Select( p => p.ParameterType )
                          where paramTypes.SequenceEqual( parameterTypes, TypeComparer.Instance )
                          select constructor;

            // must exactly match one constructor with the same sequence of parameter types
            return matches.SingleOrDefault();
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The item representing the <see cref="IEnumerable{T}">sequence</see> of
        /// <see cref="ConstructorInfo">constructors</see> to evaluate.</param>
        /// <returns>The matching <see cref="ConstructorInfo">constructor</see>.</returns>
        public ConstructorInfo Evaluate( IEnumerable<ConstructorInfo> item )
        {
            Arg.NotNull( item, nameof( item ) );
            return selector( item );
        }
    }
}
