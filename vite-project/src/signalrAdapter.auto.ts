import * as signalR from '@microsoft/signalr'
import { type ConnectionChangedHandler, SignalrReconnectionHandler } from './signalrReconnectionHandler'
import { IndexSignalrClientReceiveAdapter } from './indexSignalrClientReceiveAdapter'
import { IndexSignalrClientSendAdapter } from './indexSignalrClientSendAdapter'

class SignalrAdapter {
    transport = signalR.HttpTransportType.WebSockets
    connection!: signalR.HubConnection
    isConnected = false
    // client receive adapter
    indexSignalrClientReceiveAdapter!: IndexSignalrClientReceiveAdapter
    indexSignalrClientSendAdapter!: IndexSignalrClientSendAdapter
    // reconnection handler
    connectionChangedHandlers: ConnectionChangedHandler[] = []
    signalrReconnectionHandler!: SignalrReconnectionHandler
    // heartbeat
    heartbeatIntervalId: ReturnType<typeof setInterval> | undefined = undefined

    async init() {
        await this.start()

        this.connection.onreconnecting(async () => {
            this.log(`sigalr get onreconnecting, current connected is ${this.isConnected}`)
            this.setConnectionState(false)
        })
        this.connection.onreconnected(async () => {
            this.log(`sigalr get onreconnected, current connected is ${this.isConnected}`)
            this.setConnectionState(true)
        })
        this.connection.onclose(async () => {
            this.log(`sigalr get onclose, current connected is ${this.isConnected}`)
            this.setConnectionState(false)
        })
        
        // client receive adapter
        this.indexSignalrClientReceiveAdapter = new IndexSignalrClientReceiveAdapter(this.connection)
        this.indexSignalrClientSendAdapter = new IndexSignalrClientSendAdapter(this.connection)
        // reconnection handler
        this.signalrReconnectionHandler = new SignalrReconnectionHandler(this.connectionChangedHandlers)

        this.startHeartbeat()
        this.setConnectionState(true)

        document.addEventListener('visibilitychange', async () => {
            this.log(`visibilitychange, document.visibilityState = ${document.visibilityState}`)
            if (document.visibilityState === 'visible') {
                // 強制進行一次 ping，限制最多等 1.5 秒
                if (!await this.checkConnectionHealth(this.connection, 1500)) {
                    this.log(
                        `[${new Date().toISOString()}] visibilitychange, signalr connection is not alive, stop connection`
                    )
                } else {
                    this.log(`[${new Date().toISOString()}] visibilitychange, signalr connection is alive`)
                }
            }
        })
    }

    async stop() {
        clearInterval(this.heartbeatIntervalId)
        await this.connection.stop()
    }

    onConnectionStateChanged(handler: ConnectionChangedHandler) {
        this.connectionChangedHandlers.push(handler)
    }

    addWorkOnConnected(handler: () => void) {
        if (this.isConnected) {
            handler()
        } else {
            setTimeout(() => this.addWorkOnConnected(handler), 1000)
        }
    }

    private async start() {
        let tryStart = true
        while (tryStart) {
            try {
                this.log(`signalr start, transport = ${this.getHttpTransportTypeName(this.transport)}`)
                // const connectionUrl = environment.IONIC ? new URL('/Hub', ionicBridge.host).toString() : '/Hub'
                const connectionUrl = '/Hub'
                // const connectionUrl = 'http://localhost:5231/Hub'
                // const connectionUrl = 'http://localhost:8080/Hub'
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl(connectionUrl, {
                        transport: this.transport,
                        skipNegotiation: this.transport === signalR.HttpTransportType.WebSockets,
                        accessTokenFactory: async () => {
                            this.log(`signalr start, accessTokenFactory, token = ${localStorage.getItem('token')}`)
                            return localStorage.getItem('token') || ''
                        },
                    })
                    .withAutomaticReconnect({
                        nextRetryDelayInMilliseconds: () => this.getRandomNumberInclusive(0.1, 2)
                    })
                    .configureLogging(signalR.LogLevel.Warning)
                    .build()

                await this.connection.start()
                tryStart = false
            } catch (err) {
                // if (err instanceof Error && err.message.includes('Unauthorized: Status code \'401\'')) {
                //     console.error('signalr start failed, the client is not authenticated, stop connection')
                // }
                console.error('signalr start failed', err)
                this.log('signalr start failed')

                this.transport =
                    this.transport === signalR.HttpTransportType.WebSockets
                        ? signalR.HttpTransportType.LongPolling
                        : signalR.HttpTransportType.WebSockets
                this.log(
                    `signalr start failed, try to change transport to ${this.getHttpTransportTypeName(this.transport)}`
                )

                await new Promise((r) => setTimeout(r, 1000))
            }
        }
    }

    private async checkConnectionHealth(connection: signalR.HubConnection, timeoutMs = 1500) {
        if (connection.state !== signalR.HubConnectionState.Connected) return false

        let timeoutHandle: ReturnType<typeof setTimeout>
        const timeoutPromise = new Promise((resolve) => {
            timeoutHandle = setTimeout(() => resolve(false), timeoutMs)
        })

        const pingPromise = connection
            .invoke('Ping')
            .then(() => true)
            .catch(() => false)
            .finally(() => clearTimeout(timeoutHandle))

        return Promise.race([timeoutPromise, pingPromise])
    }

    private startHeartbeat() {
        this.heartbeatIntervalId = setInterval(async () => {
            try {
                if (this.isConnected) {
                    await this.indexSignalrClientSendAdapter.heartbeat()
                }
            } catch (err) {
                console.error('signalr startHeartbeat error', err)
            }
        }, 1000)
    }

    private setConnectionState(isConnected: boolean) {
        if (this.isConnected !== isConnected) {
            if (this.isConnected) {
                this.log('sigalr connection changed: connected => disconnected')
            } else {
                this.log('sigalr connection changed: disconnected => connected')
            }

            for (let i = 0; i < this.connectionChangedHandlers.length; i++) {
                this.connectionChangedHandlers[i](this.isConnected, isConnected)
            }

            this.isConnected = isConnected
        }
    }

    private getRandomNumberInclusive(min: number, max: number) {
        // The maximum is inclusive and the minimum is inclusive
        // 四捨五入到小數點到第三位，產出結果可能是小數也可能是整數
        return (Math.random() * (max - min) + min) * 1000
    }

    private getHttpTransportTypeName(type: signalR.HttpTransportType) {
        switch (type) {
            case signalR.HttpTransportType.WebSockets:
                return 'WebSockets'
            case signalR.HttpTransportType.ServerSentEvents:
                return 'ServerSentEvents'
            case signalR.HttpTransportType.LongPolling:
                return 'LongPolling'
            default:
                return 'Unknown'
        }
    }

    private log(message: string) {
        console.debug(`[${new Date().toISOString()}] ${message}`)
    }
}

export default new SignalrAdapter()
