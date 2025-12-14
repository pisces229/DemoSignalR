import './style.css'
import typescriptLogo from './typescript.svg'
import viteLogo from '/vite.svg'
import signalr from './signalrAdapter'
import apiService from './apiService'
import type { SignalrClientReceiveAdapter } from './signalrClientReceiveAdapter'
import type { ConnectionChangedHandler, SignalrReconnectionHandler } from './signalrReconnectionHandler'

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
  <div>
    <a href="https://vite.dev" target="_blank">
      <img src="${viteLogo}" class="logo" alt="Vite logo" />
    </a>
    <a href="https://www.typescriptlang.org/" target="_blank">
      <img src="${typescriptLogo}" class="logo vanilla" alt="TypeScript logo" />
    </a>
    <h1>Vite + TypeScript</h1>
    <div class="card">
      <button id="adminToken" type="button">Admin Token</button>
      <button id="userToken" type="button">User Token</button>
      <button id="expiredToken" type="button">Expired Token</button>
    </div>
    <div class="card">
      <button id="init" type="button">Init Signalr</button>
      <button id="stop" type="button">Stop Signalr</button>
    </div>
    <div class="card">
      <button id="send" type="button">Send</button>
      <button id="receive" type="button">Add Receive Handler</button>
      <button id="removeReceive" type="button">Remove Receive Handler</button>
    </div>
    <div class="card">
      <button id="sendDto" type="button">Send Dto</button>
      <button id="receiveDto" type="button">Add Receive Dto Handler</button>
      <button id="removeReceiveDto" type="button">Remove Receive Dto Handler</button>
    </div>
    <div class="card">
      <button id="authorize" type="button">Authorize</button>
    </div>
  </div>
