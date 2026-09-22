"""Mide senales objetivas de cada GUI de un arbol del repo Bastion_game.

Uso: python gui_signals.py <raiz del repo> <salida.json>
"""
import json
import pathlib
import re
import sys

sys.stdout.reconfigure(encoding="utf-8")

root = pathlib.Path(sys.argv[1])
out_path = sys.argv[2]
pres = root / "Presentation"


def strip_comments(src):
    out = []
    i, n = 0, len(src)
    in_str = False
    while i < n:
        c = src[i]
        if in_str:
            out.append(c)
            if c == "\\" and i + 1 < n:
                out.append(src[i + 1])
                i += 2
                continue
            if c == '"':
                in_str = False
            i += 1
            continue
        if c == '"':
            in_str = True
            out.append(c)
            i += 1
            continue
        if c == "'":
            j = src.find("'", i + 2 if src[i + 1] != "\\" else i + 3)
            if 0 < j - i <= 4:
                out.append(src[i:j + 1])
                i = j + 1
                continue
        if c == "/" and i + 1 < n and src[i + 1] == "/":
            while i < n and src[i] != "\n":
                i += 1
            continue
        if c == "/" and i + 1 < n and src[i + 1] == "*":
            i += 2
            while i + 1 < n and not (src[i] == "*" and src[i + 1] == "/"):
                i += 1
            i += 2
            continue
        out.append(c)
        i += 1
    return "".join(out)


def method_body(code, name):
    m = re.search(r"\b(?:void|bool|string|int|Rectangle)\s+" + re.escape(name) + r"\s*\([^)]*\)\s*(=>|\{)", code)
    if not m:
        return None
    if m.group(1) == "=>":
        end = code.find(";", m.end())
        return code[m.end():end]
    depth, i = 1, m.end()
    while i < len(code) and depth:
        if code[i] == "{":
            depth += 1
        elif code[i] == "}":
            depth -= 1
        i += 1
    return code[m.end():i - 1]


# --- catalogo de textos -----------------------------------------------------
catalog = (root / "Resources" / "TextCatalog.cs").read_text(encoding="utf-8")
prop_to_key = {}
for m in re.finditer(r"public static string (\w+) => GetText\((?:nameof\((\w+)\)|\"([^\"]+)\")\)", catalog):
    prop_to_key[m.group(1)] = m.group(2) or m.group(3)


def resx_keys(path):
    return set(re.findall(r'<data name="([^"]+)"', (root / "Resources" / path).read_text(encoding="utf-8")))


keys_en = resx_keys("TextCatalog.resx")
keys_es = resx_keys("TextCatalog.es-MX.resx")

# --- tipos de control -------------------------------------------------------
control_types = set()
for f in (pres / "Utils").glob("*.cs"):
    for m in re.finditer(r"class\s+(\w+)\s*(?:<[^>]*>)?\s*:\s*Control\b", f.read_text(encoding="utf-8")):
        control_types.add(m.group(1))

# --- registro en el Navigator -----------------------------------------------
nav = strip_comments((pres / "Navigator.cs").read_text(encoding="utf-8"))
screen_to_class = {}
for m in re.finditer(r"\[ScreenId\.(\w+)\]\s*=\s*\w+\s*=>\s*new\s+(\w+)\(", nav):
    screen_to_class[m.group(1)] = m.group(2)
if "GuiRegistrationSuccess" in nav:
    screen_to_class["RegistrationSuccess"] = "GuiRegistrationSuccess"
class_to_screen = {v: k for k, v in screen_to_class.items()}

all_cs = {p: strip_comments(p.read_text(encoding="utf-8")) for p in pres.rglob("*.cs")}
dialog_usage = {
    "Error": sum(t.count("DialogTone.Error") for p, t in all_cs.items() if p.name != "Navigator.cs"),
    "Success": sum(t.count("DialogTone.Success") for p, t in all_cs.items() if p.name != "Navigator.cs"),
    "Warning": sum(t.count("DialogTone.Warning") for p, t in all_cs.items() if p.name != "Navigator.cs"),
    "Confirm": sum(t.count("ShowConfirm(") for p, t in all_cs.items() if p.name not in ("Navigator.cs", "INavigator.cs")),
}

