namespace More.VisualStudio.Editors
{
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using static System.Runtime.InteropServices.Marshal;

    sealed class VsProgressAdapter : IProgress<GeneratorProgress>
    {
        const uint MinusOne = 0xFFFFFFFF;
        readonly IVsGeneratorProgress progress;

        internal VsProgressAdapter( IVsGeneratorProgress progress ) => this.progress = progress ?? NullGeneratorProgress.Instance;

        public void Report( GeneratorProgress value )
        {
            if ( value == null )
            {
                return;
            }

            var error = value.Error;
            var hr = 0;

            if ( error == null )
            {
                hr = progress.Progress( (uint) value.Completed, (uint) value.Total );
            }
            else
            {
                var warning = Convert.ToInt32( error.IsWarning );
                var line = error.Line == null ? MinusOne : (uint) error.Line.Value;
                var column = line == MinusOne ? MinusOne : ( error.Column == null ? MinusOne : (uint) error.Column.Value );
                hr = progress.GeneratorError( warning, 0U, error.Message, line, column );
            }

            ThrowExceptionForHR( hr );
        }

        sealed class NullGeneratorProgress : IVsGeneratorProgress
        {
            internal static readonly NullGeneratorProgress Instance = new NullGeneratorProgress();

            NullGeneratorProgress() { }

            public int GeneratorError( int fWarning, uint dwLevel, string bstrError, uint dwLine, uint dwColumn ) => 0;

            public int Progress( uint nComplete, uint nTotal ) => 0;
        }
    }
}