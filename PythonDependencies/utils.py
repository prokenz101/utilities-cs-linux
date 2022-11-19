import pyperclip
from pathlib import Path
from pynput import keyboard
from os import path, system
from settings import Settings
from posixpath import expanduser

user = expanduser("~")
utilities_cs_path = path.join(user, "utilities-cs")

controller = keyboard.Controller()


def notification(
    title,
    subtitle,
    icon_path=Path(user, "utilities-cs/Assets/UtilitiesIcon.png"),
    length_override=False,
    icon_path_override=False,
) -> None:
    if Settings.dict["DisableNotifications"]:
        return

    if icon_path.exists():
        if len(title) <= 29 and len(subtitle) <= 32 or length_override:
            system(
                f'notify-send --hint int:transient:1 -i {icon_path} "{title}" "{subtitle}"'
            )
        else:
            system(
                f'notify-send --hint int:transient:1 -i {icon_path} "{"Check your clipboard."}" "{"This notification was too long."}"'
            )

    elif icon_path_override:
        system(f'notify-send --hint int:transient:1 "{title}" "{subtitle}"')

    else:
        raise Exception("The specified icon path was not found.")


def copy(str):
    pyperclip.copy(str)

    if Settings.dict["AutoPaste"]:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed("v"):
                pass
