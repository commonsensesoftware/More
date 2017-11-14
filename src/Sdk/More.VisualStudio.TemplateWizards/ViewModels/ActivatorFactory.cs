namespace More.VisualStudio.ViewModels
{
    using EnvDTE;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using static System.Reflection.BindingFlags;

    static class ActivatorFactory
    {
        static readonly Lazy<Type> projectExecutionContextType = new Lazy<Type>( () => Type.GetType( "Microsoft.Data.Entity.Design.VisualStudio.ProjectExecutionContext, Microsoft.Data.Entity.Design, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true ) );

        internal static IProjectExecutionContext NewProjectExecutionContext( Project project, IServiceProvider serviceProvider )
        {
            Contract.Requires( project != null );
            Contract.Requires( serviceProvider != null );
            Contract.Ensures( Contract.Result<IProjectExecutionContext>() != null );

            var culture = CultureInfo.CurrentCulture;
            var flags = Instance | Public;
            var baseType = projectExecutionContextType.Value;
            var args = new object[] { project, serviceProvider };
            var @base = Activator.CreateInstance( baseType, flags, null, args, culture );
            var type = typeof( ProjectExecutionContextAdapter<> ).MakeGenericType( @base.GetType() );
            var instance = (IProjectExecutionContext) Activator.CreateInstance( type, flags, null, new object[] { @base }, culture );

            return instance;
        }
    }
}