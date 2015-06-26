namespace More.Windows.Input
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

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
