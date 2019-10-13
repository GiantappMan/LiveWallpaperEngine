// The "BrowserWindow" class is the primary way to create user interfaces
// in Electron. A BrowserWindow is, like the name suggests, a window.
//
// For more info, see:
// https://electronjs.org/docs/api/browser-window

const { app, BrowserWindow } = require('electron')
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');

let protoFile = `${__dirname}\\..\\..\\Protos\\api.proto`;
protoLoader.load(protoFile).then(packageDefinition => {
  const proto = grpc.loadPackageDefinition(packageDefinition);
  // let client = new service.LiveWallpaperEngine("http://127.0.0.1:8080");
  // const client = http2.connect("http://127.0.0.1:8080");

  let package = proto["LiveWallpaperEngine"];
  // let client = grpc.makeClientConstructor(packageDefinition.LiveWallpaperEngine, "http://127.0.0.1:8080");
  let client = new package["API"]("127.0.0.1:8080", grpc.credentials.createInsecure());
  // var test = client.CloseWallpaper([1, 2]);

  let test = client["CloseWallpaper"]({
    "ScreenIndexs": [3, 2]
  }, (e1) => {
    console.log("success" + e1);
  });
});
const windows = []
console.error("test");
app.on('ready', () => {
  // BrowserWindows can be created in plenty of shapes, sizes, and forms.
  // Check out the editor's auto-completion for all the configuration
  // options available in the current version.
  //
  // Let's make a few windows!

  // A transparent window
  let window = new BrowserWindow({
    skipTaskbar: true,
    frame: false,
  });

  window.openDevTools();


  let handle = window.getNativeWindowHandle().readUInt32LE();
  window
  windows.push(window)

  windows.forEach((window) => {
    // Load our index.html
    window.loadURL(`file://${__dirname}/index.html`);
  })
})
