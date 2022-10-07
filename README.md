# App to convert HTML to PDF

Uses [puppeteer-sharp](https://github.com/hardkoded/puppeteer-sharp) library.

Conversion from HTML to PDF itself is done inside separate microservice, which communicates with backend using GRPC.

Notifications for conversion status updates are send back to client using SignalR.

Backend uses background service which periodically fetches unprocessed jobs and sends them to conversion service.
 
Possible job status values: 
*  `ReceivedInputFile` (Received Input File) 
*  `InProgress` ("In Progress - conversion started, file is sent to conversion service) 
*  `Done` (Done) 
*  `Failed_ConversionServiceUnavailable` (Failed: Conversion Service Unavailable - in case conversion service is down), 
*  `Failed_GenericError` (Failed: Generic Error - pdf conversion failed for some reason)

When backend is stopped, jobs which have  `InProgress` status are reset back to `ReceivedInputFile`, which will allow them to be processed after backend is restarted.

Several instances of conversion service can be deployed behind load balancer (i.e. nginx), backend will talk to this balancer instead of conversion service directly.

## How to run

From `./HtmlConverter`, run
```
dotnet run
```

From `./HtmlConverter.ConversionService`, run
```
dotnet run
```

From `./HtmlConverter/ClientApp`, run
```
npm run dev
```

