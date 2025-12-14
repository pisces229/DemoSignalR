import { SignalrClientReceiveAdapter } from './signalrClientReceiveAdapter'
import type * as signalR from '@microsoft/signalr'

export class IndexSignalrClientReceiveAdapter {
    connection: signalR.HubConnection
    receive: SignalrClientReceiveAdapter<string>
    receiveDto: SignalrClientReceiveAdapter<{ title: string, content: string }>

    constructor(connection: signalR.HubConnection) {
        this.connection = connection
        this.receive = new SignalrClientReceiveAdapter<string>(this.connection, 'Receive')
        this.receiveDto = new SignalrClientReceiveAdapter<{ title: string, content: string }>(this.connection, 'ReceiveDto')
    }

    public clearAllHandlers() {
        this.receive.clearHandler()
        this.receiveDto.clearHandler()
    }
}
