namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Provides the code contract definition for the <see cref="ICodeGenerator"/> class.
    /// </summary>
    [ContractClassFor( typeof( ICodeGenerator ) )]
    internal abstract class ICodeGeneratorContract : ICodeGenerator
    {
        Stream ICodeGenerator.Generate( CodeGeneratorContext context )
        {
            Contract.Requires<ArgumentNullException>( context != null, "context" );
            Contract.Ensures( Contract.Result<Stream>() != null );
            Contract.Ensures( Contract.Result<Stream>().CanRead );
            Contract.Ensures( Contract.Result<Stream>().Position == 0L );
            return null;
        }
    }
}
