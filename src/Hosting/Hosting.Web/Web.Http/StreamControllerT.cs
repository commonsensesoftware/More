namespace More.Web.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Represents the base implementation of a Web API controller which supports streams.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the controller.</typeparam>
    public abstract class StreamController<TKey> : ApiController
    {
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The stream must be disposed by the caller." )]
        private static Stream EnsureStreamCanSeek( Stream stream )
        {
            Contract.Requires( stream != null );
            Contract.Ensures( Contract.Result<Stream>() != null );
            Contract.Ensures( Contract.Result<Stream>().CanSeek );

            // stream is seekable
            if ( stream.CanSeek )
                return stream;

            // stream is not seekable, so copy it into a memory stream so we can seek on it
            var copy = new MemoryStream();

            stream.CopyTo( copy );
            stream.Dispose();
            copy.Flush();
            copy.Seek( 0L, SeekOrigin.Begin );

            return copy;
        }

        /// <summary>
        /// Gets the content type for the stream with the specified key asynchronously.
        /// </summary>
        /// <param name="id">The key of the stream to get name and MIME type for.</param>
        /// <returns>The <see cref="Task{T}">task</see> containing the <see cref="MediaTypeHeaderValue">content type</see> for the requested stream.</returns>
        protected virtual Task<MediaTypeHeaderValue> GetContentTypeAsync( TKey id )
        {
            Arg.NotNull( id, nameof( id ) );
            Contract.Ensures( Contract.Result<Task<MediaTypeHeaderValue>>() != null );

            var contentType = new MediaTypeHeaderValue( MediaTypeNames.Application.Octet );

            if ( id != null )
            {
                var name = id.ToString();

                if ( string.IsNullOrEmpty( name ) )
                    contentType.Parameters.Add( new NameValueHeaderValue( "name", name ) );
            }

            return Task.FromResult( contentType );
        }

        /// <summary>
        /// Reads a stream with the specified key asynchronously.
        /// </summary>
        /// <param name="id">The key of the stream to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="Stream">stream</see>.</returns>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose of response provided to the exception." )]
        protected virtual Task<Stream> ReadStreamAsync( TKey id )
        {
            throw new HttpResponseException( Request.CreateResponse( HttpStatusCode.NotImplemented ) );
        }

        /// <summary>
        /// Writes a stream with the specified key asynchronously.
        /// </summary>
        /// <param name="id">The key of the stream to write.</param>
        /// <param name="stream">The <see cref="Stream">stream</see> to write.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose of response provided to the exception." )]
        protected virtual Task WriteStreamAsync( TKey id, Stream stream )
        {
            throw new HttpResponseException( Request.CreateResponse( HttpStatusCode.NotImplemented ) );
        }

        /// <summary>
        /// Gets a stream with the specified key.
        /// </summary>
        /// <param name="id">The key of the stream to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="HttpResponseMessage">response</see> for the request.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "By design. Maps to the HTTP GET verb." )]
        public async virtual Task<HttpResponseMessage> Get( TKey id )
        {
            var stream = await ReadStreamAsync( id );

            if ( stream == null )
                return Request.CreateResponse( HttpStatusCode.NotFound );

            var contentType = await GetContentTypeAsync( id );

            // get the range and stream media type
            var range = Request.Headers.Range;
            HttpResponseMessage response;

            if ( range == null )
            {
                // if the range header is present but null, then the header value must be invalid
                if ( Request.Headers.Contains( "Range" ) )
                    throw new HttpResponseException( Request.CreateResponse( HttpStatusCode.RequestedRangeNotSatisfiable ) );

                // if no range was requested, return the entire stream
                response = Request.CreateResponse( HttpStatusCode.OK );

                response.Headers.AcceptRanges.Add( "bytes" );
                response.Content = new StreamContent( stream );
                response.Content.Headers.ContentType = contentType;

                return response;
            }

            var partialStream = EnsureStreamCanSeek( stream );

            response = Request.CreateResponse( HttpStatusCode.PartialContent );
            response.Headers.AcceptRanges.Add( "bytes" );

            try
            {
                // return the requested range(s)
                response.Content = new ByteRangeStreamContent( partialStream, range, contentType );
            }
            catch ( InvalidByteRangeException ex )
            {
                response.Dispose();
                return Request.CreateErrorResponse( ex );
            }

            // change status code if the entire stream was requested
            if ( response.Content.Headers.ContentLength.Value == partialStream.Length )
                response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        private async Task<HttpResponseMessage> WriteStreamWithStatusCodeAsync( TKey id, HttpStatusCode statusCode )
        {
            Contract.Ensures( Contract.Result<Task<HttpResponseMessage>>() != null );

            using ( var stream = await Request.Content.ReadAsStreamAsync() )
            {
                await WriteStreamAsync( id, stream );
            }

            var response = Request.CreateResponse( statusCode );
            return response;
        }

        /// <summary>
        /// Inserts a new stream with the specified key.
        /// </summary>
        /// <param name="id">The key of the stream to insert.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="HttpResponseMessage">response</see> for the request.</returns>
        public virtual Task<HttpResponseMessage> Post( TKey id )
        {
            return WriteStreamWithStatusCodeAsync( id, HttpStatusCode.Created );
        }

        /// <summary>
        /// Updates a stream with the specified key.
        /// </summary>
        /// <param name="id">The key of the stream to update.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="HttpResponseMessage">response</see> for the request.</returns>
        public virtual Task<HttpResponseMessage> Put( TKey id )
        {
            return WriteStreamWithStatusCodeAsync( id, HttpStatusCode.OK );
        }

        /// <summary>
        /// Deletes a stream with the specified key.
        /// </summary>
        /// <param name="id">The key of the stream to delete.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose of response provided to the exception." )]
        public virtual Task Delete( TKey id )
        {
            throw new HttpResponseException( Request.CreateResponse( HttpStatusCode.NotImplemented ) );
        }
    }
}
