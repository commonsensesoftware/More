namespace More.VisualStudio.Templates
{
    using Microsoft.VisualStudio.ComponentModelHost;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using ServiceContainer = More.ServiceContainer;

    /// <summary>
    /// Represents the context for template wizard runs.
    /// </summary>
    public class TemplateWizardContext : ServiceContainer
    {
        static readonly Stack<TemplateWizardContext> contextStack = new Stack<TemplateWizardContext>();
        static readonly HashSet<string> reserved = new HashSet<string>( StringComparer.OrdinalIgnoreCase )
        {
            "$guid1$", "$guid2$", "$guid3$", "$guid4$", "$guid5$", "$guid6$", "$guid7$", "$guid8$", "$guid9$", "$guid10$",
            "$time$", "$year$", "$username$", "$userdomain$", "$machinename$", "$clrversion$", "$registeredorganization$",
            "$solutiondirectory$", "$rootname$", "$targetframeworkversion$", "$safeitemname$", "$rootnamespace$", "$projectname$",
            "$safeprojectname$", "$currentuiculturename$", "$installpath$", "$specifiedsolutionname$", "$exclusiveproject$",
            "$destinationdirectory$", "$itemname$", "$nugetpackagesfolder$", "$timestamp$", "$fileinputname$", "$fileinputextension$",
            "$safeitemrootname$", "$wizarddata$"
        };
        readonly IDictionary<string, object> sharedState;
        readonly IDictionary<string, string> currentReplacements;
        bool disposed;
        bool interactive;
        bool abandoned;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateWizardContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IOleServiceProvider">service provider</see> associated with the created context.</param>
        /// <param name="replacements">The <see cref="IDictionary{TKey,TValue}">collection</see> of template replacements for the created context.</param>
        [CLSCompliant( false )]
        public TemplateWizardContext( IOleServiceProvider serviceProvider, IDictionary<string, string> replacements ) : base( CreateParentContainer( serviceProvider ) )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( replacements, nameof( replacements ) );

            var parent = contextStack.Any() ? contextStack.Peek() : null;

            currentReplacements = replacements;

            if ( parent == null )
            {
                IsInteractive = true;
                sharedState = new Dictionary<string, object>();
            }
            else
            {
                IsInteractive = parent.IsInteractive;
                sharedState = parent.sharedState;

                foreach ( var replacement in parent.Replacements )
                {
                    if ( !reserved.Contains( replacement.Key ) )
                    {
                        currentReplacements[replacement.Key] = replacement.Value;
                    }
                }
            }

            contextStack.Push( this );
            ServiceProvider.SetCurrent( this );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The parent container is disposed, when the current container is disposed." )]
        static IServiceContainer CreateParentContainer( IOleServiceProvider serviceProvider )
        {
            Contract.Requires( serviceProvider != null );
            Contract.Ensures( Contract.Result<IServiceContainer>() != null );

            var parent = new VisualStudioServiceProvider( serviceProvider );
            parent.AddService( typeof( IComponentModel ), ( c, t ) => c.GetService( typeof( SComponentModel ) ) );
            parent.AddService( typeof( IValidator ), ( c, t ) => new ValidatorAdapter() );
            return parent;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="TemplateWizardContext"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;
            base.Dispose( disposing );

            if ( !disposing )
            {
                return;
            }

            if ( Parent is IDisposable parent )
            {
                parent.Dispose();
            }

            if ( currentReplacements != null )
            {
                currentReplacements.Clear();
            }

            Abandon();
        }

        /// <summary>
        /// Abandons the current context and clears any associated state.
        /// </summary>
        public void Abandon()
        {
            if ( abandoned )
            {
                return;
            }

            abandoned = true;

            while ( contextStack.Any() )
            {
                if ( contextStack.Pop() == this )
                {
                    break;
                }
            }

            if ( contextStack.Any() )
            {
                ServiceProvider.SetCurrent( contextStack.Peek() );
            }
            else
            {
                SharedState.Clear();
                ServiceProvider.SetCurrent( ServiceProvider.Default );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current template wizard is interactive.
        /// </summary>
        /// <value>True if the current template wizard is interactive; otherwise, false.
        /// The default value is <c>true</c>.</value>
        /// <remarks>When template wizards are chained together, parent template wizards
        /// may want to indicate the context is no longer interactive to suppress child
        /// template wizard user interfaces.</remarks>
        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        public bool IsInteractive
        {
            get => interactive;
            set
            {
                interactive = value;
                currentReplacements["$runsilent$"] = ( !value ).ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Returns an object that represents a non-interactive scope entered by the current context.
        /// </summary>
        /// <returns>A <see cref="IDisposable">disposable</see> object representing the scope.</returns>
        /// <remarks>The scope is exited when the returned object is <see cref="M:IDisposable.Dispose">disposed</see>.</remarks>
        public IDisposable EnterNonInteractiveScope() => new NonInteractiveScope( this, ( @this, value ) => @this.IsInteractive = value );

        /// <summary>
        /// Gets replacements defined in the current context.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">collection</see> of replacements defined in the current context.</value>
        public IDictionary<string, string> Replacements
        {
            get
            {
                Contract.Ensures( currentReplacements != null );
                return currentReplacements;
            }
        }

        /// <summary>
        /// Gets a state bag of information that can be shared between template wizards.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">collection</see> of information shared between template wizards.</value>
        public IDictionary<string, object> SharedState
        {
            get
            {
                Contract.Ensures( sharedState != null );
                return sharedState;
            }
        }
    }
}