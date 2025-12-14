import './style.css'
import typescriptLogo from './typescript.svg'
import viteLogo from '/vite.svg'
import signalr from './signalrAdapter'
import apiService from './apiService'

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
      <button id="send" type="button">Send Message</button>
      <button id="receive" type="button">Add Receive Handler</button>
      <button id="removeReceive" type="button">Remove Receive Handler</button>
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
    signalr.signalrReconnectionHandler.addHandler((oldState, newState) => {
      console.log('(1) signalr connection state changed', oldState, newState)
    })
    signalr.signalrReconnectionHandler.addHandler((oldState, newState) => {
      console.log('(2) signalr connection state changed', oldState, newState)
    })
    // signalr.signalrReconnectionHandler.clearHandler()
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
    signalr.indexSignalrClientReceiveAdapter?.receive?.addHandler((message) => {
      console.log('(1) signalr received message', message)
    })
    signalr.indexSignalrClientReceiveAdapter?.receive?.addHandler((message) => {
      console.log('(2) signalr received message', message)
    })
  } catch (error) {
    console.error('signalr receive failed', error)
  }
})

document.querySelector<HTMLButtonElement>('#removeReceive')!.addEventListener('click', () => {
  try {
    signalr.indexSignalrClientReceiveAdapter?.receive?.clearHandler()
  } catch (error) {
    console.error('signalr remove receive failed', error)
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