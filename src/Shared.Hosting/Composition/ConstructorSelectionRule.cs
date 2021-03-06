﻿namespace More.Composition
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
        static readonly Lazy<ConstructorSelectionRule> parameterless = new Lazy<ConstructorSelectionRule>( CreateParameterlessRule );
        readonly Func<IEnumerable<ConstructorInfo>, ConstructorInfo> selector;
        readonly Type[] parameterTypes;

        ConstructorSelectionRule( Func<IEnumerable<ConstructorInfo>, ConstructorInfo> selector ) => this.selector = selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorSelectionRule"/> class.
        /// </summary>
        public ConstructorSelectionRule() => selector = DefaultSelector;

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

        static ConstructorSelectionRule CreateParameterlessRule() => new ConstructorSelectionRule( ctors => ctors.Single( ctor => ctor.GetParameters().Length == 0 ) );

        static ConstructorInfo DefaultSelector( IEnumerable<ConstructorInfo> constructors )
        {
            ConstructorInfo constructor = null;
            var importingConstructor = typeof( ImportingConstructorAttribute );

            using ( var iterator = constructors.OrderByDescending( c => c.GetParameters().Length ).GetEnumerator() )
            {
                if ( !iterator.MoveNext() )
                {
                    return constructor;
                }

                constructor = iterator.Current;

                if ( constructor.CustomAttributes.Any( c => c.AttributeType == importingConstructor ) )
                {
                    return constructor;
                }

                while ( iterator.MoveNext() )
                {
                    var current = iterator.Current;

                    if ( current.CustomAttributes.Any( c => c.AttributeType == importingConstructor ) )
                    {
                        constructor = current;
                        break;
                    }
                }
            }

            return constructor;
        }

        ConstructorInfo MatchingTypesSelector( IEnumerable<ConstructorInfo> constructors )
        {
            Contract.Requires( constructors != null );
            Contract.Ensures( Contract.Result<ConstructorInfo>() != null );

            var matches = from constructor in constructors
                          let paramTypes = constructor.GetParameters().Select( p => p.ParameterType )
                          where paramTypes.SequenceEqual( parameterTypes, TypeComparer.Instance )
                          select constructor;

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

        sealed class TypeComparer : IEqualityComparer<Type>
        {
            TypeComparer() { }

            internal static IEqualityComparer<Type> Instance { get; } = new TypeComparer();

            static Type UnwrapType( Type type ) => type == null || !type.GetTypeInfo().IsGenericType ? type : type.GetGenericTypeDefinition();

            public bool Equals( Type x, Type y )
            {
                var a = UnwrapType( x );
                var b = UnwrapType( y );
                return a == null ? b == null : a.Equals( b );
            }

            public int GetHashCode( Type obj ) => UnwrapType( obj )?.GetHashCode() ?? 0;
        }
    }
}