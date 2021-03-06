# C# Client SDK samples

`C# DelveClientSDK` allows users to run Delve logic against local Delve servers or hosted servers in the cloud using `C#`.

## Getting started

First we need to install `Dotnet core 3.1` [dotnet-core-3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) available for Windows, Linux and OSX.

## Usage
### Dependencies
In order to perform local tests using `C# Client SDK`, we need to run Delve server locally
```
julia -O1
julia> using Pkg
julia> using Revise
julia> Pkg.activate(".")
julia> # to start Delve server
julia> include("./incubator/ClientSDKs/start-delve-server.jl")
julia> # to stop Delve server
julia> include("./incubator/ClientSDKs/stop-delve-server.jl")
```
### Building C# Client SDK and running samples
```
cd ./incubator/ClientSDKs/csharp-client-sdk
# to build the SDK
dotnet build
# to run tests
dotnet test
# to run samples
dotnet run --project DelveClientSDKSamples/DelveClientSDKSamples.csproj
```
