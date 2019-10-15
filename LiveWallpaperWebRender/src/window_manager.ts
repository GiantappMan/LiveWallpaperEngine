
import { app, BrowserWindow } from 'electron';

export class Status {
    constructor() {
        this.initlized = false;
    }
    initlized: boolean;
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
        const result: { [id: string]: number; } = {};
        for (const i of indexs) {
            if (!windows[i]) {
                let window = new BrowserWindow({
                    skipTaskbar: true,
                    frame: false,
                });
                windows[i] = window;
                window.loadURL(`file://${__dirname}/../html/loading/index.html`);
            }

            let handle = windows[i].getNativeWindowHandle().readUInt32LE(0);
            result[i] = handle;
        }

        return result;
    }
}