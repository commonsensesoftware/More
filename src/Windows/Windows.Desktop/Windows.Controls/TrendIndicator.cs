namespace More.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <content>
    /// Provides full CLR-specific implementation of the control.
    /// </content>
    public partial class TrendIndicator
    {
        private static readonly DependencyPropertyKey ScoreBrushPropertyKey =
            DependencyProperty.RegisterReadOnly( "ScoreBrush", typeof( Brush ), typeof( TrendIndicator ), new PropertyMetadata( DefaultScoreBrush ) );

        /// <summary>
        /// Gets the score brush dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ScoreBrushProperty = ScoreBrushPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the the score brush color.
        /// </summary>
        /// <value>A <see cref="Brush"/> object.</value>
        public Brush ScoreBrush
        {
            get
            {
                var result = (Brush) this.GetValue( ScoreBrushProperty ) ?? DefaultScoreBrush;
                return result;
            }
            protected set
            {
                this.SetValue( ScoreBrushPropertyKey, value );
            }
        }

        /// <summary>
        /// Indicates that the initialization process for the control is complete. 
        /// </summary>
        public override void EndInit()
        {
            // if the Trend property is set, trigger a property change after initialization
            if ( this.ReadLocalValue( TrendProperty ) != DependencyProperty.UnsetValue )
            {
                this.UpdateVisualState();
                this.OnTrendChanged( EventArgs.Empty );
            }

            // if the Score property is set, trigger a property change after initialization
            if ( this.ReadLocalValue( ScoreProperty ) != DependencyProperty.UnsetValue )
                this.OnScoreChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Overrides the default behavior when the control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualState();
        }
    }
}
