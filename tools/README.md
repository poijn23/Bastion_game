# tools/

Scripts de análisis del avance de la capa de presentación, usados para medir
qué porcentaje de las GUI que piden los casos de uso ya están implementadas.
No forman parte de la app: son para el equipo, no se compilan ni se publican.

Requieren Python 3.

## Uso

```bash
python tools/gui_signals.py . signals.json
python tools/score_gui_implementation.py . signals.json
```

- `gui_signals.py` recorre `Presentation/GUI_*`, mide señales objetivas de
  cada pantalla (controles, eventos con manejador vacío, claves de catálogo
  presentes en los dos idiomas, literales sueltos, etc.) y las vuelca en un
  JSON.
- `score_gui_implementation.py` lee ese JSON, calcula si cada pantalla es
  alcanzable desde `Login` recorriendo el grafo de `Navigator.cs`, y aplica
  una rúbrica de 100 puntos por pantalla (accesible 10, elementos que pide el
  CU 25, textos en catálogo 10, acciones con manejador 25, reglas de interfaz
  del CU 30). Dos de esas cinco columnas —elementos y reglas— son juicio
  propio, guardado en el diccionario `JUDGE` de ese script; actualízalo a
  mano cuando una pantalla cambie mucho.

Ambos toman la raíz del repo como primer argumento, así que también sirven
sobre una copia en otra rama o carpeta.
