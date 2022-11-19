from time import sleep
from json import loads
from socket import socket
from pynput import keyboard
from pyperclip import paste
from settings import Settings
from utils import notification, copy

ws = socket()
port = 1234
ip = "127.0.0.1"
ws.connect((ip, port))
controller = keyboard.Controller()


def on_release(key):
    if key == keyboard.Key.f8:
        with controller.pressed(keyboard.Key.ctrl):
            with controller.pressed("a"):
                sleep(Settings.dict["CopyingHotkeyDelay"] / 1000)
                print(Settings.dict["CopyingHotkeyDelay"])
            with controller.pressed("c"):
                pass

        if Settings.dict["PressEscape"]:
            with controller.pressed(keyboard.Key.esc):
                pass

        send_receive_data(paste())


def send_receive_data(data):
    ws.send(data.encode("UTF-8"))
    packets = []
    while True:
        packet = ws.recv(1024)
        packets.append(packet.decode("UTF-8"))
        if len(packet) < 1024:
            break

    data = "".join(packets)
    SocketJSON.read_json(data).process_data()
    print(f"Python: Received {data}")


def main():
    with keyboard.Listener(on_release=on_release) as listener:
        listener.join()


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

            case "command":
                match self.args[0]:
                    case "exit":
                        print("Exiting...")
                        # ws.close()
                        notification("Exiting.", "utilities-cs is exiting...")
                        try:
                            exit(0)
                        except:
                            pass

                    case _:
                        notification("Something went wrong.", "An exception occured.")

            case _:
                notification("Something went wrong.", "An exception occured.")


if __name__ == "__main__":
    main()
