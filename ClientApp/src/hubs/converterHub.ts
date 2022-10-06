import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

class ConverterHub {
    client: HubConnection;
   
    constructor() {
        this.client = new HubConnectionBuilder()
            .withUrl("/api/converterHub")
            .build();
    }

    start() {
        this.client.start();
    }
}

export default new ConverterHub();