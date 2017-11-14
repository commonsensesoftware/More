namespace More.VisualStudio.Templates
{
    using System;

    /// <summary>
    /// Represents the possible project template cancel behaviors.
    /// </summary>
    public enum ProjectTemplateCancelBehavior
    {
        /// <summary>
        /// Indicates the project template wizard should cancel the new project dialog.
        /// </summary>
        /// <remarks>This is the default behavior.</remarks>
        Cancel,

        /// <summary>
        /// Indicates the project template wizard should return to the new project dialog.
        /// </summary>
        BackOut
    }
}