//
//  WalkCommandHandlerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosSmooth.LocalClient.CommandHandlers.Walk
{
    /// <summary>
    /// Options for <see cref="WalkCommandHandler"/>.
    /// </summary>
    public class WalkCommandHandlerOptions
    {
        /// <summary>
        /// After what time to trigger not walking for too long error in milliseconds.
        /// </summary>
        /// <remarks>
        /// Use at least 2000 to avoid problems with false triggers.
        /// </remarks>
        public int NotWalkingTooLongTrigger { get; set; } = 2000;

        /// <summary>
        /// The command handler sleeps for this duration, then checks new info in milliseconds.
        /// </summary>
        /// <remarks>
        /// The operation is done with a cancellation token, if there is an outer event, then it can be faster.
        /// Walk function is called again as well after this delay.
        /// </remarks>
        public int CheckDelay { get; set; } = 100;
    }
}
