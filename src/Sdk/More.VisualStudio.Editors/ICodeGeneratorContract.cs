namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    [ContractClassFor( typeof( ICodeGenerator ) )]
    abstract class ICodeGeneratorContract : ICodeGenerator
    {
        Stream ICodeGenerator.Generate( CodeGeneratorContext context )
        {
            Contract.Requires<ArgumentNullException>( context != null, nameof( context ) );
            Contract.Ensures( Contract.Result<Stream>() != null );
            Contract.Ensures( Contract.Result<Stream>().CanRead );
            Contract.Ensures( Contract.Result<Stream>().Position == 0L );
            return null;
        }
    }
}