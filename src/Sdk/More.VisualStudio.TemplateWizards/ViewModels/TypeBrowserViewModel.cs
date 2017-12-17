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
        readonly HashSet<string> restrictedBaseTypeNames = new HashSet<string>( StringComparer.Ordinal );
        readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        readonly ObservableKeyedCollection<string, INamedCommand> dialogCommands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        readonly ObservableCollection<AssemblyName> localAssemblyReferences = new ObservableCollection<AssemblyName>();
        bool filterByConvention = false;
        string nameConvention;
        string title = SR.DefaultTypeSelectionTitle;
        Type selectedType;
        ISpecification<Type> typeFilter;
        Assembly localAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBrowserViewModel"/> class.
        /// </summary>
        public TypeBrowserViewModel()
        {
            interactionRequests.Add( close );
            dialogCommands.Add( new NamedCommand<object>( SR.OKCaption, p => close.Request( new WindowCloseInteraction() ), p => SelectedType != null ) );
            dialogCommands.Add( new NamedCommand<object>( SR.CancelCaption, p => close.Request( new WindowCloseInteraction( true ) ) ) );
        }

        static ISpecification<Type> CreateFilter( string nameConvention, ICollection<string> restrictedBaseTypeNames )
        {
            Contract.Ensures( Contract.Result<ISpecification<Type>>() != null );

            var names = restrictedBaseTypeNames;
            var unrestrictedType = new Specification<Type>( t => !t.IsRestricted( names ) );
            var core = new Specification<Type>( t => t.IsClass ).Or( t => t.IsInterface ).And( unrestrictedType );

            if ( string.IsNullOrEmpty( nameConvention ) )
            {
                return core;
            }

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
                Contract.Ensures( title != null );
                return title ?? ( title = string.Empty );
            }
            set => SetProperty( ref title, value );
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
                Contract.Ensures( interactionRequests != null );
                return interactionRequests;
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
                Contract.Ensures( dialogCommands != null );
                return dialogCommands;
            }
        }

        /// <summary>
        /// Gets or sets the local assembly used to filter types by.
        /// </summary>
        /// <value>The local <see cref="Assembly">assembly</see>.</value>
        public Assembly LocalAssembly
        {
            get => localAssembly;
            set
            {
                if ( SetProperty( ref localAssembly, value ) )
                {
                    OnPropertyChanged( nameof( LocalAssemblyName ) );
                }
            }
        }

        /// <summary>
        /// Gets the local assembly name used to filter types by.
        /// </summary>
        /// <value>The local <see cref="AssemblyName">assembly name</see>.</value>
        public AssemblyName LocalAssemblyName => LocalAssembly?.GetName();

        /// <summary>
        /// Gets a collection of references used by the local assembly.
        /// </summary>
        /// <value>A <see cref="ObservableCollection{T}">collection</see> of <see cref="AssemblyName">assembly references</see>
        /// used by the <see cref="LocalAssembly">local assembly</see>.</value>
        public ObservableCollection<AssemblyName> LocalAssemblyReferences
        {
            get
            {
                Contract.Ensures( localAssemblyReferences != null );
                return localAssemblyReferences;
            }
        }

        IReadOnlyList<AssemblyName> ILocalAssemblySource.LocalAssemblyReferences => LocalAssemblyReferences;

        /// <summary>
        /// Gets or sets a value indicating whether types are filtered by convention.
        /// </summary>
        /// <value>True if types are filtered by convention; otherwise, false. The default value is false.</value>
        public bool FilterByConvention
        {
            get => filterByConvention;
            set
            {
                if ( !SetProperty( ref filterByConvention, value ) )
                {
                    return;
                }

                var convention = value ? NameConvention : null;
                TypeFilter = CreateFilter( convention, RestrictedBaseTypeNames );
            }
        }

        /// <summary>
        /// Gets or sets the name used to filter types by convention.
        /// </summary>
        /// <value>The name of types to be filtered by convention.</value>
        /// <remarks>If the <see cref="FilterByConvention"/> is <c>false</c>, then the value of this property is ignored.</remarks>
        public string NameConvention
        {
            get => nameConvention;
            set
            {
                if ( !SetProperty( ref nameConvention, value ) )
                {
                    return;
                }

                if ( FilterByConvention )
                {
                    TypeFilter = CreateFilter( value, RestrictedBaseTypeNames );
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        /// <value>The selected type.</value>
        [Required]
        public Type SelectedType
        {
            get => selectedType;
            set
            {
                if ( SetProperty( ref selectedType, value ) )
                {
                    DialogCommands.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type filter specification.
        /// </summary>
        /// <value>A <see cref="ISpecification{T}">specification</see> representing the type filter.</value>
        /// <remarks>The default <see cref="ISpecification{T}">specification</see> is based on the values
        /// of the <see cref="FilterByConvention"/> and <see cref="NameConvention"/> properties.</remarks>
        public ISpecification<Type> TypeFilter
        {
            get
            {
                Contract.Ensures( typeFilter != null );
                return typeFilter ?? ( typeFilter = CreateFilter( NameConvention, RestrictedBaseTypeNames ) );
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );

                if ( !SetProperty( ref typeFilter, value ) )
                {
                    return;
                }

                // if the filter is updated, re-evaluate the specification if a type has been selected
                if ( value != null && SelectedType != null && !value.IsSatisfiedBy( SelectedType ) )
                {
                    SelectedType = null;
                }
            }
        }

        /// <summary>
        /// Gets a collection of restricted base type names.
        /// </summary>
        /// <value>A <see cref="ICollection{T}">collection</see> of base type names.</value>
        /// <remarks>The specified type names can be the type <see cref="Type.FullName">full name</see>
        /// or <see cref="Type.Name">simple name</see>.</remarks>
        public ICollection<string> RestrictedBaseTypeNames
        {
            get
            {
                Contract.Ensures( restrictedBaseTypeNames != null );
                return restrictedBaseTypeNames;
            }
        }
    }
}