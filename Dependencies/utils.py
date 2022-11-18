from pathlib import Path
from posixpath import expanduser
import os
from json import load

settings_path = "settings.json"

class Settings:
    dict = {}

    @staticmethod
    def get_settings():
        if Path(settings_path).exists():
            with open(settings_path, "r") as f:
                Settings.dict = load(f)

Settings.get_settings()
            
def notification(
    title,
    subtitle,
    icon_path=Path(f"{expanduser('~')}/utilities-cs/Assets/UtilitiesIcon.png"),
    length_override=False,
    icon_path_override=False,
):
    if Settings.dict["DisableNotifications"]: return

    if icon_path.exists():
        if len(title) <= 29 and len(subtitle) <= 32 or length_override:
            os.system(
                f'notify-send --hint int:transient:1 -i {icon_path} "{title}" "{subtitle}"'
            )
        else:
            os.system(
                f'notify-send --hint int:transient:1 -i {icon_path} "{"Sucess"}" "{"Too Long to show"}"'
            )

    elif icon_path_override:
        os.system(f'notify-send --hint int:transient:1 "{title}" "{subtitle}"')

    else:
        raise Exception("The specified icon path was not found.")