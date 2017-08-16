namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( ISearchRequest ) )]
    internal abstract class ISearchRequestContract : ISearchRequest
    {
        public string Language
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return null;
            }
        }

        public string QueryText
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return null;
            }
        }
    }
}