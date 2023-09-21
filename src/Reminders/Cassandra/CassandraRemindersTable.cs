// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Reminders.Cassandra;

using global::Orleans.Runtime;

/// <summary>
/// Cassandra Reminders Table.
/// </summary>
public class CassandraRemindersTable : IReminderTable
{
    /// <inheritdoc/>
    public Task Init()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ReminderTableData> ReadRows(GrainId grainId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ReminderTableData> ReadRows(uint begin, uint end)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ReminderEntry> ReadRow(GrainId grainId, string reminderName)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<string> UpsertRow(ReminderEntry entry)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> RemoveRow(GrainId grainId, string reminderName, string eTag)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task TestOnlyClearTable()
    {
        throw new NotImplementedException();
    }
}
