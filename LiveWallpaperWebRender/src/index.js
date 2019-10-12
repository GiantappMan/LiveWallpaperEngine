// The "BrowserWindow" class is the primary way to create user interfaces
// in Electron. A BrowserWindow is, like the name suggests, a window.
//
// For more info, see:
// https://electronjs.org/docs/api/browser-window

const { app, BrowserWindow } = require('electron')

const windows = []

app.on('ready', () => {
  // BrowserWindows can be created in plenty of shapes, sizes, and forms.
  // Check out the editor's auto-completion for all the configuration
  // options available in the current version.
  //
  // Let's make a few windows!

  // A transparent window
  var window = new BrowserWindow({
    skipTaskbar: true,
    frame: false,
  });
  let handle = window.getNativeWindowHandle().readUInt32LE();
  windows.push(window)

  windows.forEach((window) => {
    // Load our index.html
    window.loadURL(`file://${__dirname}/index.html`);
  })
})
