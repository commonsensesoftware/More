using System.Diagnostics.Contracts;
namespace More.VisualStudio.Editors.EntityFramework
{
    using ComponentModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    internal class CSharpDbContextCodeGenerator : ICodeGenerator
    {
        private const string Comment = "//";
        private const string DbContext = "DbContext";
        private const string IReadOnlyRepository = "IReadOnlyRepository";
        private const string IRepository = "IRepository";
        private const string IUnitOfWork = "IUnitOfWork";
        private const string INotifyPropertyChanged = "INotifyPropertyChanged";

        private static readonly Lazy<IReadOnlyList<UsingDirectiveSyntax>> requiredUsings = new Lazy<IReadOnlyList<UsingDirectiveSyntax>>( CreateRequiredUsings );
        private static readonly Lazy<ISpecification<GenericNameSyntax>> interfaceSpecification = new Lazy<ISpecification<GenericNameSyntax>>( CreateInterfaceSpecification );

        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CodeDom", Justification = "False positive. Literal code text." )]
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ComponentModel", Justification = "False positive. Literal code text." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Whitespace(System.String)", Justification = "Literal code elements cannot be localized." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(System.String,System.Int32,System.Boolean)", Justification = "Literal code elements cannot be localized." )]
        private static IReadOnlyList<UsingDirectiveSyntax> CreateRequiredUsings()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<UsingDirectiveSyntax>>() != null );

            var space = Whitespace( " " );

