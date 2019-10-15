import { app, BrowserWindow } from 'electron';
import express from 'express'
let httpServer = express()

httpServer.get('/', function (req, res) {
  res.send('Hello World')
})

httpServer.listen(3000)

const windows: BrowserWindow[] = []
console.error("test");
app.on('ready', () => {
  let window = new BrowserWindow({
    skipTaskbar: true,
    frame: false,
  });

  // window.webContents.openDevTools();

  let handle = window.getNativeWindowHandle().readUInt32LE(0);
  windows.push(window)

  windows.forEach((window) => {
    window.loadURL(`file://${__dirname}/../html/loading/index.html`);
  })
})
