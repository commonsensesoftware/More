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
                Contract.Ensures( this.indentSize >= 0 );
                return this.indentSize;
            }
            [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for use if indent size needs to be changed." )]
            set
            {
                Contract.Requires( value >= 0 );
                this.indentSize = value;
            }
        }

        internal int IndentLevel
        {
            get
            {
                Contract.Ensures( this.indentLevel >= 0 );
                return this.indentLevel;
            }
        }

        internal void Indent()
        {
            Contract.Ensures( this.IndentLevel == Contract.OldValue( this.IndentLevel ) + 1 );
            this.indentText = new string( Space, ++this.indentLevel * this.IndentSize );
        }

        internal void Unindent()
        {
            Contract.Ensures( this.IndentLevel == 0 || this.IndentLevel == Contract.OldValue( this.IndentLevel ) - 1 );
            this.indentText = new string( Space, Math.Max( 0, --this.indentLevel ) * this.IndentSize );
        }

        public override void Close()
        {
            this.innerWriter.Close();
        }

        protected override void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;
            base.Dispose( disposing );

            if ( !disposing )
                return;

            this.innerWriter.Dispose();
        }

        public override Encoding Encoding
        {
            get
            {
                return this.innerWriter.Encoding;
            }
        }

        public override void Flush()
        {
            this.innerWriter.Flush();
        }

        public override Task FlushAsync()
        {
            return this.innerWriter.FlushAsync();
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return this.innerWriter.FormatProvider;
            }
        }

        public override string NewLine
        {
            get
            {
                return this.innerWriter.NewLine;
            }
            set
            {
                this.innerWriter.NewLine = value;
            }
        }

        public override void Write( char value )
        {
            this.innerWriter.Write( value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by underlying TextWriter." )]
        public override void Write( char[] buffer )
        {
            this.innerWriter.Write( buffer );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by underlying TextWriter." )]
        public override void Write( char[] buffer, int index, int count )
        {
            this.innerWriter.Write( buffer, index, count );
        }

        public override void Write( string value )
        {
            this.innerWriter.Write( this.indentText );
            this.innerWriter.Write( value );
        }

        public override void WriteLine( string value )
        {
            this.innerWriter.Write( this.indentText );
            this.innerWriter.WriteLine( value );
        }

        public override void WriteLine()
        {
            this.innerWriter.WriteLine();
        }
    }
}