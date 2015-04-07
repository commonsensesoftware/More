namespace More.VisualStudio.Editors.EntityFramework
{
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a code generator for adding auxilary interface implementations to an Entity Framework database context.
    /// </summary>
    [ComVisible( true )]
    [Guid( "b54329eb-5dd0-4dc5-9e07-eac5defa33af" )]
    [ProvideObject( typeof( DbContextGenerator ) )]
    [CodeGeneratorRegistration( typeof( DbContextGenerator ), "DbContextGenerator", "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}", GeneratesDesignTimeSource = true )] 
    public sealed class DbContextGenerator : CodeGenerator
    {
        /// <summary>
        /// Generates the code content using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="">context</see> used to generate content.</param>
        /// <returns>A <see cref="Stream">stream</see> containing the generated content.</returns>
        protected override Stream Generate( CodeGeneratorContext context )
        {
            var generator = DbContextGeneratorFactory.CreateCodeGenerator( context );
            return generator.Generate( context );
        }
    }
}
