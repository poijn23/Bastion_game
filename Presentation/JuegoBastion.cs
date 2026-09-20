using Bastion.Presentation.GUI_Register;
using Bastion.Resources;
using Bastion.Presentation.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Presentation;

/// <summary>
/// Anfitrion de la ventana del cliente. Hoy abre directamente la GUI_Register
/// para poder verla; cuando exista el resto de las pantallas, aqui ira el
/// cambio entre ellas y el arranque sera la GUI_Login.
/// </summary>
public sealed class JuegoBastion : Game
{
    private readonly GraphicsDeviceManager _graficos;
    private readonly Entrada _entrada = new();

    private SpriteBatch _lote = null!;
    private Formas _formas = null!;
    private Lienzo _lienzo = null!;
    private GuiRegister _pantalla = null!;

    public JuegoBastion()
    {
        _graficos = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Tema.AnchoVentana,
            PreferredBackBufferHeight = Tema.AltoVentana,
            SynchronizeWithVerticalRetrace = true
        };

        // El idioma se fija antes de pedir el primer texto del catalogo. Sin
        // esto la interfaz arrancaria en el idioma del sistema operativo, que
        // puede no ser ninguno de los dos admitidos por la D-21.
        Idioma.Aplicar(Idioma.Predeterminado);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = Textos.TituloVentana;
        Window.AllowUserResizing = false;
    }

    protected override void Initialize()
    {
        // DesktopGL entrega aqui los caracteres ya resueltos por el sistema, con
        // acentos y distribucion de teclado incluidos. Leer las teclas crudas
        // obligaria a traducirlas a mano y romperia con otra distribucion.
        Window.TextInput += (_, argumentos) => _entrada.AgregarCaracter(argumentos.Character);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _lote = new SpriteBatch(GraphicsDevice);
        _formas = new Formas(GraphicsDevice);

        _lienzo = new Lienzo(
            _lote,
            _formas,
            Content.Load<SpriteFont>("Regular"),
            Content.Load<SpriteFont>("Bold"),
            Content.Load<SpriteFont>("Title"));

        _pantalla = new GuiRegister();
    }

    protected override void Update(GameTime tiempo)
    {
        _entrada.Actualizar(tiempo);
        _pantalla.Actualizar(_entrada);
        _entrada.LimpiarTexto();

        base.Update(tiempo);
    }

    protected override void Draw(GameTime tiempo)
    {
        GraphicsDevice.Clear(Tema.Fondo);

        _lote.Begin(samplerState: SamplerState.LinearClamp);
        _pantalla.Dibujar(_lienzo);
        _lote.End();

        base.Draw(tiempo);
    }

    protected override void UnloadContent()
    {
        _formas.Dispose();
        _lote.Dispose();
        base.UnloadContent();
    }
}
