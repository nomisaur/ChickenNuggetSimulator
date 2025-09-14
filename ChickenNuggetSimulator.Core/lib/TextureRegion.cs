using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Represents a rectangular region within a texture.
/// </summary>
public class TextureRegion
{
    /// <summary>
    /// Gets or Sets the source texture this texture region is part of.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Gets or Sets the source rectangle boundary of this texture region within the source texture.
    /// </summary>
    public Rectangle SourceRectangle { get; set; }

    /// <summary>
    /// Gets the width, in pixels, of this texture region.
    /// </summary>
    public int Width => SourceRectangle.Width;

    /// <summary>
    /// Gets the height, in pixels, of this texture region.
    /// </summary>
    public int Height => SourceRectangle.Height;
}
