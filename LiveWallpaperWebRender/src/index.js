const { app, BrowserWindow } = require('electron')
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const express = require('express')
const httpServer = express()

httpServer.get('/', function (req, res) {
  res.send('Hello World')
})

httpServer.listen(3000)

let protoFile = `${__dirname}\\..\\..\\Protos\\api.proto`;
protoLoader.load(protoFile).then(packageDefinition => {
  const proto = grpc.loadPackageDefinition(packageDefinition);

  let package = proto["LiveWallpaperEngine"];
  let client = new package["API"]("127.0.0.1:8080", grpc.credentials.createInsecure());
  let test = client["CloseWallpaper"]({
    "ScreenIndexs": [3, 2]
  }, (e1) => {
    console.log("success" + e1);
  });
});
const windows = []
console.error("test");
app.on('ready', () => {
  let window = new BrowserWindow({
    skipTaskbar: true,
    frame: false,
  });

  window.openDevTools();

  let handle = window.getNativeWindowHandle().readUInt32LE();
  window
  windows.push(window)

  windows.forEach((window) => {
    window.loadURL(`file://${__dirname}/index.html`);
  })
})
