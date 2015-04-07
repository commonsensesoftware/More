namespace More.ComponentModel
{
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Defines the behavior of a named component.
    /// </summary>
    [ContractClass( typeof( INamedComponentContract ) )] 
    public interface INamedComponent : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>The component name.</value>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the component description.
        /// </summary>
        /// <value>The component description.</value>
        string Description
        {
            get;
            set;
        }
    }
}
