using System.Diagnostics.Contracts;
namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal class IndentingTextWriter : TextWriter
    {
        private const char Space = ' ';
        private readonly TextWriter innerWriter;
        private bool disposed;
        private int indentSize = 4;
        private int indentLevel;
        private string indentText = string.Empty;

        internal IndentingTextWriter( TextWriter innerWriter )
            : base( innerWriter.FormatProvider )
        {
            Contract.Requires( innerWriter != null );
            this.innerWriter = innerWriter;
        }

        internal int IndentSize
        {
            get
            {
                Contract.Ensures( indentSize >= 0 );
                return indentSize;
            }
            [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for use if indent size needs to be changed." )]
            set
            {
                Contract.Requires( value >= 0 );
                indentSize = value;
            }
        }

        internal int IndentLevel
        {
            get
            {
                Contract.Ensures( indentLevel >= 0 );
                return indentLevel;
            }
        }

        internal void Indent()
        {
            Contract.Ensures( IndentLevel == Contract.OldValue( IndentLevel ) + 1 );
            indentText = new string( Space, ++indentLevel * IndentSize );
        }

        internal void Unindent()
        {
            Contract.Ensures( IndentLevel == 0 || IndentLevel == Contract.OldValue( IndentLevel ) - 1 );
            indentText = new string( Space, Math.Max( 0, --indentLevel ) * IndentSize );
        }

        public override void Close()
        {
            innerWriter.Close();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;
            base.Dispose( disposing );

            if ( !disposing )
                return;

            innerWriter.Dispose();
        }

        public override Encoding Encoding
        {
            get
            {
                return innerWriter.Encoding;
            }
        }

        public override void Flush()
        {
            innerWriter.Flush();
        }

        public override Task FlushAsync()
        {
            return innerWriter.FlushAsync();
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return innerWriter.FormatProvider;
            }
        }

        public override string NewLine
        {
            get
            {
                return innerWriter.NewLine;
            }
            set
            {
                innerWriter.NewLine = value;
            }
        }

        public override void Write( char value )
        {
            innerWriter.Write( value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by underlying TextWriter." )]
        public override void Write( char[] buffer )
        {
            innerWriter.Write( buffer );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by underlying TextWriter." )]
        public override void Write( char[] buffer, int index, int count )
        {
            innerWriter.Write( buffer, index, count );
        }

        public override void Write( string value )
        {
            innerWriter.Write( indentText );
            innerWriter.Write( value );
        }

        public override void WriteLine( string value )
        {
            innerWriter.Write( indentText );
            innerWriter.WriteLine( value );
        }

        public override void WriteLine()
        {
            innerWriter.WriteLine();
        }
    }
}