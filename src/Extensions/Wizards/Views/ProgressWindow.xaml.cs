namespace More.VisualStudio.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents a basic window for displaying progress.
    /// </summary>
    public partial class ProgressWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressWindow"/> class.
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressWindow"/> class.
        /// </summary>
        /// <param name="status">The status text to display during progress.</param>
        public ProgressWindow( string status )
        {
            InitializeComponent();
            Status.Text = status;
        }
    }
}
