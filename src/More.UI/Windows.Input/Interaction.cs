namespace More.Windows.Input
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a user interaction.
    /// </summary>
    public class Interaction : ObservableObject
    {
        readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        readonly Lazy<IDictionary<string, object>> continuationData = new Lazy<IDictionary<string, object>>( () => new Dictionary<string, object>() );
        string title;
        object content;
        int defaultCommandIndex = -1;
        int cancelCommandIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        public Interaction() : this( string.Empty, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public Interaction( string title ) : this( title, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interaction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="content">The content associated with the interaction.</param>
        public Interaction( string title, object content )
        {
            Arg.NotNull( title, nameof( title ) );

            this.title = title;
            this.content = content;
        }

        /// <summary>
        /// Gets or sets the title of the interaction.
        /// </summary>
        /// <value>The interaction title.</value>
        public string Title
        {
            get
            {
                Contract.Ensures( title != null );
                return title;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref title, value );
            }
        }

        /// <summary>
        /// Gets or sets the interaction content associated with the request.
        /// </summary>
        /// <value>The interaction content.</value>
        public object Content
        {
            get => content;
            set => SetProperty( ref content, value );
        }

        /// <summary>
        /// Gets the commands associated with the interaction.
        /// </summary>
        /// <value>A <see cref="KeyedCollection{TKey,TValue}">list</see> of <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>The default implementation supports <see cref="INotifyCollectionChanged"/>.</remarks>
        public virtual KeyedCollection<string, INamedCommand> Commands
        {
            get
            {
                Contract.Ensures( commands != null );
                return commands;
            }
        }

        /// <summary>
        /// Gets or sets the default command index.
        /// </summary>
        /// <value>The default command index. The default value is -1, which indicates there is no default command.</value>
        public int DefaultCommandIndex
        {
            get => defaultCommandIndex;
            set
            {
                if ( SetProperty( ref defaultCommandIndex, value ) )
                {
                    OnPropertyChanged( nameof( DefaultCommand ) );
                }
            }
        }

        /// <summary>
        /// Gets or sets the cancel command index.
        /// </summary>
        /// <value>The cancel command index. The default value is -1, which indicates there is no default command.</value>
        public int CancelCommandIndex
        {
            get => cancelCommandIndex;
            set
            {
                if ( SetProperty( ref cancelCommandIndex, value ) )
                {
                    OnPropertyChanged( nameof( CancelCommand ) );
                }
            }
        }

        /// <summary>
        /// Gets the default command based on available commands and specified default command index.
        /// </summary>
        /// <value>A <see cref="INamedCommand">command</see> based on the <see cref="DefaultCommandIndex">default command index</see>
        /// in the available <see cref="Commands">commands</see>.</value>
        public virtual INamedCommand DefaultCommand => Commands.ElementAtOrDefault( DefaultCommandIndex );

        /// <summary>
        /// Gets the cancel command based on available commands and specified cancel command index.
        /// </summary>
        /// <value>A <see cref="INamedCommand">command</see> based on the <see cref="CancelCommandIndex">cancel command index</see>
        /// in the available <see cref="Commands">commands</see>. If the <see cref="CancelCommandIndex">cancel command index</see>
        /// is less than zero, the cancel command is assumed to be the last command; otherwise, null is returned.</value>
        public virtual INamedCommand CancelCommand => Commands.ElementAtOrDefault( CancelCommandIndex );

        /// <summary>
        /// Gets a collection of continuation data.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">collection</see> containing continuation data.</value>
        /// <remarks>This property is typically only used on platforms that support interactions with
        /// continuations such as Windows Phone.</remarks>
        public virtual IDictionary<string, object> ContinuationData
        {
            get
            {
                Contract.Ensures( continuationData != null );
                return continuationData.Value;
            }
        }
    }
}