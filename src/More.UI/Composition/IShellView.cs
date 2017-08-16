namespace More.Composition
{
    /// <summary>
    /// Defines the behavior of a shell view.
    /// </summary>
    public interface IShellView
    {
        /// <summary>
        /// Gets or sets localization/globalization language information that applies to the shell view.
        /// </summary>
        /// <value>The language information for the shell view.  This property can be null.</value>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the direction that text and other user interface (UI) elements flow within the shell view.
        /// </summary>
        /// <value>The direction that text and other UI elements flow within the shell view. This property can be null.</value>
        string FlowDirection { get; set; }

        /// <summary>
        /// Shows the view representing the shell.
        /// </summary>
        void Show();
    }
}