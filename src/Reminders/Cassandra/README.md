# Escendit.Orleans.Reminders.Cassandra

`Escendit.Orleans.Reminders.Cassandra` is a NuGet package that provides the ability to register a Grain Storage
provider for Orleans State Management.

## Installation

To install `Escendit.Orleans.Reminders.Cassandra`, run the following command in the Package Manager Console:

```powershell
Install-Package Escendit.Orleans.Reminders.Cassandra
```

## Usage

### Silo

#### Register with Default Client

⚠️ This method tries to create a new client with options, and may override your settings

```csharp
Host
    .CreateDefaultBuilder()
    .UseCassandraReminderService(...);
```

#### Register with Named Client

If you have multiple clients you can pick the client which you want to use

⚠️ This method tries to create a new client with options, and may override your settings

```csharp
Host
    .CreateDefaultBuilder()
    .UseCassandraReminderService("new_name", ...);
```

#### Use Existing Client

⚠️ If you already use client, you can reuse the client.

```csharp
Host
    .CreateDefaultBuilder()
    .UseCassandraReminderService("existing_client");
```

## Contributing

If you'd like to contribute to [`cassandra-orleans-extensions`][self],
please fork the repository and make changes as you'd like.
Pull requests are warmly welcome.

[self]: https://github.com/escendit/cassandra-orleans-extensions
