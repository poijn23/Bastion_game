using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation.Utils;

public sealed class ShapeRenderer : IDisposable
{
    private readonly GraphicsDevice _device;
    private readonly SpriteBatch _batch;
    private readonly Dictionary<RoundedRectangleShape, Texture2D> _textures = new();

    public ShapeRenderer(GraphicsDevice device, SpriteBatch batch)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(batch);

        _device = device;
        _batch = batch;

        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }

    public Texture2D Pixel { get; }

    public void DrawRectangle(Rectangle bounds, Color color)
    {
        _batch.Draw(Pixel, bounds, color);
    }

    public void DrawRoundedRectangle(Rectangle bounds, int cornerRadius, Color color)
    {
        var shape = new RoundedRectangleShape
        {
            Width = bounds.Width,
            Height = bounds.Height,
            CornerRadius = cornerRadius
        };

        _batch.Draw(GetTexture(shape), new Vector2(bounds.X, bounds.Y), color);
    }

    public void DrawRoundedBorder(Rectangle bounds, BorderStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        var shape = new RoundedRectangleShape
        {
            Width = bounds.Width,
            Height = bounds.Height,
            CornerRadius = style.CornerRadius,
            BorderThickness = style.Thickness
        };

        _batch.Draw(GetTexture(shape), new Vector2(bounds.X, bounds.Y), style.Color);
    }

    public void Dispose()
    {
        foreach (Texture2D texture in _textures.Values)
        {
            texture.Dispose();
        }

        _textures.Clear();
        Pixel.Dispose();
    }

    private Texture2D GetTexture(RoundedRectangleShape shape)
    {
        if (_textures.TryGetValue(shape, out Texture2D? cached))
        {
            return cached;
        }

        var texture = new Texture2D(_device, shape.Width, shape.Height);
        texture.SetData(BuildPixels(shape));
        _textures[shape] = texture;

        return texture;
    }

    private static Color[] BuildPixels(RoundedRectangleShape shape)
    {
        Color[] pixels = new Color[shape.Width * shape.Height];

        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                float distance = GetSignedDistance(x, y, shape);
                pixels[(y * shape.Width) + x] = Color.White * GetCoverage(distance, shape.BorderThickness);
            }
        }

        return pixels;
    }

    // Signed distance to the rounded edge: negative inside, positive outside.
    // It is what lets the corner be smoothed instead of stepped.
    private static float GetSignedDistance(int x, int y, RoundedRectangleShape shape)
    {
        float halfWidth = shape.Width / 2f;
        float halfHeight = shape.Height / 2f;
        float radius = MathHelper.Clamp(shape.CornerRadius, 0, MathF.Min(halfWidth, halfHeight));
        float offsetX = MathF.Abs(x + 0.5f - halfWidth) - halfWidth + radius;
        float offsetY = MathF.Abs(y + 0.5f - halfHeight) - halfHeight + radius;
        float clampedX = MathF.Max(offsetX, 0);
        float clampedY = MathF.Max(offsetY, 0);
        float outside = MathF.Sqrt((clampedX * clampedX) + (clampedY * clampedY));

        return outside + MathF.Min(MathF.Max(offsetX, offsetY), 0) - radius;
    }

    private static float GetCoverage(float distance, int borderThickness)
    {
        float coverage = Math.Clamp(0.5f - distance, 0f, 1f);

        if (borderThickness <= 0)
        {
            return coverage;
        }

        return coverage - Math.Clamp(0.5f - (distance + borderThickness), 0f, 1f);
    }
}
