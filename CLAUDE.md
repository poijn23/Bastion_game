# BASTION: Muros y Caminos

Contexto del proyecto para quien trabaje en este repositorio, personas y Claude Code. Se armó el 2026-09-21 con los documentos del equipo y el código de `main`. Si algo de aquí contradice a un documento vigente, manda el documento.

## Qué es

Juego de mesa digital 2D por turnos, inspirado en Quoridor, para dos o cuatro jugadores en línea y público desde los 8 años. Cada jugador mueve su peón hasta el lado contrario o coloca un muro; un muro nunca puede dejar a alguien sin camino. Modos: clásico 9×9, rápido 7×7 y cuatro jugadores. Además: IA, salas privadas, amigos, chat, ranking, tienda de objetos cosméticos, tutorial y moderación.

Idiomas: es-MX (base) y en, nada más (D-21). Proyecto de la Universidad Veracruzana para Tecnología de la Construcción y Diseño de Software. Autores: Ángel Gabriel Aguilar Hernández y José Eduardo Prior Hernández. "Bastion" es un nombre provisional (Q-07): marca, arte y textos deben poder sustituirse sin tocar la lógica.

## Arquitectura objetivo

Cliente MonoGame (este repo), TCP con protocolo propio (sin WebSockets), servidor de partidas y SQL Server. Solo el servidor toca la base de datos. Lo que fijan los documentos y debe respetar el código nuevo:

- **Servidor autoritativo** (DRV-01): el cliente propone y previsualiza, el servidor decide y guarda el estado vigente en memoria. La sesión de partida es distinta de la conexión TCP.
- **Un único motor de reglas** (DRV-05), parametrizable, determinista y ejecutable sin interfaz, compartido por cliente y servidor. Los parámetros (tamaño de tablero, número de muros) van fuera del código.
- **Presupuesto de 1 s por jugada**, mediana bajo 300 ms (ASR-01). Ninguna escritura a la base entra en el camino síncrono de una jugada.
- **Persistencia por tipo de dato** (DRV-04): cuentas y progreso se confirman antes de responder; las jugadas se escriben diferidas, con ventana máxima de 5 s.
- **Protección del menor** (DRV-06): el filtro de chat vive en el servidor y falla cerrado; el segundo factor va detrás de una interfaz; se guardan los datos mínimos.
- **Localización de extremo a extremo** (DRV-07): ningún texto visible en el código ni dentro de imágenes; Unicode en pantalla, protocolo y columnas.
- Puntos de variación permitidos, y solo esos: el oponente (IA), el extremo de red y el modo de despliegue (LAN), y marca, arte y recursos de terceros.

## Estructura del repo

- `Presentation/`: cliente MonoGame DesktopGL, `net10.0`, `WinExe`. Una pantalla por carpeta, `GUI_<Nombre>/Gui<Nombre>.cs`. `Utils/` guarda los controles y las clases base (`FormScreen`, `MessageScreen`, `Control`, `Theme`, `Canvas`). `Navigator` y `ScreenId` resuelven la navegación.
- `Resources/`: `TextCatalog.cs` (una propiedad por clave), `TextCatalog.resx` (inglés, neutro) y `TextCatalog.es-MX.resx`, siempre con las mismas claves. `Language.cs` aplica la cultura.
- `Contracts/`, `DataAccess/`, `Domain/`, `Service/`: vacías (`.gitkeep`). Todavía no hay motor de reglas, protocolo, servidor ni acceso a datos.

Patrones ya establecidos:

- Una pantalla de formulario extiende `FormScreen`, registra sus controles con `Register` o `RegisterField` y reasigna sus textos en `ApplyTexts()`, que corre al cambiar de idioma sin perder lo escrito.
- Todo texto visible sale de `TextCatalog`. Para agregar uno: propiedad en `TextCatalog.cs` y la misma clave en los dos `.resx`.
- Se navega solo con `INavigator` y `ScreenId`; una pantalla no conoce a otra. Un `ScreenId` nuevo se registra en `Navigator`.
- `TestAccount` y `TestProfile` son sustitutos temporales hasta que exista `Service`; se borran al conectar.
- Una pantalla que necesita distinguir un rol o resultado (anfitrión/invitado, victoria/derrota) lo recibe como el `string? argument` de `GoTo`, con constantes públicas en la propia clase (`GuiWaitingRoom.HostRole`, `GuiMatchEnd.VictoryOutcome`) para que quien navega no escriba el literal a mano. `Navigator.Build` necesita un caso especial por cada una, junto a los de `ResetPassword` y `RegistrationSuccess`.

