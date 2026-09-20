using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation.Utils;

public sealed class TextRenderer
{
    private readonly SpriteBatch _batch;

    public TextRenderer(SpriteBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        _batch = batch;
    }

    public void Draw(string text, Vector2 position, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (style.Tracking == 0f)
        {
            DrawRun(text, position, style);
            return;
        }

        float x = position.X;

        foreach (char character in text)
        {
            string letter = character.ToString();
            DrawRun(letter, new Vector2(x, position.Y), style);
            x += (style.Font.MeasureString(letter).X * style.Scale) + style.Tracking;
        }
    }

    public void DrawCentered(string text, Rectangle area, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        float width = Measure(text, style);
        float x = area.X + ((area.Width - width) / 2f);
        float y = area.Y + ((area.Height - GetLineHeight(style)) / 2f);

        Draw(text, new Vector2(MathF.Round(x), MathF.Round(y)), style);
    }

    public float Measure(string text, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        if (string.IsNullOrEmpty(text))
        {
            return 0f;
        }

        if (style.Tracking == 0f)
        {
            return style.Font.MeasureString(text).X * style.Scale;
        }

        float width = 0f;

        foreach (char character in text)
        {
            width += (style.Font.MeasureString(character.ToString()).X * style.Scale) + style.Tracking;
        }

        return width - style.Tracking;
    }

    public float GetLineHeight(TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        return style.Font.LineSpacing * style.Scale;
    }

    private void DrawRun(string text, Vector2 position, TextStyle style)
    {
        _batch.DrawString(
            style.Font,
            text,
            position,
            style.Color,
            0f,
            Vector2.Zero,
            style.Scale,
            SpriteEffects.None,
            0f);
    }
}
