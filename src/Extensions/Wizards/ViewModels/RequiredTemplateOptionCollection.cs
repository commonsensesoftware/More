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

    /// <summary>
    /// Represents a collection of <see cref="TemplateOption">template options</see> where at least option must be
    /// enabled or selected<seealso cref="TemplateOption"/>.
    /// </summary>
    public class RequiredTemplateOptionCollection : ObservableCollection<TemplateOption>, INotifyDataErrorInfo
    {
        private bool oneItemEnabled;

        /// <summary>
        /// Gets a value indicating whether at least one item is enabled.
        /// </summary>
        /// <value>True if at least one item is selected or ena</value>
        public bool AtLeastOneItemEnabled
        {
            get
            {
                return this.oneItemEnabled;
            }
            protected set
            {
                if ( this.oneItemEnabled == value )
                    return;

                this.oneItemEnabled = value;
                this.OnPropertyChanged( new PropertyChangedEventArgs( "AtLeastOneItemEnabled" ) );
                this.OnErrorsChanged( new DataErrorsChangedEventArgs( "AtLeastOneItemEnabled" ) );
            }
        }

        private void OnItemPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            if ( e.PropertyName != "IsEnabled" && !string.IsNullOrEmpty( e.PropertyName ) )
                return;

            var item = (TemplateOption) sender;
            this.AtLeastOneItemEnabled = item.IsEnabled ? true : this.Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Overrides the default behavior when the collection is cleared.
        /// </summary>
        protected override void ClearItems()
        {
            this.Items.ForEach( i => i.PropertyChanged -= this.OnItemPropertyChanged );
            base.ClearItems();
            this.AtLeastOneItemEnabled = false;
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be inserted.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem( int index, TemplateOption item )
        {
            if ( item == null )
                throw new ArgumentNullException( "item" );

            base.InsertItem( index, item );
            item.PropertyChanged += this.OnItemPropertyChanged;
            this.AtLeastOneItemEnabled = item.IsEnabled ? true : this.Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be removed.</param>
        protected override void RemoveItem( int index )
        {
            var oldItem = this.Items[index];
            base.RemoveItem( index );
            oldItem.PropertyChanged -= this.OnItemPropertyChanged;
            this.AtLeastOneItemEnabled = oldItem.IsEnabled ? this.Items.Any( i => i.IsEnabled ) : this.AtLeastOneItemEnabled;
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be replaced.</param>
        /// <param name="item">The replacement item.</param>
        protected override void SetItem( int index, TemplateOption item )
        {
            if ( item == null )
                throw new ArgumentNullException( "item" );

            var oldItem = this.Items[index];
            base.SetItem( index, item );
            oldItem.PropertyChanged -= this.OnItemPropertyChanged;
            item.PropertyChanged += this.OnItemPropertyChanged;
            this.AtLeastOneItemEnabled = item.IsEnabled ? true : this.Items.Any( i => i.IsEnabled );
        }

        /// <summary>
        /// Raises the <see cref="E:ErrorsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DataErrorsChangedEventArgs"/> event data.</param>
        protected virtual void OnErrorsChanged( DataErrorsChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.ErrorsChanged;

            if ( handler != null )
                handler( this, e );
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

            if ( propertyName == "AtLeastOneItemEnabled" || string.IsNullOrEmpty( propertyName ) )
                validationResults.Add( new ValidationResult( SR.OneItemRequired ) );

            return validationResults;
        }

        /// <summary>
        /// Gets a value indicating whether the collection has an errors.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return !this.AtLeastOneItemEnabled;
            }
        }
    }
}
