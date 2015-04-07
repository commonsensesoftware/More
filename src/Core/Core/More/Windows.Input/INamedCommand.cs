namespace More.Windows.Input
{
    using More.ComponentModel;
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a named command.
    /// </summary>
    [ContractClass( typeof( INamedCommandContract ) )]
    public interface INamedCommand : INotifyCommandChanged, INamedComponent, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the identifier associated with the command.
        /// </summary>
        /// <value>The command identifier.</value>
        string Id
        {
            get;
            set;
        }
    }
}
