namespace More.VisualStudio.Editors.EntityFramework
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal static class DbContextGeneratorFactory
    {
        internal static ICodeGenerator CreateCodeGenerator( CodeGeneratorContext context )
        {
            Contract.Requires( context != null );
            Contract.Ensures( Contract.Result<ICodeGenerator>() != null );

            var extension = Path.GetExtension( context.FilePath ).ToUpperInvariant();

            switch ( extension )
            {
                case ".CS":
                    return new CSharpDbContextCodeGenerator();
                default:
                    return new DefaultDbContextGenerator();
            }
        }
    }
}
