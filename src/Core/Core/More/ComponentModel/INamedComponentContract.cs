namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( INamedComponent ) )]
    internal abstract class INamedComponentContract : INamedComponent
    {
        string INamedComponent.Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return default( string );
            }
            set
            {
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), "value" );
            }
        }

        string INamedComponent.Description
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return default( string );
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
            }
            remove
            {
            }
        }
    }
}
