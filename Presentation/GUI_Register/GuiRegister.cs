using Bastion.Presentation.Utils;
using Bastion.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Presentation.GUI_Register;

/// <summary>
/// GUI_Register: la pantalla del paso 1 del flujo normal del CU-02 "Registrar
/// cuenta". Pide el <c>nickname</c>, el <c>correo</c>, la contrasena y su
/// confirmacion, la <c>fecha_nacimiento</c> y el <c>idioma_preferido</c>, junto
/// con la casilla de aceptacion de los terminos de uso, las opciones "Crear
/// cuenta" y "Cancelar" y el selector de idioma de la interfaz.
///
/// Es solo la presentacion. No valida nada, no abre la conexion, no arma el
/// REGISTER_REQUEST y no navega a ninguna otra pantalla: los pasos 3 al 27 del
/// flujo normal, los flujos alternos y las excepciones quedan pendientes. Los
/// controles guardan lo capturado y se dibujan; nada mas.
/// </summary>
public sealed class GuiRegister
{
    // --- Reticula ------------------------------------------------------------
    private const int AnchoTarjeta = 760;
    private const int TarjetaX = (Tema.AnchoVentana - AnchoTarjeta) / 2;
    private const int TarjetaY = 144;
    private const int AltoTarjeta = 378;

    private const int ContenidoX = TarjetaX + Tema.RellenoTarjeta;
    private const int ContenidoAncho = AnchoTarjeta - (Tema.RellenoTarjeta * 2);
    private const int Canal = 32;
    private const int ColumnaAncho = (ContenidoAncho - Canal) / 2;
    private const int ColumnaDerechaX = ContenidoX + ColumnaAncho + Canal;

    // La separacion entre filas deja sitio para la linea de aviso que el FA-03
    // y el FA-07 piden escribir junto al campo rechazado; con menos, ese texto
    // se encima con la etiqueta de la fila siguiente.
    // Orden de las opciones en los dos selectores de idioma.
    private const int IndiceEspanol = 0;
    private const int IndiceIngles = 1;

    private const int DesplegableX = 24;
    private const int DesplegableAncho = 210;
    private const int DesplegableAlto = 40;

    private const int SeparacionFilas = 98;
    private const int Fila1 = TarjetaY + Tema.RellenoTarjeta + 22;
    private const int Fila2 = Fila1 + SeparacionFilas;
    private const int Fila3 = Fila2 + SeparacionFilas;
    private const int FilaCasilla = Fila3 + Tema.AltoCampo + 28;

    /// <summary>Cruces tenues del fondo, tomadas del prototipo.</summary>
    private static readonly Point[] Adornos =
    [
        new(96, 108), new(1122, 88), new(72, 372), new(1180, 420),
        new(152, 516), new(1060, 664), new(620, 40)
    ];

    private readonly List<Control> _controles = [];
    private readonly List<CampoTexto> _ordenDeFoco = [];

    private readonly CampoTexto _campoNickname;
    private readonly CampoTexto _campoCorreo;
    private readonly CampoTexto _campoContrasena;
    private readonly CampoTexto _campoConfirmacion;
    private readonly CampoTexto _campoDia;
    private readonly CampoTexto _campoMes;
    private readonly CampoTexto _campoAnio;
    private readonly Selector _idiomaCuenta;
    private readonly Desplegable _idiomaInterfaz;
    private readonly Casilla _casillaTerminos;
    private readonly Boton _botonCrear;
    private readonly Boton _botonCancelar;