            return new[]
            {
                UsingDirective( ParseName( "More.ComponentModel" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.CodeDom.Compiler" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Collections.Generic" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.ComponentModel" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Data.Entity" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Linq" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Threading" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Threading.Tasks" ).WithLeadingTrivia( space ) )
            };
        }

        private static ISpecification<GenericNameSyntax> CreateInterfaceSpecification()
        {
            Contract.Ensures( Contract.Result<ISpecification<GenericNameSyntax>>() != null );

            var specification = new Specification<GenericNameSyntax>( i => i.Identifier.Text == IReadOnlyRepository )
                                                                 .Or( i => i.Identifier.Text == IRepository )
                                                                 .Or( i => i.Identifier.Text == IUnitOfWork );

            return specification;
        }

        private static IReadOnlyList<UsingDirectiveSyntax> RequiredUsings
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<UsingDirectiveSyntax>>() != null );
                return requiredUsings.Value;
            }
        }

        private static ISpecification<GenericNameSyntax> InterfaceSpecification
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<GenericNameSyntax>>() != null );
                return interfaceSpecification.Value;
            }
        }

        private static IReadOnlyList<ClassDeclarationSyntax> FindClasses( SyntaxNode node )
        {
            Contract.Requires( node != null );
            Contract.Ensures( Contract.Result<IEnumerable<ClassDeclarationSyntax>>() != null );

            var classes = from @class in node.DescendantNodes().OfType<ClassDeclarationSyntax>()
                          from name in @class.BaseList.DescendantNodes().OfType<IdentifierNameSyntax>()
                          where name.Identifier.Text == DbContext
                          select @class;

            return classes.ToArray();
        }

        private static IReadOnlyList<UsingDirectiveSyntax> FindUsings( SyntaxNode node )
        {
            Contract.Requires( node != null );
            Contract.Ensures( Contract.Result<IEnumerable<UsingDirectiveSyntax>>() != null );
            return node.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
        }

        private static IReadOnlyList<InterfaceDeclaration> FindInterfaceDeclarations( IEnumerable<ClassDeclarationSyntax> classes )
        {
            Contract.Requires( classes != null );
            Contract.Ensures( Contract.Result<IEnumerable<InterfaceDeclaration>>() != null );

            var interfaces = from @class in classes
                             from @interface in @class.BaseList.DescendantNodes().OfType<GenericNameSyntax>()
                             where InterfaceSpecification.IsSatisfiedBy( @interface )
                             select new InterfaceDeclaration( @class, @interface );

            return interfaces.ToArray();
        }

        private static void ImplementInterfaces(
            CodeGeneratorContext context,
            IReadOnlyList<UsingDirectiveSyntax> usings,
            IReadOnlyList<InterfaceDeclaration> declarations,
            TextWriter writer )
        {
            Contract.Requires( context != null );
            Contract.Requires( declarations != null );
            Contract.Requires( writer != null );

            var iterator = declarations.GetEnumerator();

            if ( !iterator.MoveNext() )
                return;

            var indentingWriter = new IndentingTextWriter( writer );
            var implementedInterfaces = new HashSet<string>();
            var hasNamespace = WriteStartClass( indentingWriter, context.DefaultNamespace, usings, iterator.Current.DefiningClass );

            ImplementInterface( indentingWriter, iterator.Current, implementedInterfaces );

            while ( iterator.MoveNext() )
            {
                writer.WriteLine();
                ImplementInterface( indentingWriter, iterator.Current, implementedInterfaces );
            }

            WriteEndClass( indentingWriter, hasNamespace );
        }

        private static string ResolveNamespace( ClassDeclarationSyntax @class, string defaultNamespace )
        {
            Contract.Requires( @class != null );
            Contract.Requires( defaultNamespace != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var @namespace = @class.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            if ( @namespace == null )
                return defaultNamespace;

            // if the namespace has multiple components (e.g. dotted), then it's a "qualified name"
            var qualifiedName = @namespace.Name as QualifiedNameSyntax;

            if ( qualifiedName != null )
                return qualifiedName.ToString(); // flatten all identifers

            // if namespace has only one component, then it's an identifier
            var name = (IdentifierNameSyntax) @namespace.Name;
            return name.Identifier.Text;
        }

        private static string ResolveScopeModifier( ClassDeclarationSyntax @class )
        {
            Contract.Requires( @class != null );
            Contract.Ensures( Contract.Result<string>() != null );

            foreach ( var modifier in @class.Modifiers )
            {
                var kind = modifier.Kind();

                switch ( kind )
                {
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                        return modifier.Text + " ";
                }
            }

            return string.Empty;
        }

        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CodeDom", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ComponentModel", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Whitespace(System.String)", Justification = "A space does not require localization." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(System.String,System.Int32,System.Boolean)", Justification = "These literals are namespaces and cannot be localized." )]
        private static void WriteUsings( IndentingTextWriter writer, IReadOnlyList<UsingDirectiveSyntax> declaredUsings )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaredUsings != null );

            var usings = new SortedSet<UsingDirectiveSyntax>( UsingDirectiveComparer.Instance );

            // merge sorted, distinct list of required and declared usings
            usings.AddRange( RequiredUsings );
            usings.AddRange( declaredUsings );

            // write out required usings
            foreach ( var @using in usings )
                writer.WriteLine( @using );
        }

        private static bool WriteStartClass(
            IndentingTextWriter writer,
            string defaultNamespace,
            IReadOnlyList<UsingDirectiveSyntax> usings,
            ClassDeclarationSyntax @class )
        {
            Contract.Requires( writer != null );
            Contract.Requires( defaultNamespace != null );
            Contract.Requires( usings != null );
            Contract.Requires( @class != null );

            var @namespace = ResolveNamespace( @class, defaultNamespace );
            var hasNamespace = !string.IsNullOrEmpty( @namespace );
            var className = @class.Identifier.Text;

            if ( hasNamespace )
            {
                writer.WriteLine( "namespace {0}", @namespace );
                writer.WriteLine( "{" );
                writer.Indent();
            }

            WriteUsings( writer, usings );
            writer.WriteLine();
            writer.WriteLine( "/// <content>" );
            writer.WriteLine( "/// Provides auto-generated interfaces for the <see cref=\"{0}\" /> class. To add addition interfaces,", className );
            writer.WriteLine( "/// implement the interface in the main source file." );
            writer.WriteLine( "/// <seealso cref=\"IReadOnlyRepository{T}\" />" );
            writer.WriteLine( "/// <seealso cref=\"IRepository{T}\" />" );
            writer.WriteLine( "/// <seealso cref=\"IUnitOfWork{T}\" />." );
            writer.WriteLine( "/// <content>" );
            writer.WriteLine( "[GeneratedCode( \"More Framework\", \"1.0\" )]" );
            writer.WriteLine( "{0}partial class {1}", ResolveScopeModifier( @class ), className );
            writer.WriteLine( "{" );
            writer.Indent();

            return hasNamespace;
        }

        private static void WriteEndClass( IndentingTextWriter writer, bool hasNamespace )
        {
            Contract.Requires( writer != null );

            if ( hasNamespace )
            {
                writer.Unindent();
                writer.WriteLine( "}" );
            }

            writer.Unindent();
            writer.Write( "}" );
        }

        private static void ImplementInterface( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            switch ( declaration.Key )
            {
                case IReadOnlyRepository:
                    WriteReadOnlyRepository( writer, declaration, implementedInterfaces );
                    break;
                case IRepository:
                    var inheritedDeclaration = new InterfaceDeclaration( IReadOnlyRepository, declaration );

                    if ( WritePropertyChangedImplementation( writer, implementedInterfaces ) )
                        writer.WriteLine();

                    if ( WriteReadOnlyRepository( writer, inheritedDeclaration, implementedInterfaces ) )
                        writer.WriteLine();

                    WriteRepository( writer, declaration, implementedInterfaces );
                    break;
                case IUnitOfWork:
                    if ( WritePropertyChangedImplementation( writer, implementedInterfaces ) )
                        writer.WriteLine();

                    WriteUnitOfWork( writer, declaration, implementedInterfaces );
                    break;
            }
        }

        private static bool WritePropertyChangedImplementation( IndentingTextWriter writer, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( INotifyPropertyChanged ) )
                return false;

            implementedInterfaces.Add( INotifyPropertyChanged );

            writer.WriteLine( "public event PropertyChangedEventHandler PropertyChanged;" );
            writer.WriteLine();
            writer.WriteLine( "private void OnPropertyChanged( PropertyChangedEventArgs e ) => PropertyChanged?.Invoke( this, e );" );

            return true;
        }

        private static bool WriteReadOnlyRepository( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
                return false;

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( "async Task<IEnumerable<{1}>> {0}.GetAsync( Func<IQueryable<{1}>, IQueryable<{1}>> queryShaper, CancellationToken cancellationToken )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return await queryShaper( Set<{0}>() ).ToArrayAsync( cancellationToken ).ConfigureAwait( false );", declaration.ArgumentTypeName );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task<TResult> {0}.GetAsync<TResult>( Func<IQueryable<{1}>, TResult> queryShaper, CancellationToken cancellationToken )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return await Task<TResult>.Factory.StartNew( () => queryShaper( Set<{0}>() ), cancellationToken ).ConfigureAwait( false );", declaration.ArgumentTypeName );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static bool WriteRepository( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
                return false;

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( "bool {0}.HasPendingChanges", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "get" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return ChangeTracker.HasChanges();" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Add( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Set<{0}>().Add( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Remove( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Set<{0}>().Remove( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Update( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "if ( Entry( item ).State != EntityState.Detached )" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine();
            writer.WriteLine( "Set<{0}>().Attach( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.DiscardChanges()", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "foreach ( var entry in ChangeTracker.Entries<{0}>() )", declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "switch ( entry.State )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "case EntityState.Modified:" );
            writer.WriteLine( "case EntityState.Deleted:" );
            writer.Indent();
            writer.WriteLine( "entry.CurrentValues.SetValues( entry.OriginalValues );" );
            writer.WriteLine( "entry.State = EntityState.Unchanged;" );
            writer.WriteLine( "break;" );
            writer.Unindent();
            writer.WriteLine( "case EntityState.Added:" );
            writer.Indent();
            writer.WriteLine( "entry.State = EntityState.Detached;" );
            writer.WriteLine( "break;" );
            writer.Unindent();
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task IRepository<{0}>.SaveChangesAsync( CancellationToken cancellationToken )", declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static bool WriteUnitOfWork( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
                return false;

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( "bool {0}.HasPendingChanges", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "get" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return ChangeTracker.HasChanges();" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterNew( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Set<{0}>().Add( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterRemoved( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Set<{0}>().Remove( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterChanged( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "if ( Entry( item ).State != EntityState.Detached )" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine();
            writer.WriteLine( "Set<{0}>().Attach( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Unregister( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Entry( item ).State = EntityState.Detached;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Rollback()", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "foreach ( var entry in ChangeTracker.Entries<{0}>() )", declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "switch ( entry.State )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "case EntityState.Modified:" );
            writer.WriteLine( "case EntityState.Deleted:" );
            writer.Indent();
            writer.WriteLine( "entry.CurrentValues.SetValues( entry.OriginalValues );" );
            writer.WriteLine( "entry.State = EntityState.Unchanged;" );
            writer.WriteLine( "break;" );
            writer.Unindent();
            writer.WriteLine( "case EntityState.Added:" );
            writer.Indent();
            writer.WriteLine( "entry.State = EntityState.Detached;" );
            writer.WriteLine( "break;" );
            writer.Unindent();
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task {0}.CommitAsync( CancellationToken cancellationToken )", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static void EvaluateAndGenerateCode( CodeGeneratorContext context, StreamWriter writer )
        {
            Contract.Requires( context != null );
            Contract.Requires( writer != null );

            var ast = ParseText( context.FileContents );
            var root = ast.GetRoot();
            var classes = FindClasses( root );

            if ( classes.Count == 0 )
            {
                writer.WriteLine( SR.DbContextClassNotFound.FormatDefault( Comment ) );
                return;
            }

            var usings = FindUsings( root );
            var declarations = FindInterfaceDeclarations( classes );

            if ( declarations.Count == 0 )
                writer.WriteLine( SR.NoInterfacesFound.FormatDefault( typeof( IReadOnlyRepository<> ), typeof( IRepository<> ), typeof( IUnitOfWork<> ) ) );
            else
                ImplementInterfaces( context, usings, declarations, writer );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller." )]
        public Stream Generate( CodeGeneratorContext context )
        {
            Arg.NotNull( context, nameof( context ) );

            var stream = new MemoryStream();
            var writer = new StreamWriter( stream );

            try
            {
                EvaluateAndGenerateCode( context, writer );
            }
            catch
            {
                writer.Close();
                throw;
            }

            writer.Flush();
            stream.Seek( 0L, SeekOrigin.Begin );

            return stream;
        }
    }
}
