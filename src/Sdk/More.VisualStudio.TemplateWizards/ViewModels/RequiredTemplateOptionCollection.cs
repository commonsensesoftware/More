namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using static System.String;

    /// <summary>
    /// Represents a collection of <see cref="TemplateOption">template options</see> where at least option must be
    /// enabled or selected<seealso cref="TemplateOption"/>.
    /// </summary>
    public class RequiredTemplateOptionCollection : ObservableCollection<TemplateOption>, INotifyDataErrorInfo
    {
        bool oneItemEnabled;

        /// <summary>
        /// Gets a value indicating whether at least one item is enabled.
        /// </summary>
        /// <value>True if at least one item is selected or ena</value>
        public bool AtLeastOneItemEnabled
        {
            get => oneItemEnabled;
            protected set
            {
                if ( oneItemEnabled == value )
                {
                    return;
                }

                oneItemEnabled = value;
                OnPropertyChanged( new PropertyChangedEventArgs( nameof( AtLeastOneItemEnabled ) ) );
                OnErrorsChanged( new DataErrorsChangedEventArgs( nameof( AtLeastOneItemEnabled ) ) );
            }
        }

        void OnItemPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            if ( e.PropertyName != "IsEnabled" && !IsNullOrEmpty( e.PropertyName ) )
            {
                return;
            }

            var item = (TemplateOption) sender;
            AtLeastOneItemEnabled = item.IsEnabled ? true : Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Overrides the default behavior when the collection is cleared.
        /// </summary>
        protected override void ClearItems()
        {
            Items.ForEach( i => i.PropertyChanged -= OnItemPropertyChanged );
            base.ClearItems();
            AtLeastOneItemEnabled = false;
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be inserted.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem( int index, TemplateOption item )
        {
            base.InsertItem( index, item ?? throw new ArgumentNullException( nameof( item ) ) );
            item.PropertyChanged += OnItemPropertyChanged;
            AtLeastOneItemEnabled = item.IsEnabled ? true : Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be removed.</param>
        protected override void RemoveItem( int index )
        {
            var oldItem = Items[index];
            base.RemoveItem( index );
            oldItem.PropertyChanged -= OnItemPropertyChanged;
            AtLeastOneItemEnabled = oldItem.IsEnabled ? Items.Any( i => i.IsEnabled ) : AtLeastOneItemEnabled;
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be replaced.</param>
        /// <param name="item">The replacement item.</param>
        protected override void SetItem( int index, TemplateOption item )
        {
            var oldItem = Items[index];
            base.SetItem( index, item ?? throw new ArgumentNullException( nameof( item ) ) );
            oldItem.PropertyChanged -= OnItemPropertyChanged;
            item.PropertyChanged += OnItemPropertyChanged;
            AtLeastOneItemEnabled = item.IsEnabled ? true : Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DataErrorsChangedEventArgs"/> event data.</param>
        protected virtual void OnErrorsChanged( DataErrorsChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            ErrorsChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Occurs when the data errors when the collection have changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the collection validation errors.
        /// </summary>
        /// <param name="propertyName">The name of the property to get validation errors for.</param>
        /// <returns>A <see cref="IEnumerable">sequence</see> of <see cref="ValidationResult">validation results</see>.</returns>
        public IEnumerable GetErrors( string propertyName )
        {
            var validationResults = new List<ValidationResult>();

            if ( propertyName == nameof( AtLeastOneItemEnabled ) || IsNullOrEmpty( propertyName ) )
            {
                validationResults.Add( new ValidationResult( SR.OneItemRequired ) );
            }

            return validationResults;
        }

        /// <summary>
        /// Gets a value indicating whether the collection has an errors.
        /// </summary>
        public bool HasErrors => !AtLeastOneItemEnabled;
    }
}