namespace More.Windows.Media
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Represents XML media content.
    /// </summary>
    public class XmlContent : MediaContent<XDocument>
    {
        static XDocument Load( Stream stream )
        {
            Contract.Requires( stream != null );
            Contract.Ensures( Contract.Result<XDocument>() != null );

            using ( var reader = XmlReader.Create( stream ) )
            {
                return XDocument.Load( reader );
            }
        }

        /// <summary>
        /// Occurs when the stream is read for reading.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing the XML content to be read.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <see cref="XDocument"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        protected override Task<XDocument> OnReadStreamAsync( Stream stream ) => Task.Run( () => Load( stream ) );
    }
}