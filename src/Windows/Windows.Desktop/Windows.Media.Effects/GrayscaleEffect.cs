namespace More.Windows.Media.Effects
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    /// <summary>
    /// Represents a grayscale pixel shader effect.
    /// </summary>
    public class GrayscaleEffect : ShaderEffect
    {
        /// <summary>
        /// Gets the dependency property used for effect input.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty( "Input", typeof( GrayscaleEffect ), 0 );

        private static readonly PixelShader pixelShader = LoadShaderFromResource( "Windows.Media.Effects/GrayscaleEffect.ps" );

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleEffect"/> class.
        /// </summary>
        public GrayscaleEffect()
        {
            PixelShader = pixelShader;
            UpdateShaderValue( InputProperty );
        }

        /// <summary>
        /// Gets or sets the effect input.
        /// </summary>
        /// <value>A <see cref="Brush"/> object.</value>
        public Brush Input
        {
            get
            {
                return (Brush) GetValue( InputProperty );
            }
            set
            {
                SetValue( InputProperty, value );
            }
        }

        private static PixelShader LoadShaderFromResource( string resourceName )
        {
            Contract.Requires( resourceName != null, "resourceName" );
            Contract.Ensures( Contract.Result<PixelShader>() != null );

            var shader = new PixelShader();
            shader.UriSource = typeof( GrayscaleEffect ).CreatePackUri( resourceName );
            return shader;
        }
    }
}
