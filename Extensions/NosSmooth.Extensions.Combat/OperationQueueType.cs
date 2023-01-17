//
//  OperationQueueType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Extensions.Combat;

/// <summary>
/// A type classifying operations into multiple sequential queues.
/// </summary>
public enum OperationQueueType
{
    /// <summary>
    /// A total control of controls is needed (walking, sitting - recovering, using a skill).
    /// </summary>
    TotalControl,

    /// <summary>
    /// An item is being used.
    /// </summary>
    Item,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved1"/> operations.
    /// </summary>
    Reserved1,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved2"/> operations.
    /// </summary>
    Reserved2,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved3"/> operations.
    /// </summary>
    Reserved3,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved4"/> operations.
    /// </summary>
    Reserved4,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved5"/> operations.
    /// </summary>
    Reserved5,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved6"/> operations.
    /// </summary>
    Reserved6,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved7"/> operations.
    /// </summary>
    Reserved7,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved8"/> operations.
    /// </summary>
    Reserved8,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved9"/> operations.
    /// </summary>
    Reserved9,

    /// <summary>
    /// Any operation that should be executed sequentially with other <see cref="Reserved10"/> operations.
    /// </summary>
    Reserved10,
}