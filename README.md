# ALPS.NET

This project demonstrates how [ALPS](https://dlteam.io/ALPS.pdf) works using UDP-channels.
It uses a [Noise Protocol Framework](http://www.noiseprotocol.org/) implementation which can be found [here](https://github.com/Metalnem/noise). 

**ALPS.Server** contains the server implementation which accepts an encrypted and authenticated 0-RTT request message from the cient and sends a response message back.

**ALPS.Client** contains the client implemetation which plays the counterpart.

**ALPS.Shard** contains shared keys and listening-ports.

**ALPS.KeyGenerator** contains a key-generator which can be used to generate key-pairs and PSKs.

For demonstration-purposes client- and server will communicate using the loopback interface (localhost).

## Build and run

Use a terminal of your choice.

```
dotnet build
```

```
`dotnet <path to startup-dll>
// e.g.: dotnet .\ALPS.KeyGenerator\bin\Debug\netcoreapp2.1\ALPS.KeyGenerator.dll
```

REMARK: It's necessary to start the server first.