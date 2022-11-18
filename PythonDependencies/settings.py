from os import path
from json import load
from pathlib import Path
from posixpath import expanduser

utilities_cs_path = path.join(expanduser("~"), "utilities-cs")

class Settings:
    dict = {}

    @staticmethod
    def get_settings():
        settings_path = path.join(utilities_cs_path, "settings.json")
        
        if Path(settings_path).exists():
            with open(settings_path, "r") as f:
                Settings.dict = load(f)


current_settings = Settings.get_settings()
