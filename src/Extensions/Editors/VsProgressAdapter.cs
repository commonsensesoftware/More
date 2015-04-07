namespace More.VisualStudio.Editors
{
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;

    internal sealed class VsProgressAdapter : IProgress<GeneratorProgress>
    {
        private sealed class NullGeneratorProgress : IVsGeneratorProgress
        {
            internal static readonly NullGeneratorProgress Instance = new NullGeneratorProgress();

            private NullGeneratorProgress()
            {
            }

            public int GeneratorError( int fWarning, uint dwLevel, string bstrError, uint dwLine, uint dwColumn )
            {
                return 0;
            }

            public int Progress( uint nComplete, uint nTotal )
            {
                return 0;
            }
        }

        private const uint MinusOne = 0xFFFFFFFF;
        private readonly IVsGeneratorProgress progress;

        internal VsProgressAdapter( IVsGeneratorProgress progress )
        {
            this.progress = progress ?? NullGeneratorProgress.Instance;
        }

        public void Report( GeneratorProgress value )
        {
            if ( value == null )
                return;

            var error = value.Error;
            var hr = 0;

            if ( error == null )
            {
                hr = this.progress.Progress( (uint) value.Completed, (uint) value.Total );
            }
            else
            {
                var warning = Convert.ToInt32( error.IsWarning );
                var line = error.Line == null ? MinusOne : (uint) error.Line.Value;
                var column = line == MinusOne ? MinusOne : ( error.Column == null ? MinusOne : (uint) error.Column.Value );
                hr = this.progress.GeneratorError( warning, 0U, error.Message, line, column );
            }

            Marshal.ThrowExceptionForHR( hr );
        }
    }
}
