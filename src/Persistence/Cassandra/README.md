# Escendit.Orleans.Persistence.Cassandra

`Escendit.Orleans.Persistence.Cassandra` is a NuGet package that provides the ability to register a Grain Storage
provider for Orleans State Management.

## Installation

To install `Escendit.Orleans.Persistence.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Persistence.Cassandra
```

## Usage

### Silo

#### Simple Usage

##### Default

```csharp
Host
    .CreateDefaultBuilder()
    .ConfigureServices(services => services
        .AddCassandraClientAsDefault(...));
    .UseOrleans(siloBuilder => siloBuilder
        .AddCassandraGrainStorageAsDefault(...));
```

##### Named

```csharp
Host
    .CreateDefaultBuilder()
    .ConfigureServices(services => services
        .AddCassandraClient("name", ...));
    .UseOrleans(siloBuilder => siloBuilder
        .AddCassandraGrainStorage("name", ...));
```

## Contributing

If you'd like to contribute to [`cassandra-orleans-extensions`][self],
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.

[self]: https://github.com/escendit/cassandra-orleans-extensions
