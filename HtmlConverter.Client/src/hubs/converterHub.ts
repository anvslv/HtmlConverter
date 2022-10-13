import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

class ConverterHub {
    client: HubConnection;
   
    constructor() {
        this.client = new HubConnectionBuilder()
            .withUrl(import.meta.env.VITE_SERVER_URL + "/api/converterHub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }

    start() {
        this.client.start();
    }
}

export default new ConverterHub();