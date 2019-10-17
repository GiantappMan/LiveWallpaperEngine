import express from 'express'
import WindowManager, { Wallpaper } from './window_manager'

let windowManager = new WindowManager();
let httpServer = express()

let hostPort = process.argv.find(m => { return m.indexOf("--hostPort") >= 0 });
if (hostPort)
  hostPort = hostPort.replace("--hostPort=", "");
else
  hostPort = "3000";
console.log(hostPort);

httpServer.get('/getInfo', function (req, res) {
  let result = windowManager.getInfo();
  res.send(result);
})

httpServer.get('/muteWindow', function (req, res) {
  let indexs = req.query.screenIndexs;
  let result = windowManager.muteWindow(indexs);
  res.send(result);
})

httpServer.get('/closeWallpaper', function (req, res) {
  let indexs = req.query.screenIndexs;
  let result = windowManager.closeWallpaper(indexs);
  res.send(result);
})

httpServer.get('/showWallpaper', function (req, res) {
  let indexs = req.query.screenIndexs;
  let result = windowManager.showWallpaper({
    path: req.query.path
  }, indexs);
  res.send(result)
})

httpServer.listen(hostPort);

