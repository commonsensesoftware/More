namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Defines the behavior of a activity.
    /// </summary>
    [ContractClass( typeof( IActivityContract ) )]
    public interface IActivity : INamedComponent, ICommand, INotifyPropertyChanged, IEquatable<IActivity>
    {
        /// <summary>
        /// Gets the activity identifier.
        /// </summary>
        /// <value>A <see cref="Guid"/> structure.</value>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets the activity instance identifier.
        /// </summary>
        /// <value>A <see cref="Nullable{T}">nullable</see> <see cref="Guid"/> structure.</value>
        Guid? InstanceId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the activity has completed.
        /// </summary>
        /// <value>True if the activity has completed; otherwise, false.</value>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets or sets the expiration date and time of the activity.
        /// </summary>
        /// <value>A <see cref="Nullable{T}"/> object.  A state of null indicates the
        /// activity does not expire.</value>
        DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets a collection of dependent activities for the current activity.
        /// </summary>
        /// <value>An <see cref="ICollection{T}"/> object.</value>
        ICollection<IActivity> Dependencies { get; }

        /// <summary>
        /// Loads the serialized activity state from the specified state bag.
        /// </summary>
        /// <param name="stateBag">The <see cref="IDictionary{TKey,TValue}"/> containing the state information.</param>
        void LoadState( IDictionary<string, string> stateBag );

        /// <summary>
        /// Saves the serialized activity state to the specified state bag.
        /// </summary>
        /// <param name="stateBag">The <see cref="IDictionary{TKey,TValue}"/> containing the state information.</param>
        void SaveState( IDictionary<string, string> stateBag );

        /// <summary>
        /// Returns a value indicating whether the activity can execute.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the activity.</param>
        /// <returns>True if the activity can execute; otherwise, false.</returns>
        bool CanExecute( IServiceProvider serviceProvider );

        /// <summary>
        /// Executes the activity.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the activity.</param>
        void Execute( IServiceProvider serviceProvider );

        /// <summary>
        /// Occurs when the activity has completed.
        /// </summary>
        event EventHandler<ActivityCompletedEventArgs> Completed;
    }
}