﻿// Copyright (c) Escendit Ltd. All Rights Reserved.
// Licensed under the MIT. See LICENSE.txt file in the solution root for full license information.

namespace Orleans.Hosting;

/// <summary>
/// Clustering Client Builder.
/// </summary>
public interface IClientClusteringBuilder : IClientBuilder
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; internal set; }
}
