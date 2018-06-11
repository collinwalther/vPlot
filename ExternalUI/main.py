#!/usr/bin/env python2

from enum import IntEnum
import os
import tkinter as tk
import tkinter.filedialog

# define a main window class that derives from Tkinter.Frame
class MainFrame(tk.Frame):

    class RunType(IntEnum):
        FILE = 0
        CLIENT = 1
        SERVER = 2

    def __init__(self, master = None):
        tk.Frame.__init__(self, master)
        self.master = master
        self.master.title("vPlot Launcher")
        self.pack()

        # initialize the radio buttons
        self.m_runType = tk.IntVar()
        self.m_runType.set(MainFrame.RunType.FILE.value)
        radioButtonFile = tk.Radiobutton(self, text = "File", variable = self.m_runType, value = MainFrame.RunType.FILE.value, command = self.onClickRadioFile)
        radioButtonFile.pack(side = tk.TOP)
        radioButtonSocketClient = tk.Radiobutton(self, text = "Socket: client", variable = self.m_runType, value = MainFrame.RunType.CLIENT.value, command = self.onClickRadioSocketClient)
        radioButtonSocketClient.pack(side = tk.TOP)
        radioButtonSocketServer = tk.Radiobutton(self, text = "Socket: server", variable = self.m_runType, value = MainFrame.RunType.SERVER.value, command = self.onClickRadioSocketServer)
        radioButtonSocketServer.pack(side = tk.TOP)

        # initialize the file frame
        self.m_frameFile = tk.Frame(self)
        self.m_textFile = tk.Entry(self.m_frameFile, width=50)
        self.m_textFile.pack(side = tk.LEFT)
        buttonFile = tk.Button(self.m_frameFile, text = "Choose File", command = self.onClickFile)
        buttonFile.pack(side = tk.RIGHT)
        self.m_frameFile.pack(side = tk.TOP)

        # initialize the socket IP frame
        self.m_frameSocket = tk.Frame(self)
        self.m_labelSocketIP = tk.Label(self.m_frameSocket, text = "IP Address:")
        self.m_labelSocketIP.grid(column = 0, row = 0)
        self.m_textSocketIP = tk.Entry(self.m_frameSocket, width=50)
        self.m_textSocketIP.grid(column = 1, row = 0)
        labelSocketPort = tk.Label(self.m_frameSocket, text = "Port:")
        labelSocketPort.grid(column = 0, row = 1)
        self.m_textSocketPort = tk.Entry(self.m_frameSocket, width=50)
        self.m_textSocketPort.grid(column = 1, row = 1)

        # initialize the start button
        buttonStart = tk.Button(self, text = "Start", command = self.onClickStart)
        buttonStart.pack(side = tk.BOTTOM)

    def onClickRadioFile(self):
        self.m_frameFile.pack(side = tk.TOP)
        self.m_frameSocket.pack_forget()
    
    def onClickRadioSocketClient(self):
        self.m_frameFile.pack_forget()
        self.m_frameSocket.pack(side = tk.TOP)
        self.m_labelSocketIP.grid(column = 0, row = 0)
        self.m_textSocketIP.grid(column = 1, row = 0)

    def onClickRadioSocketServer(self):
        self.m_frameFile.pack_forget()
        self.m_frameSocket.pack(side = tk.TOP)
        self.m_labelSocketIP.grid_forget()
        self.m_textSocketIP.grid_forget()

    def onClickFile(self):
        filepath = tk.filedialog.askopenfilename(initialdir = "/", title = "Choose a file")
        if(self.m_textFile.get() != None and filepath != ""):
            self.m_textFile.delete("0", tk.END)
        self.m_textFile.insert(tk.INSERT, filepath)

    def onClickStart(self):
        if os.name == "posix":
            cmd = "open vPlot.app --args"
        else:
            cmd = "vPlot"

        if self.m_runType.get() == MainFrame.RunType.FILE.value:
            os.system("{} --file ".format(cmd) + self.m_textFile.get())
        elif self.m_runType.get() == MainFrame.RunType.CLIENT.value:
            os.system("{} --client ".format(cmd) + self.m_textSocketIP.get() + " " + self.m_textSocketPort.get())
        else:
            os.system("{} --server ".format(cmd) + self.m_textSocketPort.get())


if __name__ == "__main__":
    window = tk.Tk()
    app = MainFrame(window)
    window.mainloop()