## Estado de las GUI (actualizado 2026-09-22, rama `guis-faltantes`)

Los casos de uso nombran 55 GUI. `origin/main` tenía 34; `origin/cuenta-prototipos-v3` (de donde parte esta rama) tenía 35, con `GUI_Settings` cubriendo tanto `AccountSettings` como el idioma. Esta rama agrega las 20 que faltaban, así que hoy existen 54 de 55 carpetas — la única "faltante" es `GUI_AccountSettings` en sí, porque v3 ya la fusionó dentro de `GUI_Settings` (decisión de esa rama, no de esta).

- Todas las pantallas nuevas son solo capa de presentación: no hay motor de reglas, protocolo, servidor ni base de datos detrás. Varias simplifican lo que el caso de uso pide por eso: `GUI_Matchmaking` y `GUI_OpponentDisconnected` deberían dibujarse como overlay sobre el tablero, pero `Navigator` no tiene aún el concepto de una pantalla encima de otra fuera del mecanismo fijo de diálogo, así que son pantalla completa con una nota explicándolo en el comentario de cabecera.
- Estimación por la misma rúbrica (`tools/score_gui_implementation.py`): 79.8 % de promedio en las 54 existentes, 78.3 % sobre las 55 nombradas. Las 20 nuevas promedian 79.8 % entre sí, mismo nivel que las 34 heredadas de v3. Extremo a extremo sigue en 0 %.
- Las 54 pantallas existentes son alcanzables desde `Login` (vía el menú provisional `GUI_Menu`, que ya lista las 54); 0 quedan huérfanas.
- `dotnet build` da 0 errores y 0 advertencias con las 20 pantallas nuevas incluidas.
- Los scripts de medición están en `tools/` (`gui_signals.py` + `score_gui_implementation.py`, con su propio `README.md`).

## Compilar y ejecutar

Requiere el SDK de .NET 10. Verificado el 2026-09-21: compila con 0 errores y 0 advertencias (unos 35 s la primera vez, por la restauración de paquetes).

```
dotnet tool restore
dotnet build Presentation/Bastion.Presentation.csproj
dotnet run --project Presentation/Bastion.Presentation.csproj
```

`dotnet tool restore` instala `dotnet-mgcb`, que compila las fuentes de `Presentation/Content`. Regla del proyecto: DesktopGL, nunca WindowsDX; no activar `PublishTrimmed` ni `PublishAot`.

## Convenciones de código (Estándar de Codificación v2.1)

- Todo el código va en inglés: identificadores, comentarios, mensajes de excepción y bitácora, textos. Los comentarios explican el porqué, no el qué.
- `camelCase` en variables, `PascalCase` en métodos, propiedades y constantes, `_camelCase` en campos privados, `IInterface`, `TTipo` en genéricos. Booleanos con prefijo `is`, `has`, `can` o `should`. Colecciones en plural.
- Sangría de 4 espacios, llaves Allman, líneas de 120 caracteres como máximo, llaves en todo bloque, `default` en todo `switch`.
- Máximo tres niveles de anidamiento, complejidad ciclomática 10 y tres parámetros por método (con más, un DTO). Salida anticipada con cláusulas de guarda. DRY.
- Un tipo de nivel superior por archivo, con `namespace` de ámbito de archivo. Orden en la clase: constantes, campos estáticos, campos, constructores, propiedades, métodos. `using` fuera del namespace, `System` primero.
- Sin valores mágicos: constantes o enumeraciones. Lambdas solo de una línea; nunca en suscripciones que deban cancelarse. `class` para lo mutable y `record` para lo inmutable.
- Excepciones: nunca `Exception` genérica, `throw;` y no `throw ex;`, `ArgumentNullException.ThrowIfNull` en los puntos de entrada. Inyección de dependencias por constructor y contra interfaces.
- Bitácora con log4net y nunca `Console.WriteLine`. Documentación XML solo en clases públicas de la capa de servicios. `TODO` y `FIXME` con número de tarea, y ninguno al enviar a revisión.
- Pruebas: clase con prefijo `Test`, métodos `Metodo_Estado_Esperado`, bloques Arrange, Act y Assert separados por línea en blanco, una aserción por prueba.

