export type ConnectionChangedHandler = (oldState: boolean, newState: boolean) => void

export class SignalrReconnectionHandler {
    handlers: ConnectionChangedHandler[] = []

    constructor(connectionChangedHandlers: ConnectionChangedHandler[]) {
        this.handlers = []
        connectionChangedHandlers.push((oldState, newState) => this.handle(oldState, newState, this.handlers))
    }

    addHandler(handler: ConnectionChangedHandler) {
        this.handlers.push(handler)
        return handler
    }

    removeHandler(handler: ConnectionChangedHandler) {
        const index = this.handlers.indexOf(handler)
        if (index !== -1) {
            this.handlers.splice(index, 1)
        }
    }

    clearHandler() {
        this.handlers = []
    }

    handle(oldState: boolean, newState: boolean, handlers: ConnectionChangedHandler[]) {
        if (!newState) {
            return
        }

        for (const handler of handlers) {
            handler(oldState, newState)
        }
    }
}
