namespace More.VisualStudio.Editors.EntityFramework
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal sealed class InterfaceDeclaration
    {
        private readonly string key;
        private readonly string typeName;
        private readonly ClassDeclarationSyntax definingClass;
        private readonly GenericNameSyntax declaredInterface;

        internal InterfaceDeclaration( ClassDeclarationSyntax definingClass, GenericNameSyntax declaredInterface )
        {
            Contract.Requires( definingClass != null );
            Contract.Requires( declaredInterface != null );

            this.key = declaredInterface.Identifier.Text;
            this.typeName = declaredInterface.ToString();
            this.definingClass = definingClass;
            this.declaredInterface = declaredInterface;
        }

        internal InterfaceDeclaration( string key, InterfaceDeclaration otherInterfaceDeclaration )
        {
            Contract.Requires( !string.IsNullOrEmpty( key ) );
            Contract.Requires( otherInterfaceDeclaration != null );

            this.key = key;
            this.typeName = key + otherInterfaceDeclaration.DeclaredInterface.TypeArgumentList.ToString();
            this.definingClass = otherInterfaceDeclaration.definingClass;
            this.declaredInterface = otherInterfaceDeclaration.declaredInterface;
        }

        internal ClassDeclarationSyntax DefiningClass
        {
            get
            {
                Contract.Ensures( this.definingClass != null );
                return this.definingClass;
            }
        }

        internal GenericNameSyntax DeclaredInterface
        {
            get
            {
                Contract.Ensures( this.declaredInterface != null );
                return this.declaredInterface;
            }
        }

        internal string Key
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.key ) );
                return this.key;
            }
        }

        internal string TypeName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.typeName ) );
                return this.typeName;
            }
        }

        internal string ArgumentTypeName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return this.DeclaredInterface.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>().Single().Identifier.Text;
            }
        }
    }
}
