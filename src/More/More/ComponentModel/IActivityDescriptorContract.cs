﻿namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IActivityDescriptor ) )]
    abstract class IActivityDescriptorContract : IActivityDescriptor
    {
        string IActivityDescriptor.Id
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return default( string );
            }
        }

        string IActivityDescriptor.Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return default( string );
            }
        }

        string IActivityDescriptor.Description
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return default( string );
            }
        }
    }
}