namespace More.Windows.Interactivity
{
    using Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel.Contacts;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to select one or more contacts for the
    /// <see cref="SelectContactInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class SelectContactAction : System.Windows.Interactivity.TriggerAction
    {
        [Conditional( "PORTABLE_WP81_WPA81" )]
        static void TryIfImplemented( Action action )
        {
            // HACK: some features are not available on WPA81. handle these
            // actions gracefully. we don't ignore them as they may be available
            // in later builds such as WPA10+.

            try
            {
                action();
            }
            catch ( NotImplementedException )
            {
                // ignore features that aren't implemented
            }
        }

        async Task<IList<Contact>> SelectContactsAsync( SelectContactInteraction selectContact )
        {
            Contract.Requires( selectContact != null );
            Contract.Ensures( Contract.Result<Task<IList<Contact>>>() != null );

            var commitButtonText = selectContact.DefaultCommand?.Name;
            var dialog = new ContactPicker();

            dialog.DesiredFieldsWithContactFieldType.AddRange( selectContact.DesiredFields );
#if PORTABLE_WP81_WPA81
            TryIfImplemented( () => dialog.SelectionMode = selectContact.DesiredFields.Any() ? ContactSelectionMode.Fields : ContactSelectionMode.Contacts );
            TryIfImplemented( () => { if ( !string.IsNullOrEmpty( commitButtonText ) ) dialog.CommitButtonText = commitButtonText; } );
#else
            dialog.SelectionMode = selectContact.DesiredFields.Any() ? ContactSelectionMode.Fields : ContactSelectionMode.Contacts;

            if ( !string.IsNullOrEmpty( commitButtonText ) )
            {
                dialog.CommitButtonText = commitButtonText;
            }
#endif
            if ( selectContact.Multiselect )
            {
                return await dialog.PickContactsAsync();
            }

            var contacts = new List<Contact>();
            var contact = await dialog.PickContactAsync();

            if ( contact != null )
            {
                contacts.Add( contact );
            }

            return contacts;
        }

        static void InvokeCallbackCommand( SelectContactInteraction selectContact, IList<Contact> contacts )
        {
            Contract.Requires( selectContact != null );
            Contract.Requires( contacts != null );

            if ( contacts.Any() )
            {
                selectContact.Contacts.ReplaceAll( contacts );
                selectContact.ExecuteDefaultCommand();
            }
            else
            {
                selectContact.ExecuteCancelCommand();
            }
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var selectContact = GetRequestedInteraction<SelectContactInteraction>( parameter );

            if ( selectContact == null )
            {
                return;
            }

            var contacts = await SelectContactsAsync( selectContact );
            InvokeCallbackCommand( selectContact, contacts );
        }
    }
}