namespace System.Windows.Interactivity
{
    using More;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Media;

    /// <summary>
    /// Provides extension method for <see cref="DependencyObject">dependency objects</see>.
    /// </summary>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public static class DependencyObjectExtensions
    {
        internal static void VerifyAccess( this CoreDispatcher dispatcher )
        {
            if ( dispatcher != null && !dispatcher.HasThreadAccess )
                throw new InvalidOperationException( ExceptionMessage.WrongThread );
        }

        internal static void VerifyAccess( this DependencyObject obj )
        {
            Contract.Requires( obj != null );
            obj.Dispatcher.VerifyAccess();
        }

        /// <summary>
        /// Return all ancestors of the specified object, including the object itself.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject">object</see> in the visual tree to find ancestors of.</param>
        /// <returns>Returns itself an all ancestors in the visual tree.</returns>
        /// <remarks>This method uses the <see cref="M:VisualTreeHelper.GetParent"/> method to do a depth first walk up 
        /// the visual tree and return all ancestors of the specified object, including the object itself.</remarks>
        public static IEnumerable<DependencyObject> GetSelfAndAncestors( this DependencyObject dependencyObject )
        {
            while ( dependencyObject != null )
            {
                yield return dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent( dependencyObject );
            }
        }
    }
}