    public GuiRegister()
    {
        _campoNickname = new CampoTexto
        {
            LongitudMaxima = 30,
            Limites = new Rectangle(ContenidoX, Fila1, ColumnaAncho, Tema.AltoCampo)
        };

        _campoCorreo = new CampoTexto
        {
            LongitudMaxima = 254,
            Limites = new Rectangle(ContenidoX, Fila2, ColumnaAncho, Tema.AltoCampo)
        };

        _campoContrasena = new CampoTexto
        {
            EsContrasena = true,
            Limites = new Rectangle(ColumnaDerechaX, Fila1, ColumnaAncho, Tema.AltoCampo)
        };

        _campoConfirmacion = new CampoTexto
        {
            EsContrasena = true,
            Limites = new Rectangle(ColumnaDerechaX, Fila2, ColumnaAncho, Tema.AltoCampo)
        };

        // La fecha se captura en tres cajas en lugar de una sola: es mas claro
        // para un jugador de ocho anos (CON-12) y no obliga a explicar ningun
        // formato de escritura.
        const int anchoDia = 92;
        const int anchoMes = 92;
        const int anchoAnio = ColumnaAncho - anchoDia - anchoMes - 24;

        _campoDia = new CampoTexto
        {
            LongitudMaxima = 2,
            Centrado = true,
            Limites = new Rectangle(ContenidoX, Fila3, anchoDia, Tema.AltoCampo)
        };

        _campoMes = new CampoTexto
        {
            LongitudMaxima = 2,
            Centrado = true,
            Limites = new Rectangle(ContenidoX + anchoDia + 12, Fila3, anchoMes, Tema.AltoCampo)
        };

        _campoAnio = new CampoTexto
        {
            LongitudMaxima = 4,
            Centrado = true,
            Limites = new Rectangle(ContenidoX + anchoDia + anchoMes + 24, Fila3, anchoAnio, Tema.AltoCampo)
        };

        _idiomaCuenta = new Selector
        {
            Opciones = [Textos.IdiomaEspanolMexico, Textos.IdiomaIngles],
            Seleccion = Idioma.EsIngles ? IndiceIngles : IndiceEspanol,
            Limites = new Rectangle(ColumnaDerechaX, Fila3, ColumnaAncho, Tema.AltoCampo)
        };

        _casillaTerminos = new Casilla
        {
            Limites = new Rectangle(ContenidoX, FilaCasilla, ContenidoAncho, 24)
        };

        _idiomaInterfaz = new Desplegable
        {
            Opciones = [Textos.IdiomaEspanolMexico, Textos.IdiomaIngles],
            Seleccion = Idioma.EsIngles ? IndiceIngles : IndiceEspanol,
            Limites = new Rectangle(
                DesplegableX,
                Tema.AltoVentana - DesplegableX - DesplegableAlto,
                DesplegableAncho,
                DesplegableAlto)
        };

        _botonCrear = new Boton
        {
            Estilo = EstiloBoton.Primario,
            ConFlecha = true,
            Limites = new Rectangle(TarjetaX, TarjetaY + AltoTarjeta + 22, AnchoTarjeta, Tema.AltoBotonPrimario)
        };

        _botonCancelar = new Boton
        {
            Estilo = EstiloBoton.Secundario,
            Limites = new Rectangle(
                TarjetaX,
                TarjetaY + AltoTarjeta + 22 + Tema.AltoBotonPrimario + 12,
                AnchoTarjeta,
                Tema.AltoBotonSecundario)
        };

        // Pendiente: el paso 2 del flujo normal y el FA-01. Hoy no hacen nada,
        // porque no hay validacion, ni conexion, ni GUI_MessageConfirm.
        _botonCrear.Pulsado += () => { };
        _botonCancelar.Pulsado += () => { };

        // FA-02, parcial: cambia el catalogo de recursos y vuelve a dibujar.
        _idiomaInterfaz.Cambiado += AlCambiarIdiomaDeInterfaz;

        _ordenDeFoco.AddRange([
            _campoNickname, _campoContrasena, _campoCorreo, _campoConfirmacion,
            _campoDia, _campoMes, _campoAnio
        ]);

        _controles.AddRange([
            _campoNickname, _campoCorreo, _campoContrasena, _campoConfirmacion,
            _campoDia, _campoMes, _campoAnio,
            _idiomaCuenta, _casillaTerminos,
            _botonCrear, _botonCancelar,
            _idiomaInterfaz
        ]);

        AplicarTextos();
        _campoNickname.Enfocado = true;
    }

    /// <summary>
    /// Toma del catalogo de la cultura vigente todos los textos visibles y los
    /// reparte entre los controles. Se llama al construir la pantalla y cada
    /// vez que se cambia de idioma; ningun control guarda una cadena escrita
    /// dentro del codigo (RN-14).
    /// </summary>
    private void AplicarTextos()
    {
        _campoNickname.Etiqueta = Textos.RegistroEtiquetaNickname;
        _campoNickname.Marcador = Textos.RegistroMarcadorNickname;

        _campoCorreo.Etiqueta = Textos.RegistroEtiquetaCorreo;
        _campoCorreo.Marcador = Textos.RegistroMarcadorCorreo;

        _campoContrasena.Etiqueta = Textos.RegistroEtiquetaContrasena;
        _campoContrasena.Marcador = Textos.RegistroMarcadorContrasena;

        _campoConfirmacion.Etiqueta = Textos.RegistroEtiquetaConfirmacion;
        _campoConfirmacion.Marcador = Textos.RegistroMarcadorConfirmacion;

        _campoDia.Etiqueta = Textos.RegistroEtiquetaFechaNacimiento;
        _campoDia.Marcador = Textos.RegistroMarcadorDia;
        _campoMes.Marcador = Textos.RegistroMarcadorMes;
        _campoAnio.Marcador = Textos.RegistroMarcadorAnio;

        _idiomaCuenta.Etiqueta = Textos.RegistroEtiquetaIdiomaCuenta;
        _idiomaCuenta.Opciones = [Textos.IdiomaEspanolMexico, Textos.IdiomaIngles];
        _idiomaInterfaz.Opciones = [Textos.IdiomaEspanolMexico, Textos.IdiomaIngles];

        _casillaTerminos.Texto = Textos.RegistroCasillaTerminos;
        _casillaTerminos.TextoEnlace = Textos.RegistroEnlaceTerminos;

        _botonCrear.Titulo = Textos.RegistroBotonCrear;
        _botonCrear.Subtitulo = Textos.RegistroBotonCrearDetalle;
        _botonCancelar.Titulo = Textos.RegistroBotonCancelar;
    }

