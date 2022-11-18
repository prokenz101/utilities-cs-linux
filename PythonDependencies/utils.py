import pyperclip
from json import loads
from pynput import keyboard
from os import path, system
from settings import Settings
from posixpath import expanduser
from pathlib import Path

user = expanduser("~")
utilities_cs_path = path.join(user, "utilities-cs")

controller = keyboard.Controller()


class SocketJSON:
    def __init__(self, request: str, args: list):
        self.request = request
        self.args = args

    @staticmethod
    def read_json(json):
        socket_response = loads(json)
        return SocketJSON(socket_response["Request"], socket_response["Arguments"])

    def process_data(self) -> None:
        match self.request:
            case "regular":
                print(self.args[0])
                copy(self.args[0])
                notification(*self.args[1:])

            case "notification":
                notification(*self.args)
                if Settings.dict["PressEscape"]:
                    with controller.pressed(keyboard.Key.esc):
                        pass
            
            case _:
                print("not regular or notification")


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
                f'notify-send --hint int:transient:1 -i {icon_path} "{"Sucess"}" "{"Too Long to show"}"'
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

    if Settings.dict["PressEscape"]:
        with controller.pressed(keyboard.Key.esc):
            pass
