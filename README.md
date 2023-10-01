# Orleans Cassandra Extensions

Orleans Cassandra &amp; ScyllaDB (or any other database implementing CQL specification) Extensions allow you to easily configure
and manage providers for:
- Grain Storage
- Clustering
- Reminders

## Installation

Choose from three `Cassandra` provider types:

### [Grain Storage][grain-storage]

To install `Escendit.Orleans.Persistence.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Persistence.Cassandra
```

### [Clustering][clustering]

To install `Escendit.Orleans.Clustering.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Clustering.Cassandra
```

### [Reminders][reminders]

To install `Escendit.Orleans.Reminders.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Reminders.Cassandra
```

## Contributing

If you'd like to contribute to [`cassandra-orleans-extensions`][self],
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.

[self]: https://github.com/escendit/cassandra-orleans-extensions
[grain-storage]: src/Persistence/Cassandra
[clustering]: src/Clustering/Cassandra
[reminders]: src/Reminders/Cassandra
