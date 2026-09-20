using Microsoft.Xna.Framework;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Base de todo control de la interfaz. Sabe donde esta, si el raton esta
/// encima y si tiene el foco; no sabe nada de reglas de negocio ni del
/// servidor.
/// </summary>
public abstract class Control
{
    public Rectangle Limites;

    public bool Habilitado { get; set; } = true;

    public bool Visible { get; set; } = true;

    /// <summary>El raton esta sobre el control en el cuadro actual.</summary>
    public bool Encima { get; protected set; }

    /// <summary>El control recibe lo que se escriba en el teclado.</summary>
    public bool Enfocado { get; set; }

    /// <summary>
    /// Marca de error del control. Los flujos alternos FA-03, FA-04, FA-05 y
    /// FA-07 del CU-02 piden resaltar el campo rechazado y escribir a un lado
    /// que se espera de el. Aqui solo se guarda y se dibuja; quien la enciende
    /// es la validacion, que todavia no existe.
    /// </summary>
    public string? Aviso { get; set; }

    public bool TieneAviso => !string.IsNullOrEmpty(Aviso);

    public virtual void Actualizar(Entrada entrada)
    {
        Encima = Habilitado && Visible && Limites.Contains(entrada.Raton);
    }

    public abstract void Dibujar(Lienzo lienzo);
}
