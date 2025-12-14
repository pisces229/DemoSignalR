import type * as signalR from '@microsoft/signalr'

export class SignalrClientReceiveAdapter<T> {
    connection: signalR.HubConnection
    handlers: ((input: T) => void)[] = []

    constructor(connection: signalR.HubConnection, clientName: string) {
        this.connection = connection
        this.handlers = []

        this.connection.on(clientName, (input) => {
            for (let i = 0; i < this.handlers.length; i++) {
                this.handlers[i](input)
            }
        })
    }

    addHandler(handler: (input: T) => void) {
        this.handlers.push(handler)
        return handler
    }

    removeHandler(handler: (input: T) => void) {
        for (let i = 0; i < this.handlers.length; i++) {
            if (this.handlers[i] === handler) {
                this.handlers.splice(i, 1)
                break
            }
        }
    }

    clearHandler() {
        this.handlers = []
    }
}
