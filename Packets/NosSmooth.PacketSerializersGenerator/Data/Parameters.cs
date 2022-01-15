//
//  Parameters.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializersGenerator.AttributeGenerators;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.Data;

/// <summary>
/// Contains set of parameters of a packet.
/// </summary>
public class Parameters
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parameters"/> class.
    /// </summary>
    /// <param name="parameters">The list of the parameters.</param>
    public Parameters(IReadOnlyList<ParameterInfo> parameters)
    {
        List = parameters;
    }

    /// <summary>
    /// Gets the list of the parameters.
    /// </summary>
    public IReadOnlyList<ParameterInfo> List { get; }

    /// <summary>
    /// Gets the current index of the parameter.
    /// </summary>
    public int CurrentIndex { get; set; }

    /// <summary>
    /// Gets the currently processing parameter.
    /// </summary>
    public ParameterInfo Current => List[CurrentIndex];

    /// <summary>
    /// Gets the next processing parameter.
    /// </summary>
    public ParameterInfo? Next => CurrentIndex < List.Count - 1 ? List[CurrentIndex + 1] : null;

    /// <summary>
    /// Gets the previous processing parameter.
    /// </summary>
    public ParameterInfo? Previous => CurrentIndex > 0 ? List[CurrentIndex - 1] : null;

    /// <summary>
    /// Gets whether the current parameter is the last one.
    /// </summary>
    public bool IsLast => CurrentIndex == List.Count - 1 || IsRestOptionals();

    /// <summary>
    /// Gets whether the current parameter is the first one.
    /// </summary>
    public bool IsFirst => CurrentIndex == 0;

    private bool IsRestOptionals()
    {
        for (int i = CurrentIndex + 1; i < List.Count; i++)
        {
            if (!List[i].IsOptional() && List[i].Attributes.All(x => x.FullName != PacketConditionalIndexAttributeGenerator.PacketConditionalIndexAttributeFullName))
            {
                return false;
            }
        }

        return true;
    }
}