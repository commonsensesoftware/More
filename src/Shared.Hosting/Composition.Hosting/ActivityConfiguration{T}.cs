namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the configuration for an activity.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="IActivity">activity</see> to configure.</typeparam>
    /// <example>This example illustrates how to register and configure a startup activity.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.ComponentModel;
    /// using System.Composition;
    /// using System.Composition.Hosting;
    ///
    /// public class Program
    /// {
    ///     [STAThread]
    ///     public static void Main()
    ///     {
    ///         var configRoot = new Uri( "myconfig.xml", UriKind.Relative );
    ///
    ///         using ( var host = new Host() )
    ///         {
    ///             // register and configure fluently
    ///             host.Register<LoadConfiguration>().Configure( lc => lc.ConfigurationRoot = configRoot );
    ///         }
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public class ActivityConfiguration<T> : IActivityConfiguration where T : IActivity
    {
        readonly List<Action<T>> configurations = new List<Action<T>>();
        readonly List<Type> dependencies = new List<Type>();

        /// <summary>
        /// Adds a configuration action.
        /// </summary>
        /// <param name="configuration">The configuration <see cref="Action{T}">action</see>.</param>
        /// <returns>The original <see cref="ActivityConfiguration{T}">activity configuration</see>.</returns>
        /// <example>This example illustrates how to register and configure a startup activity.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System;
        ///
        /// public class Program
        /// {
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         var configRoot = new Uri( "myconfig.xml", UriKind.Relative );
        ///
        ///         using ( var host = new Host() )
        ///         {
        ///             // register and configure fluently
        ///             host.Register<LoadConfiguration>().Configure( lc => lc.ConfigurationRoot = configRoot );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public ActivityConfiguration<T> Configure( Action<T> configuration )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Contract.Ensures( Contract.Result<ActivityConfiguration<T>>() != null );
            configurations.Add( configuration );
            return this;
        }

        /// <summary>
        /// Configures the activity with a dependency to another activity.
        /// </summary>
        /// <param name="activityType">The <see cref="IActivity">activity</see> <see cref="Type">type</see> the
        /// configured <see cref="IActivity">activity</see> depends on.</param>
        /// <returns>The original <see cref="ActivityConfiguration{T}">activity configuration</see>.</returns>
        /// <example>This example illustrates how to add an activity dependency.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        ///
        /// public class Program
        /// {
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         using ( var host = new Host() )
        ///         {
        ///             // register and configure fluently
        ///             host.Register<LoadConfiguration>()
        ///             host.Register<CreateSecurityPrincipal>().DependsOn( typeof( LoadConfiguration ) );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public virtual ActivityConfiguration<T> DependsOn( Type activityType )
        {
            Arg.NotNull( activityType, nameof( activityType ) );
            Contract.Ensures( Contract.Result<ActivityConfiguration<T>>() != null );
            dependencies.Add( activityType );
            return this;
        }

        /// <summary>
        /// Configures the activity with a dependency to another activity.
        /// </summary>
        /// <typeparam name="TActivity">The <see cref="Type">type</see> of <see cref="IActivity">activity</see> the
        /// configured activity depends on.</typeparam>
        /// <returns>The original <see cref="ActivityConfiguration{T}">activity configuration</see>.</returns>
        /// <example>This example illustrates how to add an activity dependency.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        ///
        /// public class Program
        /// {
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         using ( var host = new Host() )
        ///         {
        ///             // register and configure fluently
        ///             host.Register<LoadConfiguration>()
        ///             host.Register<CreateSecurityPrincipal>().DependsOn<LoadConfiguration>();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Prevents common coding errors." )]
        public ActivityConfiguration<T> DependsOn<TActivity>() where TActivity : IActivity
        {
            Contract.Ensures( Contract.Result<ActivityConfiguration<T>>() != null );
            return DependsOn( typeof( TActivity ) );
        }

        /// <summary>
        /// Configures the specified activity.
        /// </summary>
        /// <param name="activity">The <typeparamref name="T">activity</typeparamref> to configure.</param>
        protected virtual void Configure( T activity )
        {
            Arg.NotNull( activity, nameof( activity ) );
            configurations.ForEach( configure => configure( activity ) );
        }

        /// <summary>
        /// Gets the sequence of dependent activity types the configured activity depends on.
        /// </summary>
        /// <value>The <see cref="IActivity">activity</see> <see cref="Type">types</see> the
        /// configured <see cref="IActivity">activity</see> depends on.</value>
        public IEnumerable<Type> Dependencies => dependencies;

        void IActivityConfiguration.Configure( IActivity activity )
        {
            Arg.NotNull( activity, nameof( activity ) );
            Configure( (T) activity );
        }

        void IActivityConfiguration.DependsOn( Type activityType )
        {
            Arg.NotNull( activityType, nameof( activityType ) );
            DependsOn( activityType );
        }
    }
}