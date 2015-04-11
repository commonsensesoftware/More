namespace More.VisualStudio.Editors.EntityFramework
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    internal class CSharpDbContextCodeGenerator : ICodeGenerator
    {
        private const string Comment = "//";
        private const string DbContext = "DbContext";
        private const string IReadOnlyRepository = "IReadOnlyRepository";
        private const string IRepository = "IRepository";
        private const string IUnitOfWork = "IUnitOfWork";
        private const string INotifyPropertyChanged = "INotifyPropertyChanged";

        private readonly Lazy<ISpecification<GenericNameSyntax>> interfaceSpecification = new Lazy<ISpecification<GenericNameSyntax>>( CreateInterfaceSpecification );

        private static ISpecification<GenericNameSyntax> CreateInterfaceSpecification()
        {
            var specification = new Specification<GenericNameSyntax>( i => i.Identifier.Text == IReadOnlyRepository )
                                                                 .Or( i => i.Identifier.Text == IRepository )
                                                                 .Or( i => i.Identifier.Text == IUnitOfWork );

            return specification;
        }

        private ISpecification<GenericNameSyntax> InterfaceSpecification
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<GenericNameSyntax>>() != null );
                return this.interfaceSpecification.Value;
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

        private IReadOnlyList<InterfaceDeclaration> FindInterfaceDeclarations( IEnumerable<ClassDeclarationSyntax> classes )
        {
            Contract.Requires( classes != null );
            Contract.Ensures( Contract.Result<IEnumerable<InterfaceDeclaration>>() != null );

            var interfaces = from @class in classes
                             from @interface in @class.BaseList.DescendantNodes().OfType<GenericNameSyntax>()
                             where this.InterfaceSpecification.IsSatisfiedBy( @interface )
                             select new InterfaceDeclaration( @class, @interface );

            return interfaces.ToArray();
        }

        private static void ImplementInterfaces( CodeGeneratorContext context, IReadOnlyList<InterfaceDeclaration> declarations, TextWriter writer )
        {
            Contract.Requires( context != null );
            Contract.Requires( declarations != null );
            Contract.Requires( writer != null );

            var iterator = declarations.GetEnumerator();

            if ( !iterator.MoveNext() )
                return;

            var indentingWriter = new IndentingTextWriter( writer );
            var implementedInterfaces = new HashSet<string>();
            var hasNamespace = WriteStartClass( indentingWriter, context.DefaultNamespace, iterator.Current.DefiningClass );

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

        private static bool WriteStartClass( IndentingTextWriter writer, string defaultNamespace, ClassDeclarationSyntax @class )
        {
            Contract.Requires( writer != null );
            Contract.Requires( defaultNamespace != null );
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

            writer.WriteLine( "using More.ComponentModel;" );
            writer.WriteLine( "using System;" );
            writer.WriteLine( "using System.CodeDom.Compiler;" );
            writer.WriteLine( "using System.Collections.Generic;" );
            writer.WriteLine( "using System.ComponentModel;" );
            writer.WriteLine( "using System.Data.Entity;" );
            writer.WriteLine( "using System.Linq;" );
            writer.WriteLine( "using System.Threading;" );
            writer.WriteLine( "using System.Threading.Tasks;" );
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
                    WriteReadOnlyRepositoryImplementation( writer, declaration, implementedInterfaces );
                    break;
                case IRepository:
                    var inheritedDeclaration = new InterfaceDeclaration( IReadOnlyRepository, declaration );

                    if ( WritePropertyChangedImplementation( writer, implementedInterfaces ) )
                        writer.WriteLine();
                    
                    if ( WriteReadOnlyRepositoryImplementation( writer, inheritedDeclaration, implementedInterfaces ) )
                        writer.WriteLine();

                    WriteRepositoryImplementation( writer, declaration, implementedInterfaces );
                    break;
                case IUnitOfWork:
                    if ( WritePropertyChangedImplementation( writer, implementedInterfaces ) )
                        writer.WriteLine();

                    WriteUnitOfWorkImplementation( writer, declaration, implementedInterfaces );
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

            writer.WriteLine( "private volatile PropertyChangedEventHandler propertyChanged;" );
            writer.WriteLine();
            writer.WriteLine( "event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "add" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.propertyChanged += value;" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine( "remove" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.propertyChanged -= value;" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "private void OnPropertyChanged( PropertyChangedEventArgs e )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "var handler = this.propertyChanged;" );
            writer.WriteLine();
            writer.WriteLine( "if ( handler != null )" );
            writer.Indent();
            writer.WriteLine( "handler( this, e );" );
            writer.Unindent();
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static bool WriteReadOnlyRepositoryImplementation( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.Key ) )
                return false;

            implementedInterfaces.Add( declaration.Key );

            writer.WriteLine( "async Task<IEnumerable<{1}>> {0}.GetAsync( Func<IQueryable<{1}>, IQueryable<{1}>> queryShaper, CancellationToken cancellationToken )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return await queryShaper( this.Set<{0}>() ).ToArrayAsync( cancellationToken ).ConfigureAwait( false );", declaration.ArgumentTypeName );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task<TResult> {0}.GetAsync<TResult>( Func<IQueryable<{1}>, TResult> queryShaper, CancellationToken cancellationToken )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return await Task<TResult>.Factory.StartNew( () => queryShaper( this.Set<{0}>() ), cancellationToken ).ConfigureAwait( false );", declaration.ArgumentTypeName );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static bool WriteRepositoryImplementation( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.Key ) )
                return false;

            implementedInterfaces.Add( declaration.Key );

            writer.WriteLine( "bool {0}.HasPendingChanges", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "get" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return this.ChangeTracker.HasChanges();" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Add( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.Set<{0}>().Add( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Remove( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.Set<{0}>().Remove( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Update( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "if ( this.Entry( item ).State != EntityState.Detached )" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine();
            writer.WriteLine( "this.Set<{0}>().Attach( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.DiscardChanges()", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "foreach ( var entry in this.ChangeTracker.Entries<{0}>() )", declaration.ArgumentTypeName );
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
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task IRepository<{0}>.SaveChangesAsync( CancellationToken cancellationToken )", declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await this.SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private static bool WriteUnitOfWorkImplementation( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.Key ) )
                return false;

            implementedInterfaces.Add( declaration.Key );

            writer.WriteLine( "bool {0}.HasPendingChanges", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "get" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return this.ChangeTracker.HasChanges();" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterNew( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.Set<{0}>().Add( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterRemoved( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.Set<{0}>().Remove( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.RegisterChanged( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "if ( this.Entry( item ).State != EntityState.Detached )" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine();
            writer.WriteLine( "this.Set<{0}>().Attach( item );", declaration.ArgumentTypeName );
            writer.WriteLine( "this.Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Unregister( {1} item )", declaration.TypeName, declaration.ArgumentTypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "this.Entry( item ).State = EntityState.Detached;" );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "void {0}.Rollback()", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "foreach ( var entry in this.ChangeTracker.Entries<{0}>() )", declaration.ArgumentTypeName );
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
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( "async Task {0}.CommitAsync( CancellationToken cancellationToken )", declaration.TypeName );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await this.SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "this.OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        private void EvaluateAndGenerateCode( CodeGeneratorContext context, StreamWriter writer )
        {
            Contract.Requires( context != null );
            Contract.Requires( writer != null );

            var ast = CSharpSyntaxTree.ParseText( context.FileContents );
            var root = ast.GetRoot();
            var classes = FindClasses( root );

            if ( classes.Count == 0 )
            {
                writer.WriteLine( SR.DbContextClassNotFound.FormatDefault( Comment ) );
                return;
            }

            var declarations = this.FindInterfaceDeclarations( classes );

            if ( declarations.Count == 0 )
                writer.WriteLine( SR.NoInterfacesFound.FormatDefault( typeof( IReadOnlyRepository<> ), typeof( IRepository<> ), typeof( IUnitOfWork<> ) ) );
            else
                ImplementInterfaces( context, declarations, writer );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller." )]
        public Stream Generate( CodeGeneratorContext context )
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter( stream );

            try
            {
                this.EvaluateAndGenerateCode( context, writer );
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