`

localStorage.removeItem('token')

document.querySelector<HTMLButtonElement>('#adminToken')!.addEventListener('click', async () => {
  try {
    const result = await apiService.get<{ token: string }>('/api/adminToken')
    localStorage.setItem('token', result.data?.token as string)
    console.log('API GET result', result)
  } catch (error) {
    console.error('API GET failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#userToken')!.addEventListener('click', async () => {
  try {
    const result = await apiService.get<{ token: string }>('/api/userToken')
    localStorage.setItem('token', result.data?.token as string)
    console.log('API GET result', result)
  } catch (error) {
    console.error('API GET failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#expiredToken')!.addEventListener('click', async () => {
  try {
    const result = await apiService.get<{ token: string }>('/api/expiredToken')
    localStorage.setItem('token', result.data?.token as string)
    console.log('API GET result', result)
  } catch (error) {
    console.error('API GET failed', error)
  }
})

signalr.addWorkOnConnected(() => {
  // once connected, add work on connected
  console.log('signalr connected work on connected')
})

document.querySelector<HTMLButtonElement>('#init')!.addEventListener('click', async () => {
  try {
    await signalr.init()
    signalr.onConnectionStateChanged((oldState, newState) => {
      console.log('signalr connection state changed', oldState, newState)
    })
    const handler1 = (oldState: boolean, newState: boolean) => {
      console.log('(1) signalr connection state changed', oldState, newState)
    }
    const handler2 = (oldState: boolean, newState: boolean) => {
      console.log('(2) signalr connection state changed', oldState, newState)
    }
    // signalr.signalrReconnectionHandler.addHandler(handler1)
    // signalr.signalrReconnectionHandler.addHandler(handler2)
    // signalr.signalrReconnectionHandler.clearHandler()
    let { onMounted: onMounted1, onUnmounted: onUnmounted1 } = useSignalrReconnectionHandler(
      (signalrAdapter) => signalrAdapter.signalrReconnectionHandler, handler1)
    let { onMounted: onMounted2, onUnmounted: onUnmounted2 } = useSignalrReconnectionHandler(
      (signalrAdapter) => signalrAdapter.signalrReconnectionHandler, handler2)
    let onMounted = () => {
      onMounted1()
      onMounted2()
    }
    let onUnmounted = () => {
      onUnmounted1()
      onUnmounted2()
    }
    onMounted()
    console.log(onUnmounted)
  } catch (error) {
    console.error('signalr init failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#stop')!.addEventListener('click', async () => {
  try {
    await signalr.stop()
  } catch (error) {
    console.error('signalr stop failed', error)
  }
})


document.querySelector<HTMLButtonElement>('#send')!.addEventListener('click', async () => {
  try {
    let result = await signalr.indexSignalrClientSendAdapter.send('Hello, Signalr!')
    console.log('signalr send result', result)
  } catch (error) {
    console.error('signalr send failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#receive')!.addEventListener('click', () => {
  try {
    const handler1 = (message: string) => {
      console.log('(1) signalr received message', message)
    }
    const handler2 = (message: string) => {
      console.log('(2) signalr received message', message)
    }
    // signalr.indexSignalrClientReceiveAdapter?.receive?.addHandler(handler1)
    // signalr.indexSignalrClientReceiveAdapter?.receive?.addHandler(handler2)
    let { onMounted: onMounted1, onUnmounted: onUnmounted1 } = useSignalrClientReciverHandler(
      (signalrAdapter) => signalrAdapter.indexSignalrClientReceiveAdapter?.receive, handler1)
    let { onMounted: onMounted2, onUnmounted: onUnmounted2 } = useSignalrClientReciverHandler(
      (signalrAdapter) => signalrAdapter.indexSignalrClientReceiveAdapter?.receive, handler2)
    let onMounted = () => {
      onMounted1()
      onMounted2()
    }
    let onUnmounted = () => {
      onUnmounted1()
      onUnmounted2()
    }
    onMounted()
    document.querySelector<HTMLButtonElement>('#removeReceive')!.addEventListener('click', () => {
      try {
        onUnmounted()
      } catch (error) {
        console.error('signalr remove receive failed', error)
      }
    })
  } catch (error) {
    console.error('signalr receive failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#sendDto')!.addEventListener('click', async () => {
  try {
    let result = await signalr.indexSignalrClientSendAdapter.sendDto({ title: 'Hello', content: 'Signalr!' })
    console.log('signalr send dto result', result)
  } catch (error) {
    console.error('signalr send dto failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#receiveDto')!.addEventListener('click', () => {
  try {
    const handler1 = (dto: { title: string, content: string }) => {
      console.log('(1) signalr received dto', dto)
    }
    const handler2 = (dto: { title: string, content: string }) => {
      console.log('(2) signalr received dto', dto)
    }
    // signalr.indexSignalrClientReceiveAdapter?.receiveDto?.addHandler(handler1)
    // signalr.indexSignalrClientReceiveAdapter?.receiveDto?.addHandler(handler2)
    let { onMounted: onMounted1, onUnmounted: onUnmounted1 } = useSignalrClientReciverHandler(
      (signalrAdapter) => signalrAdapter.indexSignalrClientReceiveAdapter?.receiveDto, handler1)
    let { onMounted: onMounted2, onUnmounted: onUnmounted2 } = useSignalrClientReciverHandler(
      (signalrAdapter) => signalrAdapter.indexSignalrClientReceiveAdapter?.receiveDto, handler2)
    let onMounted = () => {
      onMounted1()
      onMounted2()
    }
    let onUnmounted = () => {
      onUnmounted1()
      onUnmounted2()
    }
    onMounted()
    document.querySelector<HTMLButtonElement>('#removeReceiveDto')!.addEventListener('click', () => {
      try {
        onUnmounted()
      } catch (error) {
        console.error('signalr remove receive dto failed', error)
      }
    })
  } catch (error) {
    console.error('signalr receive dto failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#authorize')!.addEventListener('click', async () => {
  try {
    await signalr.indexSignalrClientSendAdapter.authorize()
    console.log('signalr authorize success')
  } catch (error) {
    console.error('signalr authorize failed', error)
  }
})

function useSignalrReconnectionHandler(
  getClient: (signalrAdapter: typeof signalr) => SignalrReconnectionHandler | undefined,
  handler: ConnectionChangedHandler
) {
  return {
    onMounted: () => {
        signalr.addWorkOnConnected(() => {
            const client = getClient(signalr)
            client?.addHandler(handler)
        })
    },
    onUnmounted: () => {
        const client = getClient(signalr)
        client?.removeHandler(handler)
    }
  }
}

function useSignalrClientReciverHandler<T>(
  getClient: (signalrAdapter: typeof signalr) => SignalrClientReceiveAdapter<T> | undefined,
  handler: (input: T) => void
) {
  return {
    onMounted: () => {
        signalr.addWorkOnConnected(() => {
            const client = getClient(signalr)
            client?.addHandler(handler)
        })
    },
    onUnmounted: () => {
        const client = getClient(signalr)
        client?.removeHandler(handler)
    }
  }
}