# Escendit.Orleans.Clustering.Cassandra

`Escendit.Orleans.Clustering.Cassandra` is a NuGet package that provides the ability to register a Clustering provider
for Orleans to use for Cluster coordination.

## Installation

To install `Escendit.Orleans.Persistence.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Persistence.Cassandra
```

## Usage

### Silo

#### Simple Usage
```csharp
Host
    .CreateDefaultBuilder()
    .UseOrleans(siloBuilder => siloBuilder
        .UseCassandraClustering()
        .WithClientAsDefault(...)
        .Build());
```

### Client

#### Simple Usage

```csharp
Host
    .CreateDefaultBuilder()
    .UseOrleansClient(clientBuilder => clientBuilder
        .UseCassandraClustering()
        .WithClientAsDefault(...)
        .Build());
```

## Contributing

If you'd like to contribute to [`cassandra-orleans-extensions`][self],
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.

[self]: https://github.com/escendit/cassandra-orleans-extensions
