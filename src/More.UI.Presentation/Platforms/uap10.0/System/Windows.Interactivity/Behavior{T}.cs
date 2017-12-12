namespace System.Windows.Interactivity
{
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Media;
    using Microsoft.Xaml.Interactivity;
    using More;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable object.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of the <see cref="DependencyObject" /> that can be attached.</typeparam>
    [CLSCompliant( false )]
    public abstract class Behavior<T> : DependencyObject, IBehavior where T : DependencyObject
    {
        /// <summary>
        /// Gets the object to which this behavior is attached.
        /// </summary>
        /// <value>An associated object of type <typeparamref name="T"/>.</value>
        public T AssociatedObject { get; private set; }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected virtual void OnAttached() { }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected virtual void OnDetaching() { }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The object to attach to.</param>
        /// <exception cref="System.InvalidOperationException">The Behavior is already hosted on a different element.</exception>
        /// <exception cref="System.InvalidOperationException">dependencyObject does not satisfy the Behavior type constraint.</exception>
        public void Attach( T associatedObject )
        {
            if ( associatedObject == AssociatedObject )
            {
                return;
            }

            if ( AssociatedObject != null )
            {
                throw new InvalidOperationException( ExceptionMessage.CannotHostBehaviorMultipleTimesExceptionMessage );
            }

            this.VerifyAccess();
            AssociatedObject = associatedObject;
            OnAttached();
        }

        DependencyObject IBehavior.AssociatedObject => AssociatedObject;

        void IBehavior.Attach( DependencyObject associatedObject )
        {
            if ( associatedObject != null && !typeof( T ).GetTypeInfo().IsAssignableFrom( associatedObject.GetType().GetTypeInfo() ) )
            {
                throw new InvalidOperationException( ExceptionMessage.TypeConstraintViolatedExceptionMessage.FormatDefault( new object[] { GetType().Name, associatedObject.GetType().Name, typeof( T ).Name } ) );
            }

            Attach( (T) associatedObject );
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            this.VerifyAccess();
            AssociatedObject = null;
        }
    }
}