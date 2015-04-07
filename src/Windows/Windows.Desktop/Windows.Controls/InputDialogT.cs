namespace More.Windows.Controls
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the base implementation for an input dialog.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of response.</typeparam>
    public abstract class InputDialog<T> : MessageDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputDialog{T}"/> class.
        /// </summary>
        protected InputDialog()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Gets or sets the input dialog response.
        /// </summary>
        /// <value>The input dialog response.</value>
        public virtual T Response
        {
            get;
            protected internal set;
        }

        /// <summary>
        /// Gets or sets the input dialog default response.
        /// </summary>
        /// <value>The input dialog default response.</value>
        public virtual T DefaultResponse
        {
            get;
            set;
        }
    }
}
