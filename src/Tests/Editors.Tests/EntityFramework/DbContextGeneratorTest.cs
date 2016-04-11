namespace More.VisualStudio.Editors.EntityFramework
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DbContextGenerator"/>.
    /// </summary>
    public class DbContextGeneratorTest : CodeGeneratorUnitTest<DbContextGenerator>
    {
        private static readonly Type INotifyPropertyChanged = typeof( INotifyPropertyChanged );
        private static readonly Type IReadOnlyRepository = typeof( IReadOnlyRepository<> );
        private static readonly Type IRepository = typeof( IRepository<> );
        private static readonly Type IUnitOfWork = typeof( IUnitOfWork<> );

        private static string CreateFileContent( params Type[] interfaceTypes )
        {
            var codeFormat =
@"namespace ClassLibrary1
{{
    using System;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Linq;
    using ConnectionStringSettings = System.Configuration.ConnectionStringSettings;

    public partial class MyDbContext : DbContext{0}
    {{
    }}
}}";
            var interfaces = new StringBuilder();

            foreach ( var interfaceType in interfaceTypes )
            {
                if ( interfaceType.Equals( IReadOnlyRepository ) )
                    interfaces.Append( ", IReadOnlyRepository<Class1>" );
                else if ( interfaceType.Equals( IRepository ) )
                    interfaces.Append( ", IRepository<Class1>" );
                else if ( interfaceType.Equals( IUnitOfWork ) )
                    interfaces.Append( ", IUnitOfWork<Class1>" );
            }

            var code = string.Format( codeFormat, interfaces );
            return code;
        }

        private static string CreateExpected( params Type[] interfaceTypes )
        {
            var code = new StringBuilder();

            code.Append(
@"namespace ClassLibrary1
{
    using More.ComponentModel;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ConnectionStringSettings = System.Configuration.ConnectionStringSettings;

    /// <content>
    /// Provides auto-generated interfaces for the <see cref=""MyDbContext"" /> class. To add addition interfaces,
    /// implement the interface in the main source file.
    /// <seealso cref=""IReadOnlyRepository{T}"" />
    /// <seealso cref=""IRepository{T}"" />
    /// <seealso cref=""IUnitOfWork{T}"" />.
    /// <content>
    [GeneratedCode( ""More Framework"", ""1.1"" )]
    public partial class MyDbContext
    {
" );
            var implementedInterfaces = new HashSet<Type>();
            var iterator = interfaceTypes.AsEnumerable().GetEnumerator();

            if ( iterator.MoveNext() )
            {
                AppendInterfaceImplementation( code, iterator.Current, implementedInterfaces );

                while ( iterator.MoveNext() )
                {
                    code.AppendLine();
                    code.AppendLine();
                    AppendInterfaceImplementation( code, iterator.Current, implementedInterfaces );
                }
            }

            code.Append( @"
    }
}" );
            return code.ToString();
        }

        private static void AppendInterfaceImplementation( StringBuilder code, Type interfaceType, ICollection<Type> implementedInterfaces )
        {
            if ( interfaceType.Equals( IReadOnlyRepository ) )
            {
                AppendReadOnlyRepositoryImplementation( code, implementedInterfaces );
            }
            else if ( interfaceType.Equals( IRepository ) )
            {
                AppendRepositoryImplementation( code, implementedInterfaces );
            }
            else if ( interfaceType.Equals( IUnitOfWork ) )
            {
                AppendUnitOfWorkImplementation( code, implementedInterfaces );
            }
        }

        private static void AppendReadOnlyRepositoryImplementation( StringBuilder code, ICollection<Type> implementedInterfaces )
        {
            if ( !implementedInterfaces.Contains( IReadOnlyRepository ) )
            {
                code.Append( CreateExpectedReadOnlyRepositoryCode() );
                implementedInterfaces.Add( IReadOnlyRepository );
            }
        }

        private static void AppendRepositoryImplementation( StringBuilder code, ICollection<Type> implementedInterfaces )
        {
            var hasPreceeding = false;

            if ( !implementedInterfaces.Contains( INotifyPropertyChanged ) )
            {
                code.Append( CreateExpectedPropertyChangedCode() );
                implementedInterfaces.Add( INotifyPropertyChanged );
                hasPreceeding = true;
            }

            if ( !implementedInterfaces.Contains( IRepository ) && !implementedInterfaces.Contains( IUnitOfWork ) )
            {
                if ( hasPreceeding )
                {
                    code.AppendLine();
                    code.AppendLine();
                }

                code.Append( CreateExpectedOnAddChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnRemoveChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnUpdateChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnDiscardChangeCode() );
            }

            if ( !implementedInterfaces.Contains( IReadOnlyRepository ) )
            {
                if ( hasPreceeding )
                {
                    code.AppendLine();
                    code.AppendLine();
                }

                code.Append( CreateExpectedReadOnlyRepositoryCode() );
                implementedInterfaces.Add( IReadOnlyRepository );
            }

            if ( !implementedInterfaces.Contains( IRepository ) )
            {
                if ( hasPreceeding )
                {
                    code.AppendLine();
                    code.AppendLine();
                }

                code.Append( CreateExpectedRepositoryCode() );
                implementedInterfaces.Add( IRepository );
            }
        }

        private static void AppendUnitOfWorkImplementation( StringBuilder code, ICollection<Type> implementedInterfaces )
        {
            var hasPreceeding = false;

            if ( !implementedInterfaces.Contains( INotifyPropertyChanged ) )
            {
                code.Append( CreateExpectedPropertyChangedCode() );
                implementedInterfaces.Add( INotifyPropertyChanged );
                hasPreceeding = true;
            }

            if ( !implementedInterfaces.Contains( IUnitOfWork ) && !implementedInterfaces.Contains( IRepository ) )
            {
                if ( hasPreceeding )
                {
                    code.AppendLine();
                    code.AppendLine();
                }

                code.Append( CreateExpectedOnAddChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnRemoveChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnUpdateChangeCode() );
                code.AppendLine();
                code.AppendLine();
                code.Append( CreateExpectedOnDiscardChangeCode() );
            }

            if ( !implementedInterfaces.Contains( IUnitOfWork ) )
            {
                if ( hasPreceeding )
                {
                    code.AppendLine();
                    code.AppendLine();
                }

                code.Append( CreateExpectedUnitOfWorkCode() );
                implementedInterfaces.Add( IUnitOfWork );
            }
        }

        private static string CreateExpectedPropertyChangedCode()
        {
            return
@"        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged( PropertyChangedEventArgs e ) => PropertyChanged?.Invoke( this, e );";
        }

        private static string CreateExpectedOnAddChangeCode() => @"        partial void OnAdd( Class1 item );";

        private static string CreateExpectedOnRemoveChangeCode() => @"        partial void OnRemove( Class1 item );";

        private static string CreateExpectedOnUpdateChangeCode() => @"        partial void OnUpdate( Class1 item );";

        private static string CreateExpectedOnDiscardChangeCode() => @"        partial void OnDiscardChange( DbEntityEntry<Class1> entry );";

        private static string CreateExpectedReadOnlyRepositoryCode()
        {
            return
@"        async Task<IEnumerable<Class1>> IReadOnlyRepository<Class1>.GetAsync( Func<IQueryable<Class1>, IQueryable<Class1>> queryShaper, CancellationToken cancellationToken )
        {
            return await queryShaper( Set<Class1>() ).ToArrayAsync( cancellationToken ).ConfigureAwait( false );
        }

        async Task<TResult> IReadOnlyRepository<Class1>.GetAsync<TResult>( Func<IQueryable<Class1>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            return await Task<TResult>.Factory.StartNew( () => queryShaper( Set<Class1>() ), cancellationToken ).ConfigureAwait( false );
        }";
        }

        private static string CreateExpectedRepositoryCode()
        {
            return
@"        bool IRepository<Class1>.HasPendingChanges => ChangeTracker.HasChanges();

        void IRepository<Class1>.Add( Class1 item )
        {
            OnAdd( item );
            Set<Class1>().Add( item );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IRepository<Class1>.Remove( Class1 item )
        {
            OnRemove( item );
            Set<Class1>().Remove( item );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IRepository<Class1>.Update( Class1 item )
        {
            OnUpdate( item );
            if ( Entry( item ).State != EntityState.Detached )
            {
                return;
            }
            Set<Class1>().Attach( item );
            Entry( item ).State = EntityState.Modified;
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IRepository<Class1>.DiscardChanges()
        {
            foreach ( var entry in ChangeTracker.Entries<Class1>() )
            {
                OnDiscardChange( entry );
                switch ( entry.State )
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.CurrentValues.SetValues( entry.OriginalValues );
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        async Task IRepository<Class1>.SaveChangesAsync( CancellationToken cancellationToken )
        {
            await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }";
        }

        private static string CreateExpectedUnitOfWorkCode()
        {
            return
@"        bool IUnitOfWork<Class1>.HasPendingChanges => ChangeTracker.HasChanges();

        void IUnitOfWork<Class1>.RegisterNew( Class1 item )
        {
            OnAdd( item );
            Set<Class1>().Add( item );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IUnitOfWork<Class1>.RegisterRemoved( Class1 item )
        {
            OnRemove( item );
            Set<Class1>().Remove( item );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IUnitOfWork<Class1>.RegisterChanged( Class1 item )
        {
            OnUpdate( item );
            if ( Entry( item ).State != EntityState.Detached )
            {
                return;
            }
            Set<Class1>().Attach( item );
            Entry( item ).State = EntityState.Modified;
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IUnitOfWork<Class1>.Unregister( Class1 item )
        {
            Entry( item ).State = EntityState.Detached;
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        void IUnitOfWork<Class1>.Rollback()
        {
            foreach ( var entry in ChangeTracker.Entries<Class1>() )
            {
                OnDiscardChange( entry );
                switch ( entry.State )
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.CurrentValues.SetValues( entry.OriginalValues );
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }

        async Task IUnitOfWork<Class1>.CommitAsync( CancellationToken cancellationToken )
        {
            await SaveChangesAsync( cancellationToken ).ConfigureAwait( false );
            OnPropertyChanged( new PropertyChangedEventArgs( ""HasPendingChanges"" ) );
        }";
        }

        [Fact( DisplayName = "generate should write code for IReadOnlyRepository<T>" )]
        public void GenerateShouldReturnExpectedOutputForIReadOnlyRepository()
        {
            var path = @"C:\temp\MyDbContext.cs";
            var content = CreateFileContent( IReadOnlyRepository );
            var defaultNamespace = "ClassLibrary1";
            var expected = CreateExpected( IReadOnlyRepository );
            string actual;
            var result = Generate( path, content, defaultNamespace, out actual );

            Assert.Equal( 0, result );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "generate should write code for IRepository<T>" )]
        public void GenerateShouldReturnExpectedOutputForIRepository()
        {
            var path = @"C:\temp\MyDbContext.cs";
            var content = CreateFileContent( IRepository );
            var defaultNamespace = "ClassLibrary1";
            var expected = CreateExpected( IRepository );
            string actual;
            var result = Generate( path, content, defaultNamespace, out actual );

            Assert.Equal( 0, result );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "generate should write code for IUnitOfWork<T>" )]
        public void GenerateShouldReturnExpectedOutputForIUnitOfWork()
        {
            var path = @"C:\temp\MyDbContext.cs";
            var content = CreateFileContent( IUnitOfWork );
            var defaultNamespace = "ClassLibrary1";
            var expected = CreateExpected( IUnitOfWork );
            string actual;
            var result = Generate( path, content, defaultNamespace, out actual );

            Assert.Equal( 0, result );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "generate should write code for multiple interfaces" )]
        public void GenerateShouldReturnExpectedOutputForMultipleInterfaces()
        {
            var path = @"C:\temp\MyDbContext.cs";
            var content = CreateFileContent( IReadOnlyRepository, IRepository, IUnitOfWork );
            var defaultNamespace = "ClassLibrary1";
            var expected = CreateExpected( IReadOnlyRepository, IRepository, IUnitOfWork );
            string actual;
            var result = Generate( path, content, defaultNamespace, out actual );

            Assert.Equal( 0, result );
            Assert.Equal( expected, actual );
        }
    }
}
