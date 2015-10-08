namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Linq;
    using System.Web.Http.Services;

    /// <summary>
    /// Represents a <see cref="ISpecification{T}">specification</see> for matching <see cref="IDecorator{T}">decorated</see> <see cref="Type">types</see>.
    /// </summary>
    public class DecoratorSpecification : SpecificationBase<Type>
    {
        private static readonly Type decoratorTypeDef = typeof( IDecorator<> );

        /// <summary>
        /// Determines whether the specified item is satisfied by the specification.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the <paramref name="item"/> is satisfied by the specification; otherwise, false.</returns>
        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            // if the specified type implements IDecorator<T>, then it must be a decorator
            var matches = from @interface in item.GetInterfaces()
                          where @interface.IsGenericType &&
                                @interface.GetGenericTypeDefinition().Equals( decoratorTypeDef )
                          select @interface;

            return matches.Any();
        }
    }
}
