//
//  WalkCommandHandlerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.LocalClient.CommandHandlers.Walk
{
    /// <summary>
    /// Options for <see cref="PlayerWalkCommandHandler"/>.
    /// </summary>
    public class WalkCommandHandlerOptions
    {
        /// <summary>
        /// The command handler sleeps for this duration, then checks for new info. Unit is milliseconds.
        /// </summary>
        /// <remarks>
        /// The operation is done with a cancellation token, if there is an outer event, then it can be faster.
        /// Walk function is called again as well after this delay.
        /// </remarks>
        public int CheckDelay { get; set; } = 50;
    }
}
