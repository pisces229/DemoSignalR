import { SignalrClientReceiveAdapter } from './signalrClientReceiveAdapter'
import type * as signalR from '@microsoft/signalr'

export class IndexSignalrClientReceiveAdapter {
    connection: signalR.HubConnection
    receive: SignalrClientReceiveAdapter<{ message: string }>

    constructor(connection: signalR.HubConnection) {
        this.connection = connection
        this.receive = new SignalrClientReceiveAdapter<{ message: string }>(this.connection, 'Receive')
    }

    public clearAllHandlers() {
        this.receive.clearHandler()
    }
}
