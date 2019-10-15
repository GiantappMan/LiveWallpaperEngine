
import { app, BrowserWindow } from 'electron';

export class Status {
    constructor() {
        this.initlized = false;
    }
    initlized: boolean;
}

export interface Wallpaper {

}

const windows: { [id: string]: BrowserWindow; } = {};
let status: Status = new Status();
app.on('ready', () => {
    status.initlized = true;
})

export default class WindowManager {
    getInfo() {
        return status;
    }
    getHosts(indexs: string[]) {
        const result: { [id: number]: number; } = {};
        for (const i of indexs) {
            let tmpIndex = parseInt(i);
            if (isNaN(tmpIndex))
                continue;

            if (!windows[tmpIndex]) {
                let window = new BrowserWindow({
                    skipTaskbar: true,
                    frame: false,
                });
                windows[tmpIndex] = window;
                window.loadURL(`file://${__dirname}/../html/loading/index.html`);
            }

            let handle = windows[tmpIndex].getNativeWindowHandle().readUInt32LE(0);
            result[tmpIndex] = handle;
        }

        return result;
    }
    showWallpaper(wallpaper: Wallpaper, screenIndex: number) {

    }
}