    /// <summary>
    /// Responde al desplegable de idioma de la esquina. Recibe el indice de la
    /// opcion elegida: <see cref="IndiceEspanol"/> o <see cref="IndiceIngles"/>.
    ///
    /// Cambia la cultura vigente y vuelve a repartir los textos del catalogo
    /// entre los controles. Lo que el Jugador ya habia capturado se conserva,
    /// porque aqui solo se tocan las etiquetas y los textos guia, nunca el
    /// contenido de los campos (FA-02 pasos 1 y 2).
    ///
    /// El encabezado no aparece en esta lista: se lee del catalogo en cada
    /// cuadro, al dibujarse, asi que se actualiza solo.
    ///
    /// Faltan los otros dos pasos del FA-02, que ya no son refrescar la
    /// pantalla sino reglas del flujo: descartar las dos contrasenas, desmarcar
    /// la casilla de terminos porque el texto aceptado deja de ser el que se
    /// esta viendo, y proponer este idioma como idioma_preferido de la cuenta.
    /// </summary>
    private void AlCambiarIdiomaDeInterfaz(int indice)
    {
        Idioma.Aplicar(indice == IndiceIngles ? Idioma.Ingles : Idioma.EspanolMexico);
        AplicarTextos();
    }

    public void Actualizar(Entrada entrada)
    {
        CampoTexto.EntradaSegundos = entrada.Segundos;

        if (entrada.Click)
        {
            ResolverFoco(entrada);
        }

        if (entrada.TeclaNueva(Keys.Tab))
        {
            AvanzarFoco(entrada.TeclaOprimida(Keys.LeftShift) || entrada.TeclaOprimida(Keys.RightShift));
        }

        foreach (var control in _controles)
        {
            control.Actualizar(entrada);
        }
    }

    public void Dibujar(Lienzo lienzo)
    {
        var lote = lienzo.Lote;

        DibujarFondo(lienzo);
        DibujarEncabezado(lienzo);

        lienzo.Formas.RectanguloRedondo(
            lote,
            new Rectangle(TarjetaX, TarjetaY, AnchoTarjeta, AltoTarjeta),
            Tema.RadioTarjeta,
            Tema.Tarjeta);

        foreach (var control in _controles)
        {
            control.Dibujar(lienzo);
        }
    }

    private static void DibujarFondo(Lienzo lienzo)
    {
        foreach (var adorno in Adornos)
        {
            lienzo.Formas.Rectangulo(lienzo.Lote, new Rectangle(adorno.X - 7, adorno.Y - 1, 15, 2), Tema.FondoAdorno);
            lienzo.Formas.Rectangulo(lienzo.Lote, new Rectangle(adorno.X - 1, adorno.Y - 7, 2, 15), Tema.FondoAdorno);
        }
    }

    private static void DibujarEncabezado(Lienzo lienzo)
    {
        var lote = lienzo.Lote;

        string titulo = Textos.TituloJuego;
        float anchoTitulo = DibujoTexto.Ancho(lienzo.Titulo, titulo, Tema.EscalaTitulo, Tema.EspaciadoTitulo);
        DibujoTexto.Dibujar(
            lote,
            lienzo.Titulo,
            titulo,
            new Vector2(MathF.Round((Tema.AnchoVentana - anchoTitulo) / 2f), 52),
            Tema.TextoClaro,
            Tema.EscalaTitulo,
            Tema.EspaciadoTitulo);

        string subtitulo = Textos.RegistroSubtitulo;
        float anchoSubtitulo = DibujoTexto.Ancho(lienzo.Negrita, subtitulo, Tema.EscalaEtiqueta, 4f);
        DibujoTexto.Dibujar(
            lote,
            lienzo.Negrita,
            subtitulo,
            new Vector2(MathF.Round((Tema.AnchoVentana - anchoSubtitulo) / 2f), 112),
            Tema.TextoTenue,
            Tema.EscalaEtiqueta,
            4f);
    }

    /// <summary>Da el foco al campo donde se hizo click, o lo quita si fue fuera de todos.</summary>
    private void ResolverFoco(Entrada entrada)
    {
        foreach (var campo in _ordenDeFoco)
        {
            campo.Enfocado = campo.Limites.Contains(entrada.Raton);
        }
    }

    private void AvanzarFoco(bool haciaAtras)
    {
        int actual = _ordenDeFoco.FindIndex(campo => campo.Enfocado);
        int siguiente = actual < 0
            ? 0
            : (actual + (haciaAtras ? -1 : 1) + _ordenDeFoco.Count) % _ordenDeFoco.Count;

        for (int i = 0; i < _ordenDeFoco.Count; i++)
        {
            _ordenDeFoco[i].Enfocado = i == siguiente;
        }
    }
}