records = {}
for gui_dir in sorted(pres.glob("GUI_*")):
    name = gui_dir.name
    files = sorted(gui_dir.glob("*.cs"))
    code = "\n".join(all_cs[f] for f in files)
    raw = "\n".join(f.read_text(encoding="utf-8") for f in files)
    cls = re.search(r"class\s+(Gui\w+)\s*:\s*(\w+)", code)
    class_name = cls.group(1) if cls else "?"
    base = cls.group(2) if cls else "?"

    loc = sum(1 for line in code.splitlines() if line.strip())

    controls = {}
    for t in control_types:
        c = len(re.findall(r"\bnew\s+" + t + r"\b", code))
        if c:
            controls[t] = c
    for helper, t in (("CreatePrimaryButton", "Button"), ("CreateSecondaryButton", "Button"), ("CreateValueBox", "ValueBox")):
        c = len(re.findall(r"\b" + helper + r"\(", code))
        if c:
            controls[t] = controls.get(t, 0) + c
    lang_picker = "LanguagePicker" in code

    # eventos y manejadores
    events = []
    for m in re.finditer(r"([\w.]+?)\.(\w+)\s*\+=\s*(\w+)\s*;", code):
        target, event, handler = m.groups()
        body = method_body(code, handler)
        if body is None:
            kind = "missing"
        else:
            b = re.sub(r"\s+", "", body)
            if b == "":
                kind = "empty"
            elif "GoTo(" in b or "GoBack(" in b:
                kind = "navigates"
            elif "ShowConfirm(" in b or "ShowMessage(" in b:
                kind = "dialog"
            else:
                kind = "logic"
        events.append((target, event, handler, kind))
    lambdas = len(re.findall(r"\+=\s*(?:\([^)]*\)|\w+)\s*=>", code))
    empty_handlers = [e[2] for e in events if e[3] in ("empty", "missing")]

    disabled = len(re.findall(r"IsEnabled\s*=\s*false", code))

    used_props = sorted(set(re.findall(r"TextCatalog\.(\w+)", code)))
    sample_props = [p for p in used_props if "Sample" in p]
    missing_en = [p for p in used_props if prop_to_key.get(p, p) not in keys_en]
    missing_es = [p for p in used_props if prop_to_key.get(p, p) not in keys_es]

    literals = [s for s in re.findall(r'"((?:[^"\\\n]|\\.)*)"', code) if s.strip()]

    goto = sorted(set(re.findall(r"GoTo\(ScreenId\.(\w+)", code)))
    back = code.count("GoBack(")
    confirms = code.count("ShowConfirm(")
    messages = code.count("ShowMessage(")
    warnings = len(re.findall(r"\.Warning\s*=", code))
    validators = len(re.findall(r"\b(?:bool|void)\s+(?:Validate|Is\w*Valid|Check)\w*\(", code))
    stub_test = len(re.findall(r"\bTest(?:Account|Profile)\.", code))
    todos = len(re.findall(r"TODO|FIXME|NotImplemented", raw))

    screen = class_to_screen.get(class_name)
    if screen:
        own = set(files)
        inbound = sorted({p.parent.name for p, t in all_cs.items()
                          if p not in own and p.name not in ("ScreenId.cs", "Navigator.cs")
                          and re.search(r"ScreenId\." + screen + r"\b", t)})
        # el menu / barra lateral de v3 tambien cuenta como entrada
        registered = True
    else:
        inbound = []
        registered = False
    dialog_kind = None
    for k in ("Error", "Success", "Warning", "Confirm"):
        if class_name == f"GuiMessage{k}":
            dialog_kind = k
    records[name] = {
        "class": class_name, "base": base, "loc": loc, "controls": controls,
        "lang_picker": lang_picker, "events": len(events), "lambdas": lambdas,
        "empty_handlers": empty_handlers, "disabled": disabled,
        "catalog_keys": len(used_props), "sample_keys": sample_props,
        "missing_en": missing_en, "missing_es": missing_es,
        "literals": literals, "goto": goto, "back": back, "confirms": confirms,
        "messages": messages, "warnings": warnings, "validators": validators,
        "stub_test": stub_test, "todos": todos, "registered": registered,
        "screen": screen, "inbound": inbound,
        "dialog_kind": dialog_kind,
        "dialog_uses": dialog_usage.get(dialog_kind, 0) if dialog_kind else None,
        "used_props": used_props,
    }

json.dump(records, open(out_path, "w", encoding="utf-8"), ensure_ascii=False, indent=1)

hdr = f"{'GUI':26}{'base':13}{'loc':>4} {'ctl':>3} {'ev':>2} {'vac':>3} {'dis':>3} {'txt':>3} {'smp':>3} {'lit':>3} {'wrn':>3} {'val':>3} {'cnf':>3} {'msg':>3} {'nav':>3} {'ent':>3} {'stub':>4}"
print(hdr)
for name, r in records.items():
    print(f"{name[4:]:26}{r['base']:13}{r['loc']:4} {sum(r['controls'].values()):3} {r['events']:2} {len(r['empty_handlers']):3} {r['disabled']:3} "
          f"{r['catalog_keys']:3} {len(r['sample_keys']):3} {len(r['literals']):3} {r['warnings']:3} {r['validators']:3} {r['confirms']:3} {r['messages']:3} "
          f"{len(r['goto']) + (1 if r['back'] else 0):3} {len(r['inbound']):3} {r['stub_test']:4}")
print("uso de dialogos:", dialog_usage)
print("claves catalogo:", len(prop_to_key), "props;", len(keys_en), "en;", len(keys_es), "es-MX")
print("tipos de control:", sorted(control_types))
