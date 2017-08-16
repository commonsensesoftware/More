namespace More.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a view for a dialog window.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of view model to attach to the view.</typeparam>
    [ContractClass( typeof( IDialogViewContract<> ) )]
    public interface IDialogView<T> : IView<T, T> where T : class
    {
        /// <summary>
        /// Gets a value indicating whether the dialog view is active.
        /// </summary>
        /// <value>True if the dialog is the active view; otherwise, false.</value>
        bool IsActive { get; }

        /// <summary>
        /// Gets the result associated with the dialog view.
        /// </summary>
        /// <value>True if the user input was accepted, false if the user input was canceled, or
        /// <see langword="null"/> if no response was provided.</value>
        bool? DialogResult { get; }

        /// <summary>
        /// Activates the dialog view.
        /// </summary>
        /// <returns>True if the dialog view is activated; otherwise, false.</returns>
        bool Activate();

        /// <summary>
        /// Closes the dialog view.
        /// </summary>
        void Close();

        /// <summary>
        /// Closes the dialog view.
        /// </summary>
        /// <param name="dialogResult">The <see cref="Nullable{T}">dialog result</see> associated with the dialog.</param>
        void Close( bool? dialogResult );

        /// <summary>
        /// Shows the view as a modeless dialog.
        /// </summary>
        void Show();

        /// <summary>
        /// Shows the view as a modal dialog.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="Nullable{T}">dialog result</see> that signifies
        /// how a view was closed by the user.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        Task<bool?> ShowDialogAsync();
    }
}