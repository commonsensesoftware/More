namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
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
        private async Task<IList<Contact>> SelectContactsAsync( SelectContactInteraction selectContact )
        {
            Contract.Requires( selectContact != null );
            Contract.Ensures( Contract.Result<Task<IList<Contact>>>() != null );

            var commitButton = selectContact.DefaultCommand;
            var dialog = new ContactPicker();

            dialog.DesiredFieldsWithContactFieldType.AddRange( selectContact.DesiredFields );
            dialog.SelectionMode = selectContact.DesiredFields.Any() ? ContactSelectionMode.Fields : ContactSelectionMode.Contacts;

            if ( commitButton != null )
                dialog.CommitButtonText = commitButton.Name;

            if ( selectContact.Multiselect )
                return await dialog.PickContactsAsync();

            var contacts = new List<Contact>();
            var contact = await dialog.PickContactAsync();

            if ( contact != null )
                contacts.Add( contact );

            return contacts;
        }

        private static void InvokeCallbackCommand( SelectContactInteraction selectContact, IList<Contact> contacts )
        {
            Contract.Requires( selectContact != null );
            Contract.Requires( contacts != null );

            if ( contacts.Any() )
            {
                // set contacts and execute accept
                selectContact.Contacts.ReplaceAll( contacts );
                selectContact.ExecuteDefaultCommand();
            }
            else
            {
                // execute cancel
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
                return;

            var contacts = await SelectContactsAsync( selectContact );
            InvokeCallbackCommand( selectContact, contacts );
        }
    }
}
