﻿namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( INamedComponent ) )]
    abstract class INamedComponentContract : INamedComponent
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
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), nameof( value ) );
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
                Contract.Requires<ArgumentNullException>( value != null, nameof( value ) );
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }
    }
}