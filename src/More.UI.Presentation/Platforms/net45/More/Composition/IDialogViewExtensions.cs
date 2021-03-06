﻿namespace More.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Provides extension methods for the <see cref="IDialogView{T}"/> interface.
    /// </summary>
    public static partial class IDialogViewExtensions
    {
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        static bool TrySetWindowAsOwner( this Window window, object owner )
        {
            Contract.Requires( window != null );

            if ( owner is Window parent )
            {
                window.Owner = parent;
                return true;
            }

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        static bool TrySetWin32WindowAsOwner( this Window window, object owner )
        {
            Contract.Requires( window != null );

            if ( owner is IWin32Window parent )
            {
                var shim = new WindowInteropHelper( window ) { Owner = parent.Handle };
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shows the view as a modeless dialog window asynchronously.
        /// </summary>
        /// <typeparam name="TViewModel">The <see cref="Type">type</see> of view model associated with the view.</typeparam>
        /// <param name="view">The <see cref="IDialogView{T}">view</see> to show.</param>
        /// <param name="owner">An <see cref="object">object</see> representing the owner of the dialog.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>If the specified <paramref name="view"/> is a <see cref="Window">window</see> and the <paramref name="owner"/> is
        /// either a <see cref="Window">window</see> or <see cref="IWin32Window">Win32 window</see>, then the <paramref name="owner"/>
        /// will be set as the owning window before the <paramref name="view"/> is shown.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static Task ShowAsync<TViewModel>( this IDialogView<TViewModel> view, object owner ) where TViewModel : class
        {
            Arg.NotNull( view, nameof( view ) );

            if ( view is Window window )
            {
                if ( !window.TrySetWindowAsOwner( owner ) )
                {
                    window.TrySetWin32WindowAsOwner( owner );
                }
            }

            view.Show();
            return CompletedTask.Value;
        }

        /// <summary>
        /// Shows the view as a modal dialog window asynchronously.
        /// </summary>
        /// <typeparam name="TViewModel">The <see cref="Type">type</see> of view model associated with the view.</typeparam>
        /// <param name="view">The <see cref="IDialogView{T}">view</see> to show.</param>
        /// <param name="owner">An <see cref="object">object</see> representing the owner of the dialog.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a <see cref="Nullable{T}"/> value of type <see cref="bool"/> that signifies how a view was closed by the user.</returns>
        /// <remarks>If the specified <paramref name="view"/> is a <see cref="Window">window</see> and the <paramref name="owner"/> is
        /// either a <see cref="Window">window</see> or <see cref="IWin32Window">Win32 window</see>, then the <paramref name="owner"/>
        /// will be set as the owning window before the <paramref name="view"/> is shown.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Nullable<T> result in Task<T>." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static Task<bool?> ShowDialogAsync<TViewModel>( this IDialogView<TViewModel> view, object owner ) where TViewModel : class
        {
            Arg.NotNull( view, nameof( view ) );

            if ( view is Window window )
            {
                if ( !window.TrySetWindowAsOwner( owner ) )
                {
                    window.TrySetWin32WindowAsOwner( owner );
                }
            }

            return view.ShowDialogAsync();
        }
    }
}