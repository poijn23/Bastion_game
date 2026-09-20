using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Dibujo de texto con espaciado entre letras. El prototipo separa las letras
/// en los titulos y en las etiquetas en mayusculas, y <see cref="SpriteFont"/>
/// solo ofrece un espaciado por fuente, compartido por todo el que la use; aqui
/// se aplica por llamada, dibujando caracter por caracter cuando hace falta.
/// </summary>
public static class DibujoTexto
{
    public static void Dibujar(
        SpriteBatch lote,
        SpriteFont fuente,
        string texto,
        Vector2 posicion,
        Color color,
        float escala = 1f,
        float espaciado = 0f)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return;
        }

        if (espaciado == 0f)
        {
            lote.DrawString(fuente, texto, posicion, color, 0f, Vector2.Zero, escala, SpriteEffects.None, 0f);
            return;
        }

        float x = posicion.X;
        foreach (char caracter in texto)
        {
            string letra = caracter.ToString();
            lote.DrawString(fuente, letra, new Vector2(x, posicion.Y), color, 0f, Vector2.Zero, escala, SpriteEffects.None, 0f);
            x += fuente.MeasureString(letra).X * escala + espaciado;
        }
    }

    /// <summary>Ancho que ocupara el texto con esa escala y ese espaciado.</summary>
    public static float Ancho(SpriteFont fuente, string texto, float escala = 1f, float espaciado = 0f)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return 0f;
        }

        if (espaciado == 0f)
        {
            return fuente.MeasureString(texto).X * escala;
        }

        float ancho = 0f;
        foreach (char caracter in texto)
        {
            ancho += fuente.MeasureString(caracter.ToString()).X * escala + espaciado;
        }

        return ancho - espaciado;
    }

    public static float Alto(SpriteFont fuente, float escala = 1f)
        => fuente.LineSpacing * escala;

    /// <summary>Dibuja el texto centrado horizontalmente dentro de <paramref name="area"/>.</summary>
    public static void DibujarCentrado(
        SpriteBatch lote,
        SpriteFont fuente,
        string texto,
        Rectangle area,
        Color color,
        float escala = 1f,
        float espaciado = 0f)
    {
        float ancho = Ancho(fuente, texto, escala, espaciado);
        float x = area.X + (area.Width - ancho) / 2f;
        float y = area.Y + (area.Height - Alto(fuente, escala)) / 2f;
        Dibujar(lote, fuente, texto, new Vector2(MathF.Round(x), MathF.Round(y)), color, escala, espaciado);
    }
}
