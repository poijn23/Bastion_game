using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Presentation.Utils;

/// <summary>
/// Estado del raton y del teclado en el cuadro actual, ya resuelto en forma de
/// "que paso" y no de "que esta oprimido": los controles preguntan si hubo
/// click, no comparan estados entre cuadros cada uno por su cuenta.
/// </summary>
public sealed class Entrada
{
    private MouseState _anterior;
    private MouseState _actual;
    private KeyboardState _tecladoAnterior;
    private KeyboardState _tecladoActual;

    private readonly List<char> _caracteres = new();

    /// <summary>Segundos transcurridos desde que arranco el juego, para las animaciones simples.</summary>
    public float Segundos { get; private set; }

    public Point Raton => _actual.Position;

    /// <summary>El boton izquierdo se solto en este cuadro.</summary>
    public bool Click => _anterior.LeftButton == ButtonState.Pressed &&
                         _actual.LeftButton == ButtonState.Released;

    public bool BotonOprimido => _actual.LeftButton == ButtonState.Pressed;

    /// <summary>Caracteres que el sistema entrego en este cuadro, ya con el teclado del usuario.</summary>
    public IReadOnlyList<char> Caracteres => _caracteres;

    public void Actualizar(GameTime tiempo)
    {
        Segundos = (float)tiempo.TotalGameTime.TotalSeconds;
        _anterior = _actual;
        _actual = Mouse.GetState();
        _tecladoAnterior = _tecladoActual;
        _tecladoActual = Keyboard.GetState();
    }

    /// <summary>Se llama al final del cuadro, cuando los controles ya leyeron lo escrito.</summary>
    public void LimpiarTexto() => _caracteres.Clear();

    /// <summary>Lo alimenta el evento TextInput de la ventana.</summary>
    public void AgregarCaracter(char caracter) => _caracteres.Add(caracter);

    public bool TeclaNueva(Keys tecla)
        => _tecladoActual.IsKeyDown(tecla) && _tecladoAnterior.IsKeyUp(tecla);

    public bool TeclaOprimida(Keys tecla) => _tecladoActual.IsKeyDown(tecla);
}
