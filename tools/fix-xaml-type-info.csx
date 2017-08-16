#r "Microsoft.CodeAnalysis.CSharp.dll"
#r "System.Threading.Tasks.dll"

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using static System.ComponentModel.EditorBrowsableState;
using static System.Environment;
using static System.IO.File;

public sealed class XamlTypeInfoRewriter : CSharpSyntaxRewriter
{
    public XamlTypeInfoRewriter() : base( visitIntoStructuredTrivia: true ) { }

    static AttributeListSyntax NewClsCompliantAttribute()
    {
        var literal = LiteralExpression( FalseLiteralExpression );
        var args = AttributeArgumentList( SeparatedList( new[] { AttributeArgument( literal ) } ) );
        var attributes = AttributeList( SeparatedList( new[] { Attribute( ParseName( typeof( CLSCompliantAttribute ).FullName ), args ) } ) );
        return attributes.WithLeadingTrivia( Whitespace( "    " ) ).WithTrailingTrivia( EndOfLine( NewLine ) );
    }

    static AttributeListSyntax NewEditorBrowsableAttribute()
    {
        var literal = MemberAccessExpression( SimpleMemberAccessExpression, ParseTypeName( typeof( EditorBrowsableState ).FullName ), IdentifierName( nameof( Never ) ) );
        var args = AttributeArgumentList( SeparatedList( new[] { AttributeArgument( literal ) } ) );
        var attributes = AttributeList( SeparatedList( new[] { Attribute( ParseName( typeof( EditorBrowsableAttribute ).FullName ), args ) } ) );
        return attributes.WithLeadingTrivia( Whitespace( "    " ) ).WithTrailingTrivia( EndOfLine( NewLine ) );
    }

    static ClassDeclarationSyntax AddAttributeIfNecessary( ClassDeclarationSyntax @class, string attributeName, Func<AttributeListSyntax> newAttribute )
    {
        var match = from list in @class.AttributeLists
                    from attribute in list.Attributes
                    where attribute.Name.ToString() == attributeName
                    select attribute;

        return match.Any() ? @class : @class.AddAttributeLists( newAttribute() );
    }

    static ClassDeclarationSyntax AddClsCompliantIfNecessary( ClassDeclarationSyntax @class ) =>
        AddAttributeIfNecessary( @class, typeof( CLSCompliantAttribute ).FullName, NewClsCompliantAttribute );

    static ClassDeclarationSyntax AddEditorBrowsableIfNecessary( ClassDeclarationSyntax @class ) =>
        AddAttributeIfNecessary( @class, typeof( EditorBrowsableAttribute ).FullName, NewEditorBrowsableAttribute );

    public bool HasDisableXmlCommentsWarningDirective { get; private set; }

    public bool HasRestoreXmlCommentsWarningDirective { get; private set; }

    public override SyntaxNode VisitClassDeclaration( ClassDeclarationSyntax node )
    {
        var @class = (ClassDeclarationSyntax) base.VisitClassDeclaration( node );

        if ( !@class.Modifiers.Any( m => m.Kind() == PublicKeyword ) )
        {
            return @class;
        }

        return AddEditorBrowsableIfNecessary( AddClsCompliantIfNecessary( @class ) );
    }

    public override SyntaxNode VisitPragmaWarningDirectiveTrivia( PragmaWarningDirectiveTriviaSyntax node )
    {
        var pragma = node.DescendantNodes()
                         .OfType<LiteralExpressionSyntax>()
                         .FirstOrDefault( l => l.Kind() == NumericLiteralExpression && l.Token.Text == "1591" );

        if ( pragma != null )
        {
            switch ( node.DisableOrRestoreKeyword.Text )
            {
                case "disable":
                    HasDisableXmlCommentsWarningDirective = true;
                    break;
                case "restore":
                    HasRestoreXmlCommentsWarningDirective = true;
                    break;
            }
        }

        return base.VisitPragmaWarningDirectiveTrivia( node );
    }
}

static PragmaWarningDirectiveTriviaSyntax XmlCommentsPragma( bool suppress )
{
    ExpressionSyntax literal = LiteralExpression( NumericLiteralExpression, Literal( 1591 ) );
    var space = Whitespace( " " );
    var token = suppress ? Token( DisableKeyword ) : Token( RestoreKeyword );
    var pragma = PragmaWarningDirectiveTrivia( token, SeparatedList( new[] { literal } ), suppress );

    return pragma.WithPragmaKeyword( pragma.PragmaKeyword.WithTrailingTrivia( space ) )
                 .WithWarningKeyword( pragma.WarningKeyword.WithTrailingTrivia( space ) )
                 .WithDisableOrRestoreKeyword( pragma.DisableOrRestoreKeyword.WithTrailingTrivia( space ) )
                 .WithTrailingTrivia( EndOfLine( NewLine ), EndOfLine( NewLine ) );
}

static SyntaxTrivia DisableXmlCommentsWarning() => Trivia( XmlCommentsPragma( true ) );

static SyntaxTrivia RestoreXmlCommentsWarning() => Trivia( XmlCommentsPragma( false ) );

var filePath = Args[0];
var source = ParseText( ReadAllText( filePath ) );
var rewriter = new XamlTypeInfoRewriter();
var code = rewriter.Visit( source.GetRoot() );

if ( !rewriter.HasDisableXmlCommentsWarningDirective )
{
    code = code.WithLeadingTrivia( code.GetLeadingTrivia().Union( new[] { DisableXmlCommentsWarning() } ) );
}

if ( !rewriter.HasRestoreXmlCommentsWarningDirective )
{
    code = code.WithTrailingTrivia( RestoreXmlCommentsWarning() );
}

using ( var stream = new FileStream( filePath, FileMode.Create ) )
using ( var writer = new StreamWriter( stream ) )
{
    code.WriteTo( writer );
    writer.Flush();
}