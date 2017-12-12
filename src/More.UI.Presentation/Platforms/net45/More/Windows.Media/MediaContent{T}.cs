namespace More.Windows.Media
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Windows;

    /// <content>
    /// Provides additional implementation for Windows Desktop applications.
    /// </content>
    public abstract partial class MediaContent<T>
    {
        /// <summary>
        /// Returns the media content from the specified resource asynchronously.
        /// </summary>
        /// <param name="resourceName">The relative name of the resource.  The resource specified should
        /// include the relative path after the <b>component</b> segment of the <b>pack://</b> URI.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        /// <remarks>The specified resource must exist in the current <see cref="Application">application</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "False positive. The overload is called using a constructed Uri object." )]
        public Task<T> FromResourceAsync( string resourceName )
        {
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            resourceName = resourceName.TrimStart( '/' );

            var assemblyName = System.Reflection.Assembly.GetCallingAssembly().GetName();
            var uri = new Uri( TypeExtensions.PackUriFormat.FormatInvariant( assemblyName.Name, resourceName ), UriKind.Relative );

            return FromResourceAsync( uri );
        }

        /// <summary>
        /// Returns the media content from the specified resource asynchronously.
        /// </summary>
        /// <param name="resourceUri">The <see cref="Uri"/> for the resource.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="T"/>.</returns>
        public virtual async Task<T> FromResourceAsync( Uri resourceUri )
        {
            Arg.NotNull( resourceUri, nameof( resourceUri ) );
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            var info = Application.GetResourceStream( resourceUri );

            if ( info == null )
            {
                throw new InvalidOperationException( ExceptionMessage.MissingResourceException.FormatDefault( resourceUri ) );
            }

            T content;

            using ( var stream = info.Stream )
            {
                content = await OnReadStreamAsync( stream ).ConfigureAwait( false );
            }

            return content;
        }
    }
}