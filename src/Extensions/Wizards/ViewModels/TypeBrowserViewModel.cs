namespace More.VisualStudio.ViewModels
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a view model for selecting a type.
    /// </summary>
    public class TypeBrowserViewModel : ValidatableObject, ILocalAssemblySource
    {
        private readonly HashSet<string> restrictedBaseTypeNames = new HashSet<string>( StringComparer.Ordinal );
        private readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        private readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> dialogCommands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly ObservableCollection<AssemblyName> localAssemblyReferences = new ObservableCollection<AssemblyName>();
        private bool filterByConvention = false;
        private string nameConvention;
        private string title = SR.DefaultTypeSelectionTitle;
        private Type selectedType;
        private ISpecification<Type> typeFilter;
        private Assembly localAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBrowserViewModel"/> class.
        /// </summary>
        public TypeBrowserViewModel()
        {
            this.interactionRequests.Add( this.close );
            this.dialogCommands.Add( new NamedCommand<object>( SR.OKCaption, p => this.close.Request( new WindowCloseInteraction() ), p => this.SelectedType != null ) );
            this.dialogCommands.Add( new NamedCommand<object>( SR.CancelCaption, p => this.close.Request( new WindowCloseInteraction( true ) ) ) );
        }

        private static ISpecification<Type> CreateFilter( string nameConvention, ICollection<string> restrictedBaseTypeNames )
        {
            Contract.Ensures( Contract.Result<ISpecification<Type>>() != null );

            var names = restrictedBaseTypeNames;
            var unrestrictedType = new Specification<Type>( t => !t.IsRestricted( names ) );
            var core = new Specification<Type>( t => t.IsClass ).Or( t => t.IsInterface ).And( unrestrictedType );

            if ( string.IsNullOrEmpty( nameConvention ) )
                return core;

            var comparison = StringComparison.OrdinalIgnoreCase;
            var name = new Specification<Type>( t => t.Name.Contains( nameConvention, comparison ) )
                                           .Or( t => t.Namespace.Contains( nameConvention, comparison ) )
                                          .And( core );

            return name;
        }

        /// <summary>
        /// Gets or sets an associated title.
        /// </summary>
        /// <value>The associated title.</value>
        public string Title
        {
            get
            {
                Contract.Ensures( this.title != null );
                return this.title ?? ( this.title = string.Empty );
            }
            set
            {
                this.SetProperty( ref this.title, value );
            }
        }

        /// <summary>
        /// Gets the collection of view model interaction requests.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="IInteractionRequest">interaction requests</see>.</value>
        public ObservableKeyedCollection<string, IInteractionRequest> InteractionRequests
        {
            get
            {
                Contract.Ensures( this.interactionRequests != null );
                return this.interactionRequests;
            }
        }

        /// <summary>
        /// Gets the collection of view model dialog commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands control the behavior of a view, such as cancelling it.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> DialogCommands
        {
            get
            {
                Contract.Ensures( this.dialogCommands != null );
                return this.dialogCommands;
            }
        }

        /// <summary>
        /// Gets or sets the local assembly used to filter types by.
        /// </summary>
        /// <value>The local <see cref="Assembly">assembly</see>.</value>
        public Assembly LocalAssembly
        {
            get
            {
                return this.localAssembly;
            }
            set
            {
                if ( this.SetProperty( ref this.localAssembly, value ) )
                    this.OnPropertyChanged( "LocalAssemblyName" );
            }
        }

        /// <summary>
        /// Gets the local assembly name used to filter types by.
        /// </summary>
        /// <value>The local <see cref="AssemblyName">assembly name</see>.</value>
        public AssemblyName LocalAssemblyName
        {
            get
            {
                var value = this.LocalAssembly;
                return value == null ? null : value.GetName();
            }
        }

        /// <summary>
        /// Gets a collection of references used by the local assembly.
        /// </summary>
        /// <value>A <see cref="ObservableCollection{T">collection</see> of <see cref="AssemblyName">assembly references</see>
        /// used by the <see cref="P:LocalAssembly">local assembly</see>.</value>
        public ObservableCollection<AssemblyName> LocalAssemblyReferences
        {
            get
            {
                Contract.Ensures( this.localAssemblyReferences != null );
                return this.localAssemblyReferences;
            }
        }

        IReadOnlyList<AssemblyName> ILocalAssemblySource.LocalAssemblyReferences
        {
            get
            {
                return this.LocalAssemblyReferences;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether types are filtered by convention.
        /// </summary>
        /// <value>True if types are filtered by convention; otherwise, false. The default value is false.</value>
        public bool FilterByConvention
        {
            get
            {
                return this.filterByConvention;
            }
            set
            {
                if ( !this.SetProperty( ref this.filterByConvention, value ) )
                    return;

                var convention = value ? this.NameConvention : null;
                this.TypeFilter = CreateFilter( convention, this.RestrictedBaseTypeNames );
            }
        }

        /// <summary>
        /// Gets or sets the name used to filter types by convention.
        /// </summary>
        /// <value>The name of types to be filtered by convention.</value>
        /// <remarks>If the <see cref="P:FilterByConvention"/> is <c>false</c>, then the value of this property is ignored.</remarks>
        public string NameConvention
        {
            get
            {
                return this.nameConvention;
            }
            set
            {
                if ( !this.SetProperty( ref this.nameConvention, value ) )
                    return;

                if ( this.FilterByConvention )
                    this.TypeFilter = CreateFilter( value, this.RestrictedBaseTypeNames );
            }
        }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        /// <value>The selected type.</value>
        [Required]
        public Type SelectedType
        {
            get
            {
                return this.selectedType;
            }
            set
            {
                if ( this.SetProperty( ref this.selectedType, value ) )
                    this.DialogCommands.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the type filter specification.
        /// </summary>
        /// <value>A <see cref="ISpecification{T}">specification</see> representing the type filter.</value>
        /// <remarks>The default <see cref="ISpecification{T}">specification</see> is based on the values
        /// of the <see cref="P:FilterByConvention"/> and <see cref="P:NameConvention"/> properties.</remarks>
        public ISpecification<Type> TypeFilter
        {
            get
            {
                Contract.Ensures( this.typeFilter != null );
                return this.typeFilter ?? ( this.typeFilter = CreateFilter( this.NameConvention, this.RestrictedBaseTypeNames ) );
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null );

                if ( !this.SetProperty( ref this.typeFilter, value ) )
                    return;

                // if the filter is updated, re-evaluate the specification if a type has been selected
                if ( value != null && this.SelectedType != null && !value.IsSatisfiedBy( this.SelectedType ) )
                    this.SelectedType = null;
            }
        }

        /// <summary>
        /// Gets a collection of restricted base type names.
        /// </summary>
        /// <value>A <see cref="ICollection{T}">collection</see> of base type names.</value>
        /// <remarks>The specified type names can be the type <see cref="P:Type.FullName">full name</see>
        /// or <see cref="P:Type.Name">simple name</see>.</remarks>
        public ICollection<string> RestrictedBaseTypeNames
        {
            get
            {
                Contract.Ensures( this.restrictedBaseTypeNames != null );
                return this.restrictedBaseTypeNames;
            }
        }
    }
}
