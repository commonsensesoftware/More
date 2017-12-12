namespace More.VisualStudio.Templates
{
    using System;

    class ReplacementBinder<T>
    {
        readonly Action<T, string> assignTo;
        readonly Func<T, string> assignFrom;

        internal ReplacementBinder( string key, Action<T, string> assignTo, Func<T, string> assignFrom )
        {
            if ( ( this.assignTo = assignTo ) == null )
            {
                this.assignTo = ( m, v ) => { };
                IsWriteOnly = true;
            }

            if ( ( this.assignFrom = assignFrom ) == null )
            {
                if ( IsWriteOnly )
                {
                    throw new InvalidOperationException( "The property binder must be read, write, or read/write." );
                }
                else
                {
                    this.assignFrom = m => default( string );
                    IsReadOnly = true;
                }
            }

            Key = key;
        }

        internal string Key { get; }

        internal void AssignTo( T model, string value ) => assignTo( model, value );

        internal string AssignFrom( T model ) => assignFrom( model );

        internal bool IsReadOnly { get; }

        internal bool IsWriteOnly { get; }
    }
}