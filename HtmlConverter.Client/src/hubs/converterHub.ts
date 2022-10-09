import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

class ConverterHub {
    client: HubConnection;
   
    constructor() {
        this.client = new HubConnectionBuilder()
            .withUrl(import.meta.env.VITE_SERVER_URL + "/api/converterHub")
            .build();
    }

    start() {
        this.client.start();
    }
}

export default new ConverterHub();