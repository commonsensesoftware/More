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
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    sealed class CSharpDbContextCodeGenerator : ICodeGenerator
    {
        const string Comment = "//";
        const string DbContext = nameof( DbContext );
        const string IReadOnlyRepository = nameof( IReadOnlyRepository );
        const string IRepository = nameof( IRepository );
        const string IUnitOfWork = nameof( IUnitOfWork );
        const string INotifyPropertyChanged = nameof( INotifyPropertyChanged );
        static readonly Lazy<IReadOnlyList<UsingDirectiveSyntax>> requiredUsings = new Lazy<IReadOnlyList<UsingDirectiveSyntax>>( CreateRequiredUsings );
        static readonly Lazy<ISpecification<GenericNameSyntax>> interfaceSpecification = new Lazy<ISpecification<GenericNameSyntax>>( CreateInterfaceSpecification );

        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CodeDom", Justification = "False positive. Literal code text." )]
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ComponentModel", Justification = "False positive. Literal code text." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Whitespace(System.String)", Justification = "Literal code elements cannot be localized." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(System.String,System.Int32,System.Boolean)", Justification = "Literal code elements cannot be localized." )]
        static IReadOnlyList<UsingDirectiveSyntax> CreateRequiredUsings()
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
                UsingDirective( ParseName( "System.Data.Entity.Infrastructure" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Linq" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Threading" ).WithLeadingTrivia( space ) ),
                UsingDirective( ParseName( "System.Threading.Tasks" ).WithLeadingTrivia( space ) )
            };
        }

        static ISpecification<GenericNameSyntax> CreateInterfaceSpecification()
        {
            Contract.Ensures( Contract.Result<ISpecification<GenericNameSyntax>>() != null );

            var specification = new Specification<GenericNameSyntax>( i => i.Identifier.Text == IReadOnlyRepository )
                                                                 .Or( i => i.Identifier.Text == IRepository )
                                                                 .Or( i => i.Identifier.Text == IUnitOfWork );

            return specification;
        }

        static IReadOnlyList<UsingDirectiveSyntax> RequiredUsings
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<UsingDirectiveSyntax>>() != null );
                return requiredUsings.Value;
            }
        }

        static ISpecification<GenericNameSyntax> InterfaceSpecification
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<GenericNameSyntax>>() != null );
                return interfaceSpecification.Value;
            }
        }

        static IReadOnlyList<ClassDeclarationSyntax> FindClasses( SyntaxNode node )
        {
            Contract.Requires( node != null );
            Contract.Ensures( Contract.Result<IEnumerable<ClassDeclarationSyntax>>() != null );

            var classes = from @class in node.DescendantNodes().OfType<ClassDeclarationSyntax>()
                          from name in @class.BaseList.DescendantNodes().OfType<IdentifierNameSyntax>()
                          where name.Identifier.Text == DbContext
                          select @class;

            return classes.ToArray();
        }

        static IReadOnlyList<UsingDirectiveSyntax> FindUsings( SyntaxNode node )
        {
            Contract.Requires( node != null );
            Contract.Ensures( Contract.Result<IEnumerable<UsingDirectiveSyntax>>() != null );
            return node.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
        }

        static IReadOnlyList<InterfaceDeclaration> FindInterfaceDeclarations( IEnumerable<ClassDeclarationSyntax> classes )
        {
            Contract.Requires( classes != null );
            Contract.Ensures( Contract.Result<IEnumerable<InterfaceDeclaration>>() != null );

            var interfaces = from @class in classes
                             from @interface in @class.BaseList.DescendantNodes().OfType<GenericNameSyntax>()
                             where InterfaceSpecification.IsSatisfiedBy( @interface )
                             select new InterfaceDeclaration( @class, @interface );

            return interfaces.ToArray();
        }

        static void ImplementInterfaces( CodeGeneratorContext context, IReadOnlyList<UsingDirectiveSyntax> usings, IReadOnlyList<InterfaceDeclaration> declarations, TextWriter writer )
        {
            Contract.Requires( context != null );
            Contract.Requires( declarations != null );
            Contract.Requires( writer != null );

            var iterator = declarations.GetEnumerator();

            if ( !iterator.MoveNext() )
            {
                return;
            }

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

        static string ResolveNamespace( ClassDeclarationSyntax @class, string defaultNamespace )
        {
            Contract.Requires( @class != null );
            Contract.Requires( defaultNamespace != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var @namespace = @class.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            if ( @namespace == null )
            {
                return defaultNamespace;
            }

            // if the namespace has multiple components (e.g. dotted), then it's a "qualified name"
            if ( @namespace.Name is QualifiedNameSyntax qualifiedName )
            {
                return qualifiedName.ToString();
            }

            // if namespace has only one component, then it's an identifier
            var name = (IdentifierNameSyntax) @namespace.Name;
            return name.Identifier.Text;
        }

        static string ResolveScopeModifier( ClassDeclarationSyntax @class )
        {
            Contract.Requires( @class != null );
            Contract.Ensures( Contract.Result<string>() != null );

            foreach ( var modifier in @class.Modifiers )
            {
                var kind = modifier.Kind();

                switch ( kind )
                {
                    case PrivateKeyword:
                    case ProtectedKeyword:
                    case PublicKeyword:
                    case InternalKeyword:
                        return modifier.Text + " ";
                }
            }

            return string.Empty;
        }

        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CodeDom", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ComponentModel", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Whitespace(System.String)", Justification = "A space does not require localization." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(System.String,System.Int32,System.Boolean)", Justification = "These literals are namespaces and cannot be localized." )]
        static void WriteUsings( IndentingTextWriter writer, IReadOnlyList<UsingDirectiveSyntax> declaredUsings )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaredUsings != null );

            var usings = new SortedSet<UsingDirectiveSyntax>( UsingDirectiveComparer.Instance );

            usings.AddRange( RequiredUsings );
            usings.AddRange( declaredUsings );

            foreach ( var @using in usings )
            {
                writer.WriteLine( @using );
            }
        }

        static bool WriteStartClass( IndentingTextWriter writer, string defaultNamespace, IReadOnlyList<UsingDirectiveSyntax> usings, ClassDeclarationSyntax @class )
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
            writer.WriteLine( "[GeneratedCode( \"More Framework\", \"1.2\" )]" );
            writer.WriteLine( "{0}partial class {1}", ResolveScopeModifier( @class ), className );
            writer.WriteLine( "{" );
            writer.Indent();

            return hasNamespace;
        }

        static void WriteEndClass( IndentingTextWriter writer, bool hasNamespace )
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

        static void ImplementInterface( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
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
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnAdd( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnRemove( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnUpdate( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnDiscardChange( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteReadOnlyRepository( writer, inheritedDeclaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    WriteRepository( writer, declaration, implementedInterfaces );
                    break;
                case IUnitOfWork:
                    if ( WritePropertyChangedImplementation( writer, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnAdd( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnRemove( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnUpdate( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    if ( WriteOnDiscardChange( writer, declaration, implementedInterfaces ) )
                    {
                        writer.WriteLine();
                    }

                    WriteUnitOfWork( writer, declaration, implementedInterfaces );
                    break;
            }
        }

        static bool WritePropertyChangedImplementation( IndentingTextWriter writer, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( INotifyPropertyChanged ) )
            {
                return false;
            }

            implementedInterfaces.Add( INotifyPropertyChanged );

            writer.WriteLine( "public event PropertyChangedEventHandler PropertyChanged;" );
            writer.WriteLine();
            writer.WriteLine( "private void OnPropertyChanged( PropertyChangedEventArgs e ) => PropertyChanged?.Invoke( this, e );" );

            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteOnAdd( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            var add = declaration.ArgumentTypeName + ".OnAdd";

            if ( implementedInterfaces.Contains( add ) )
            {
                return false;
            }

            implementedInterfaces.Add( add );
            writer.WriteLine( $"partial void OnAdd( {declaration.ArgumentTypeName} item );" );
            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteOnRemove( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            var remove = declaration.ArgumentTypeName + ".OnRemove";

            if ( implementedInterfaces.Contains( remove ) )
            {
                return false;
            }

            implementedInterfaces.Add( remove );
            writer.WriteLine( $"partial void OnRemove( {declaration.ArgumentTypeName} item );" );
            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteOnUpdate( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            var update = declaration.ArgumentTypeName + ".OnUpdate";

            if ( implementedInterfaces.Contains( update ) )
            {
                return false;
            }

            implementedInterfaces.Add( update );
            writer.WriteLine( $"partial void OnUpdate( {declaration.ArgumentTypeName} item );" );
            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteOnDiscardChange( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            var discardChange = declaration.ArgumentTypeName + ".OnDiscardChange";

            if ( implementedInterfaces.Contains( discardChange ) )
            {
                return false;
            }

            implementedInterfaces.Add( discardChange );
            writer.WriteLine( $"partial void OnDiscardChange( DbEntityEntry<{declaration.ArgumentTypeName}> entry );" );
            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteReadOnlyRepository( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
            {
                return false;
            }

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( $"async Task<IEnumerable<{declaration.ArgumentTypeName}>> {declaration.TypeName}.GetAsync( Func<IQueryable<{declaration.ArgumentTypeName}>, IQueryable<{declaration.ArgumentTypeName}>> queryShaper, CancellationToken cancellationToken )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( $"return await queryShaper( Set<{declaration.ArgumentTypeName}>() ).ToArrayAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"async Task<TResult> {declaration.TypeName}.GetAsync<TResult>( Func<IQueryable<{declaration.ArgumentTypeName}>, TResult> queryShaper, CancellationToken cancellationToken )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( $"return await Task<TResult>.Factory.StartNew( () => queryShaper( Set<{declaration.ArgumentTypeName}>() ), cancellationToken ).ConfigureAwait( false );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteRepository( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
            {
                return false;
            }

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( $"bool {declaration.TypeName}.HasPendingChanges => ChangeTracker.HasChanges();" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.Add( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnAdd( item );" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Add( item );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.Remove( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnRemove( item );" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Remove( item );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.Update( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnUpdate( item );" );
            writer.WriteLine( "if ( Entry( item ).State != EntityState.Detached )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Attach( item );" );
            writer.WriteLine( "Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.DiscardChanges()" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( $"foreach ( var entry in ChangeTracker.Entries<{declaration.ArgumentTypeName}>() )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnDiscardChange( entry );" );
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
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"async Task IRepository<{declaration.ArgumentTypeName}>.SaveChangesAsync( CancellationToken cancellationToken )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "The current context is invariant" )]
        [SuppressMessage( "Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "The current context is invariant" )]
        static bool WriteUnitOfWork( IndentingTextWriter writer, InterfaceDeclaration declaration, ICollection<string> implementedInterfaces )
        {
            Contract.Requires( writer != null );
            Contract.Requires( declaration != null );
            Contract.Requires( implementedInterfaces != null );

            if ( implementedInterfaces.Contains( declaration.TypeName ) )
            {
                return false;
            }

            implementedInterfaces.Add( declaration.TypeName );

            writer.WriteLine( $"bool {declaration.TypeName}.HasPendingChanges => ChangeTracker.HasChanges();" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.RegisterNew( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnAdd( item );" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Add( item );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.RegisterRemoved( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnRemove( item );" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Remove( item );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.RegisterChanged( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnUpdate( item );" );
            writer.WriteLine( "if ( Entry( item ).State != EntityState.Detached )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "return;" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine( $"Set<{declaration.ArgumentTypeName}>().Attach( item );" );
            writer.WriteLine( "Entry( item ).State = EntityState.Modified;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.Unregister( {declaration.ArgumentTypeName} item )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "Entry( item ).State = EntityState.Detached;" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"void {declaration.TypeName}.Rollback()" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( $"foreach ( var entry in ChangeTracker.Entries<{declaration.ArgumentTypeName}>() )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "OnDiscardChange( entry );" );
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
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );
            writer.WriteLine();
            writer.WriteLine( $"async Task {declaration.TypeName}.CommitAsync( CancellationToken cancellationToken )" );
            writer.WriteLine( "{" );
            writer.Indent();
            writer.WriteLine( "await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );" );
            writer.WriteLine( "OnPropertyChanged( new PropertyChangedEventArgs( \"HasPendingChanges\" ) );" );
            writer.Unindent();
            writer.WriteLine( "}" );

            return true;
        }

        static void EvaluateAndGenerateCode( CodeGeneratorContext context, StreamWriter writer )
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
            {
                writer.WriteLine( SR.NoInterfacesFound.FormatDefault( typeof( IReadOnlyRepository<> ), typeof( IRepository<> ), typeof( IUnitOfWork<> ) ) );
            }
            else
            {
                ImplementInterfaces( context, usings, declarations, writer );
            }
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
            stream.Position = 0L;

            return stream;
        }
    }
}