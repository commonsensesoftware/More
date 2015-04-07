namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.Specialized;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IContentManager ) )]
    internal abstract class IContentManagerContract : IContentManager
    {
        IEnumerable<object> IContentManager.Content
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<object>>() != null );
                return default( IEnumerable<object> );
            }
        }

        void IContentManager.SetContent( object content )
        {
            Contract.Requires<ArgumentNullException>( content != null, "content" );
        }

        void IContentManager.AddToContent( object content )
        {
            Contract.Requires<ArgumentNullException>( content != null, "content" );
        }

        void IContentManager.RemoveFromContent( object content )
        {
            Contract.Requires<ArgumentNullException>( content != null, "content" );
        }

        void IContentManager.ClearContent()
        {
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
            }
            remove
            {
            }
        }
    }
}
