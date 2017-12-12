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
        static readonly DependencyPropertyKey ScoreBrushPropertyKey =
            DependencyProperty.RegisterReadOnly( nameof( ScoreBrush ), typeof( Brush ), typeof( TrendIndicator ), new PropertyMetadata( DefaultScoreBrush ) );

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
            get => (Brush) GetValue( ScoreBrushProperty ) ?? DefaultScoreBrush;
            protected set => SetValue( ScoreBrushPropertyKey, value );
        }

        /// <summary>
        /// Indicates that the initialization process for the control is complete.
        /// </summary>
        public override void EndInit()
        {
            if ( ReadLocalValue( TrendProperty ) != DependencyProperty.UnsetValue )
            {
                UpdateVisualState();
                OnTrendChanged( EventArgs.Empty );
            }

            if ( ReadLocalValue( ScoreProperty ) != DependencyProperty.UnsetValue )
            {
                OnScoreChanged( EventArgs.Empty );
            }
        }

        /// <summary>
        /// Overrides the default behavior when the control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState();
        }
    }
}