# DemoSignalR

[SignalR](https://learn.microsoft.com/en-us/aspnet/signalr/)

[來玩玩即時互動App吧! ASP.NET Core SignalR](https://ithelp.ithome.com.tw/users/20109839/ironman/1606)

[signalr](https://www.npmjs.com/package/@microsoft/signalr)

[libman](https://learn.microsoft.com/en-us/aspnet/core/client-side/libman/libman-vs?view=aspnetcore-8.0)

[SignalR vs. WebSocket: Key differences and which to use](https://ably.com/topic/signalr-vs-websocket)

## backplane

[ASP.NET Core SignalR hosting and scaling](https://learn.microsoft.com/zh-tw/aspnet/core/signalr/scale?view=aspnetcore-8.0)

[backplane](https://ithelp.ithome.com.tw/articles/10188426)

[SqlServer](https://github.com/IntelliTect/IntelliTect.AspNetCore.SignalR.SqlServer)


## Nginx Load Balance
```bash
docker run -d --name nginx-signalr-loadbalancer -p 8080:80 -v ./nginx.conf:/etc/nginx/nginx.conf:ro nginx:latest
```