// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Escendit.Orleans.Persistence.Cassandra.Tests.Grains;

/// <summary>
/// Simple Integer Grain Interface.
/// </summary>
public interface ISimpleIntegerGrain : IGrainWithIntegerKey, ITestGrain
{
}
