"""Scores each GUI in signals.json against the 55 GUIs the use cases name.

Two steps:
    python gui_signals.py <repo_root> signals.json
    python score_gui_implementation.py <repo_root> signals.json

Rubric (100 pts per GUI, see gui_signals.py for the measured columns):
  Accesible      10  reachable from Login, via the Navigator graph
  Elementos      25  fraction of what the CU asks the screen to show (judgment)
  Textos         10  catalog present in both languages (measured)
  Acciones       25  fraction of buttons with a non-empty handler (measured)
  Reglas de UI   30  validation, messages, confirmations the CU assigns (judgment)

The two judged columns (elementos, reglas) are opinion, recorded in JUDGE below
with the GUI it came from; update them by hand as screens change. A GUI not in
JUDGE defaults to (0, 0), which is conservative, not a bug.
"""
import json
import pathlib
import re
import sys

sys.stdout.reconfigure(encoding="utf-8")

if len(sys.argv) != 3:
    raise SystemExit(f"uso: python {pathlib.Path(__file__).name} <repo_root> <signals.json>")

repo_root = pathlib.Path(sys.argv[1])
signals_path = pathlib.Path(sys.argv[2])
SPEC = 55

# (elementos, reglas) 0..1, judged against what each CU step asks the screen
# to show and enforce, as of 2026-09-22.
JUDGE = {
    "ActiveSessions": (.75, .75), "AddFriend": (.90, .25), "AdminPanel": (.70, .50),
    "Appeal": (.90, .25), "ApplySanction": (.85, .50), "BoxPurchaseConfirm": (.90, .25),
    "ChangeEmail": (1, .80), "ChangeNickname": (1, .90), "ChangePassword": (1, .90),
    "CoinHistory": (.60, .25), "Customize": (.75, .25), "DeleteAccount": (1, .90),
    "EditProfile": (.90, .90), "ForgotPassword": (1, .80), "Friends": (.40, .25),
    "Login": (.875, .75), "Logs": (.60, .25), "MatchHistory": (.50, .25),
    "MessageConfirm": (1, 1), "MessageError": (1, 1), "MessageSuccess": (1, 1), "MessageWarning": (1, 1),
    "ModerationQueue": (.50, .25), "PlayerCard": (.30, .25), "Profile": (.90, .75),
    "PurchaseConfirm": (1, .25), "Ranking": (.70, .25), "Register": (.89, .75),
    "RegistrationSuccess": (1, 1), "Report": (1, .25), "ReportReview": (.60, .25),
    "ResetPassword": (1, .90), "Settings": (.80, .75), "AccountSettings": (.80, .75), "Shop": (.50, .25),
    "MainScreen": (.60, .50), "PendingVerification": (.90, .60), "SecondFactor": (.90, .70),
    "BannedAccount": (.90, .40), "FirstTime": (.85, .60), "LinkAccount": (.75, .70),
    "MainMenu": (.75, .40), "SelectMode": (.90, .60), "Matchmaking": (.90, .60),
    "VersusScreen": (.85, .30), "Match": (.55, .55), "MatchEnd": (.60, .55),
    "OpponentDisconnected": (.90, .75), "PrivateMatch": (.90, .70), "WaitingRoom": (.75, .60),
    "AIDifficulty": (.90, .40), "AIMatchEnd": (.75, .30), "Spectator": (.75, .30),
    "Replay": (.75, .40), "TutorialIndex": (.75, .40),
}
EXTRA = {"Menu"}  # provisional review menu, not a use-case screen


def reachable(root):
    pres = root / "Presentation"
    nav = (pres / "Navigator.cs").read_text(encoding="utf-8")
    screen_class = {}
    for m in re.finditer(r"\[ScreenId\.(\w+)\]\s*=\s*\w+\s*=>\s*new\s+(\w+)\(", nav):
        screen_class[m.group(1)] = m.group(2)
    for m in re.finditer(r"if\s*\(screen\s*==\s*ScreenId\.(\w+)\)\s*\{\s*return\s+new\s+(\w+)\(", nav):
        screen_class[m.group(1)] = m.group(2)

    class_files = {}
    for d in pres.glob("GUI_*"):
        txt = "\n".join(f.read_text(encoding="utf-8") for f in d.glob("*.cs"))
        cm = re.search(r"class\s+(Gui\w+)", txt)
        if cm:
            class_files[cm.group(1)] = txt

    edges = {}
    for scr, cls in screen_class.items():
        refs = set(re.findall(r"ScreenId\.(\w+)", class_files.get(cls, "")))
        edges.setdefault(scr, set()).update(refs)

    seen, todo = {"Login"}, ["Login"]
    while todo:
        cur = todo.pop()
        for nxt in edges.get(cur, ()):
            if nxt not in seen:
                seen.add(nxt)
                todo.append(nxt)

    return seen, screen_class


def tier(total):
    if total >= 85:
        return "A"
    if total >= 70:
        return "B"
    return "C"


def main():
    signals = json.loads(signals_path.read_text(encoding="utf-8"))
    seen, screen_class = reachable(repo_root)
    class_screens = {}
    for scr, cls in screen_class.items():
        class_screens.setdefault(cls, set()).add(scr)

    rows = []
    for gui, r in signals.items():
        g = gui[4:]
        if g in EXTRA:
            continue
        e, rules = JUDGE.get(g, (0.0, 0.0))
        if r["dialog_kind"]:
            acc = 10 if r["dialog_uses"] else 5
        elif any(sc in seen for sc in class_screens.get(r["class"], ())):
            acc = 10
        elif r["class"] in class_screens:
            acc = 5
        else:
            acc = 0
        txt = 10 if not r["missing_en"] and not r["missing_es"] else 5
        actions = r["events"] + r["lambdas"]
        act = 1.0 if actions == 0 else max(0.0, (actions - len(r["empty_handlers"])) / actions)
        total = acc + (25 * e) + txt + (25 * act) + (30 * rules)
        rows.append((g, acc, e, txt, act, rules, total))

    print(f"GUI implementadas: {len(rows)} de {SPEC} nombradas en los casos de uso")
    print(f"{'GUI':22}{'acc':>4}{'elem':>6}{'txt':>4}{'acc.':>6}{'reglas':>7}{'  TOTAL':>8}  nivel")
    for g, acc, e, txt, act, rules, total in sorted(rows, key=lambda x: -x[6]):
        print(f"{g:22}{acc:4}{e:6.2f}{txt:4}{act:6.2f}{rules:7.2f}{total:8.1f}  {tier(total)}")

    tot = sum(r[6] for r in rows)
    print(f"\npromedio de las {len(rows)} existentes: {tot / len(rows):.1f}%")
    print(f"sobre las {SPEC} especificadas (faltantes = 0): {tot / SPEC:.1f}%")
    print(f"pantallas alcanzables desde Login: {len(seen)}")
    unreachable = [r[0] for r in rows if r[1] == 5]
    print(f"registradas pero sin camino desde Login: {len(unreachable)} -> {', '.join(unreachable)}")


if __name__ == "__main__":
    main()
