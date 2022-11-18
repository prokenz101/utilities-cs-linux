import socket
import time
from pynput import keyboard
import pyperclip
from utils import *

ws = socket.socket()
port = 1234
ip = "127.0.0.1"
ws.connect((ip, port))

controller = keyboard.Controller()


def on_release(key):
    if key == keyboard.Key.f8:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed("a"):
                time.sleep(Settings.dict["CopyingHotkeyDelay"] / 1000)
                print(Settings.dict["CopyingHotkeyDelay"])
            with controller.pressed("c"):
                pass

        send_receive_data(pyperclip.paste())


def send_receive_data(data):
    ws.send(data.encode("UTF-8"))
    data = ws.recv(1024).decode("UTF-8")
    print(f"Python: Received {data}")
    copy_paste(data)


def copy_paste(str):
    pyperclip.copy(str)

    if Settings.dict["AutoPaste"]:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed("v"):
                pass
    
    if Settings.dict["PressEscape"]:
        with controller.pressed(keyboard.Key.esc):
            pass

    notification("something happened", str)


def main():
    with keyboard.Listener(on_release=on_release) as listener:
        listener.join()


if __name__ == "__main__":
    main()
