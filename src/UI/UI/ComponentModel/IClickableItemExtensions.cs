namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="IClickableItem{T}"/> interface.
    /// </summary>
    public static class IClickableItemExtensions
    {
        /// <summary>
        /// Performs the click operation for the item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item to click.</typeparam>
        /// <param name="item">The <see cref="IClickableItem{T}">item</see> to click.</param>
        /// <remarks>This method invokes the <see cref="P:IClickableItem{T}.Click"/> command using <see cref="P:IClickableItem{T}.Value"/> as the parameter,
        /// if the command can be executed.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void PerformClick<T>( this IClickableItem<T> item )
        {
            Arg.NotNull( item, nameof( item ) );

            if ( item.Click.CanExecute( null ) )
                item.Click.Execute( null );
        }

        /// <summary>
        /// Performs the click operation for the item.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item to click.</typeparam>
        /// <param name="item">The <see cref="IClickableItem{T}">item</see> to click.</param>
        /// <param name="parameter">The parameter associated with the click operation.</param>
        /// <remarks>This method invokes the <see cref="P:IClickableItem{T}.Click"/> command with the <paramref name="parameter"/>,
        /// if the command can be executed.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void PerformClick<T>( this IClickableItem<T> item, object parameter )
        {
            Arg.NotNull( item, nameof( item ) );

            if ( item.Click.CanExecute( parameter ) )
                item.Click.Execute( parameter );
        }
    }
}