El repo no trae `.editorconfig` ni `CodeMetricsConfig.txt`: hoy estas reglas se aplican a mano.

## Git

`main` está protegida en GitHub (`poijn23/Bastion_game`): se trabaja en una rama por tema, en `kebab-case` (por ejemplo `registro-validacion-i18n`), y se integra con un Pull Request. Los mensajes de commit van en español, sin acentos, en una frase que dice qué cambia y sin prefijos.

## Documentos de referencia

Viven fuera del repo, como PDF exportados de proyectos LaTeX del equipo. Los `.tex` que hay en algunas carpetas locales están desactualizados: manda el PDF. La herramienta de lectura de PDF puede rechazarlos como "protegidos" sin que lo estén; extraer el texto con `pdftotext -layout -enc UTF-8` de TeX Live.

- `casos-de-uso.pdf` (555 págs.): 49 casos de uso, 52 funcionalidades CRUD, decisiones D-01 a D-21, modelo de datos (47 entidades), scripts de SQL Server y datos de prueba.
- `Estandar v2.1.pdf`: el estándar de codificación de C#.
- `Prototipos de Interfaz Gráfica.pdf`: reglas del Quoridor y 7 pantallas prototipo. Los 125 prototipos que el código cita (5a, 9c, 12c...) no están en el repo.
- `documento.pdf` y `diccionario_internacionalizacion.xlsx`: entrega de internacionalización con 548 elementos.
- `equipo4-hito-1.pdf` (Diseño de Software): contexto, escenarios de calidad, ASR-01 a ASR-06 y DRV-01 a DRV-08, con los identificadores STK, CRN, CON, REQ, DEP, TEN y Q que citan los demás.

## Decisiones vigentes de los casos de uso

- D-01: una cuenta puede tener varias sesiones, pero una sola partida sin terminar (incluida la IA, D-11).
- D-06: el registro no pide nombre, apellidos ni región. Sí pide `fecha_nacimiento`, con edad mínima de 8 años.
- D-07 y D-17: la baja de cuenta es lógica, definitiva y anonimiza en la misma transacción.
- D-13: el nickname se elige en el registro; en la primera vez solo se cambian peón e icono.
- D-15: perder la conexión no es abandonar; el rival puede reclamar la victoria a los 60 s.
- D-21: es-MX es el idioma por omisión y el de respaldo cuando falta una clave. Los catálogos guardan códigos (`CLASICO`) y los diccionarios los traducen (`Modo.CLASICO`). Lo que escriben las personas no se traduce.
- Cuando un prototipo y su caso de uso no coinciden, manda el caso de uso.

## Desajustes conocidos entre los documentos y el código

- `GuiRegister` pide nombre y apellidos (contra D-06) y no aplica la edad mínima de 8 años ni la política de contraseña de 3 grupos de caracteres (CU-02 RN-03, RN-05). En la v3, `InputRules.MeetsPasswordPolicy` ya existe y solo la usan el cambio y la recuperación de contraseña.
- El código declara `NeutralLanguage=en` y `Language.Default` es inglés; D-21 fija es-MX.
- `Language.cs` y la restricción de la base fijan exactamente dos idiomas; el escenario ESC-11 del Hito 1 pide agregar un tercero sin tocar código.
- ESC-03 exige segundo factor en cada equipo nuevo; CU-01 lo hace opcional por cuenta.
- D-15 da 60 s para reclamar la victoria; ASR-04 cierra la partida a los 2 min.
- El orden de construcción del Hito 1 (DRV-08) pone motor, protocolo y servidor antes que cuentas y extras; el repo va al revés.
- `Bastion.Presentation.csproj` cita "Regla 1 y Regla 5 del estándar del equipo" (DesktopGL, WCF): no están en el estándar ni en los casos de uso, que hablan de TCP.
- El estándar pide elementos de base de datos en inglés; el modelo usa nombres en español (`Usuario`, `contrasena_hash`).
