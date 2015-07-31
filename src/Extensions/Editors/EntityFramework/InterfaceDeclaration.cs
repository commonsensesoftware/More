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

            key = declaredInterface.Identifier.Text;
            typeName = declaredInterface.ToString();
            this.definingClass = definingClass;
            this.declaredInterface = declaredInterface;
        }

        internal InterfaceDeclaration( string key, InterfaceDeclaration otherInterfaceDeclaration )
        {
            Contract.Requires( !string.IsNullOrEmpty( key ) );
            Contract.Requires( otherInterfaceDeclaration != null );

            this.key = key;
            typeName = key + otherInterfaceDeclaration.DeclaredInterface.TypeArgumentList.ToString();
            definingClass = otherInterfaceDeclaration.definingClass;
            declaredInterface = otherInterfaceDeclaration.declaredInterface;
        }

        internal ClassDeclarationSyntax DefiningClass
        {
            get
            {
                Contract.Ensures( definingClass != null );
                return definingClass;
            }
        }

        internal GenericNameSyntax DeclaredInterface
        {
            get
            {
                Contract.Ensures( declaredInterface != null );
                return declaredInterface;
            }
        }

        internal string Key
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( key ) );
                return key;
            }
        }

        internal string TypeName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( typeName ) );
                return typeName;
            }
        }

        internal string ArgumentTypeName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return DeclaredInterface.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>().Single().Identifier.Text;
            }
        }
    }
}
