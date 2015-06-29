namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Saves and retrieves the properties of a storage item.
    /// </summary>
    [ContractClass( typeof( IStorageItemPropertiesContract ) )]
    public interface IStorageItemProperties
    {
        /// <summary>
        /// Retrieves the properties with the specified names.
        /// </summary>
        /// <param name="propertiesToRetrieve">A <see cref="IEnumerable{T}">sequence</see> of properties names to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a <see cref="IDictionary{TKey,TValue}">collection</see>
        /// of the retrieved properties.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        Task<IDictionary<string, object>> RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve );

        /// <summary>
        /// Saves all properties associated with the item.
        /// </summary>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task SavePropertiesAsync();

        /// <summary>
        /// Saves the specified properties associated with the item.
        /// </summary>
        /// <param name="propertiesToSave">A <see cref="IDictionary{TKey,TValue}">collection</see> of the properties
        /// and their values to save.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        Task SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave );
    }
}
