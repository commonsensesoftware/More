namespace More.Windows.Media
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using global::Windows.Storage;

    /// <content>
    /// Provides additional implementation for Windows Runtime applications.
    /// </content>
    public abstract partial class MediaContent<T>
    {
        /// <summary>
        /// Returns the media content from the specified resource asynchronously.
        /// </summary>
        /// <param name="resourceName">The relative name of the resource.  The resource specified should
        /// include the relative path after the <b>component</b> segment of the <b>pack://</b> URI.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        /// <remarks>The specified resource must exist in the current <see cref="T:Application">application</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "False positive. The overload is called using a constructed Uri object." )]
        public Task<T> FromResourceAsync( string resourceName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( resourceName ), "resourceName" );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            var uri = new Uri( resourceName, UriKind.RelativeOrAbsolute );

            if ( !uri.IsAbsoluteUri)
                uri = new Uri( TypeExtensions.AppXFormat.FormatInvariant( resourceName), UriKind.Absolute );

            return this.FromResourceAsync( uri );
        }

        /// <summary>
        /// Returns the media content from the specified resource asynchronously.
        /// </summary>
        /// <param name="resourceUri">The <see cref="Uri"/> for the resource.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        /// <remarks>The specified resource is retrieved using the <see cref="M:StorageFile.GetFileFromApplicationUriAsync"/> method.</remarks>
        public virtual async Task<T> FromResourceAsync( Uri resourceUri )
        {
            Contract.Requires<ArgumentNullException>( resourceUri != null, "resourceUri" );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            var file = await StorageFile.GetFileFromApplicationUriAsync( resourceUri );

            using ( var inputStream = await file.OpenSequentialReadAsync() )
                return await this.OnReadStreamAsync( inputStream.AsStreamForRead() );
        }
    }
}
