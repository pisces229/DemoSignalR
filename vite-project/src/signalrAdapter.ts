import * as signalR from '@microsoft/signalr'
import { type ConnectionChangedHandler, SignalrReconnectionHandler } from './signalrReconnectionHandler'
import { IndexSignalrClientReceiveAdapter } from './indexSignalrClientReceiveAdapter'
import { IndexSignalrClientSendAdapter } from './indexSignalrClientSendAdapter'

class SignalrAdapter {
    isInitialized = false
    isStopped = false
    isConnected = false
    isReStartStopped = false
    transport = signalR.HttpTransportType.WebSockets
    heartbeatIntervalId: ReturnType<typeof setInterval> | undefined = undefined
    connectionChangedHandlers: ConnectionChangedHandler[] = []

    connection!: signalR.HubConnection
    indexSignalrClientReceiveAdapter!: IndexSignalrClientReceiveAdapter
    indexSignalrClientSendAdapter!: IndexSignalrClientSendAdapter

    signalrReconnectionHandler!: SignalrReconnectionHandler

    initCallbacks: (() => void)[] = []

    async init() {
        await this.start()

        this.isInitialized = false
        this.isConnected = false
        this.isStopped = false
        this.isReStartStopped = false
        this.connectionChangedHandlers = []

        this.indexSignalrClientReceiveAdapter = new IndexSignalrClientReceiveAdapter(this.connection)
        this.indexSignalrClientSendAdapter = new IndexSignalrClientSendAdapter(this.connection)

        // 斷線後重新連上時處理
        this.signalrReconnectionHandler = new SignalrReconnectionHandler(this.connectionChangedHandlers)

        this.connection.onclose(async () => {
            this.log(`sigalr get onclose, current connected is ${this.isConnected}`)
            if (this.isConnected) {
                this.setConnectionState(false)
                await this.reStart()
            }
        })

        // 連線成功設定第一次連上線的工作和狀態
        this.startHeartbeat()
        this.isInitialized = true
        this.setConnectionState(true)

        document.addEventListener('visibilitychange', async () => {
            this.log(`visibilitychange, document.visibilityState = ${document.visibilityState}`)

            if (document.visibilityState === 'visible') {
                // 強制進行一次 ping，限制最多等 1.5 秒
                const isConnectionAlive = await this.checkConnectionHealth(this.connection, 1500)

                if (!isConnectionAlive) {
                    this.log(
                        `[${new Date().toISOString()}] visibilitychange, signalr connection is not alive, stop connection`
                    )
                    this.isReStartStopped = true
                    // 這裡只需要中斷，接著onclose會被呼叫，然後reStart
                    await this.connection.stop()
                } else {
                    this.log(`[${new Date().toISOString()}] visibilitychange, signalr connection is alive`)
                }
            }
        })
    }

    async stop() {
        this.isStopped = true
        clearInterval(this.heartbeatIntervalId)
        await this.connection.stop()
        this.setConnectionState(false)
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
                // const connectionUrl = '/Hub'
                const connectionUrl = 'http://localhost:5231/Hub'
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
                    .configureLogging(signalR.LogLevel.Warning)
                    .build()

                await this.connection.start()
                tryStart = false
            } catch (err) {
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

    private async reStartStop() {
        if (!this.isReStartStopped) {
            this.log('signalr reStartStop, stop connection')
            this.isReStartStopped = true
            try {
                await this.connection.stop()
            } catch (err) {
                console.error('signalr reStartStop failed', err)
                // 這裡應該要設回false，但是stop在未連線成功前重複呼叫兩次以上可能會一直錯誤，這個try catch也是避免這個情況，所以就不設回false
                // this.isReStartStopped = false
            }
        }
    }

    private async reStart() {
        if (this.isStopped) {
            return
        }

        try {
            this.log(
                `signalr reStart, current connection state: ${this.connection.state} , navigator.onLine: ${navigator.onLine}`
            )
            await this.reStartStop()
            await this.connection.start()
            this.setConnectionState(true)
        } catch (err) {
            console.error('signalr reStart failed, navigator.onLine:', navigator.onLine, err)
            this.setConnectionState(false)
            const retryTime = this.getRandomNumberInclusive(0.1, 2) * 1000
            this.log(`signalr retry start after ${retryTime}ms`)

            setTimeout(() => this.reStart(), retryTime)
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
        }, 300000)
    }

    private setConnectionState(isConnected: boolean) {
        if (this.isConnected !== isConnected) {
            if (this.isConnected) {
                this.log('sigalr connection changed: connected => disconnected')
            } else {
                this.log('sigalr connection changed: disconnected => connected')
                this.isReStartStopped = false
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
        return Math.round((Math.random() * (max - min) + min) * 1000) / 1000
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
