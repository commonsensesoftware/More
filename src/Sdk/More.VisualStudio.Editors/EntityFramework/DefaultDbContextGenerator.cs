namespace More.VisualStudio.Editors.EntityFramework
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    sealed class DefaultDbContextGenerator : ICodeGenerator
    {
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Stream Generate( CodeGeneratorContext context )
        {
            Arg.NotNull( context, nameof( context ) );

            var extension = Path.GetExtension( context.FilePath );
            var message = SR.CodeGeneratorFileTypeNotSupported.FormatDefault( extension );
            context.Progress.ReportWarning( message );
            return new MemoryStream();
        }
    }
}