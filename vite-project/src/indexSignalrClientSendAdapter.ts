import type * as signalR from '@microsoft/signalr'

export class IndexSignalrClientSendAdapter {
    connection: signalR.HubConnection

    constructor(connection: signalR.HubConnection) {
        this.connection = connection
    }

    heartbeat(): Promise<void> {
        return this.connection.invoke('Heartbeat')
    }

    send(message: string): Promise<{ message: string }> {
        return this.connection.invoke('Send', message)
    }

    authorize(): Promise<void> {
        return this.connection.invoke('Authorize')
    }
}
