namespace More.VisualStudio.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines the behavior of a code generator.
    /// </summary>
    [ContractClass( typeof( ICodeGeneratorContract ) )]
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates the code content using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="">context</see> used to generate content.</param>
        /// <returns>A <see cref="Stream">stream</see> containing the generated content.</returns>
        Stream Generate( CodeGeneratorContext context );
    }
}
