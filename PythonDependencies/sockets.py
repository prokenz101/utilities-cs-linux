from time import sleep
from socket import socket
from pynput import keyboard
from pyperclip import paste
from utils import SocketJSON
from settings import Settings

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

        send_receive_data(paste())


def send_receive_data(data):
    ws.send(data.encode("UTF-8"))
    # data = ""
    # packet = ws.recv(1024).decode("UTF-8")
    # data += packet
    # while packet:
    #     print("loopin")
    #     packet = ws.recv(1024).decode("UTF-8")
    #     print(packet)
    #     data += packet
    packets = []
    while True:
        packet = ws.recv(1024)
        packets.append(packet.decode("UTF-8"))
        if len(packet) < 1024:
            break
    
    print("stopped loopin")
    data = "".join(packets)
    print(data)
    SocketJSON.read_json(data).process_data()
    print(f"Python: Received {data}")


def main():
    with keyboard.Listener(on_release=on_release) as listener:
        listener.join()


if __name__ == "__main__":
    main()
