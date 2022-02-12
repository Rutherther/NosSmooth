//
//  Health.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents the health or mana of an entity.
/// </summary>
public class Health
{
    private byte? _percentage;
    private long? _amount;
    private long? _maximum;

    /// <summary>
    /// Gets or sets the percentage of the health.
    /// </summary>
    public byte? Percentage
    {
        get => _percentage;
        set
        {
            _percentage = value;
            if (value is null)
            {
                return;
            }

            var maximum = _maximum;
            if (maximum is not null)
            {
                _amount = (long)((value / 100.0) * maximum);
                return;
            }

            var amount = _amount;
            if (amount is not null)
            {
                _maximum = (long)(amount / (value / 100.0));
            }
        }
    }

    /// <summary>
    /// Gets or sets the health amount.
    /// </summary>
    public long? Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            if (value is null)
            {
                return;
            }

            var maximum = _maximum;
            if (maximum is not null)
            {
                _percentage = (byte)(((double)value / maximum) * 100);
                return;
            }

            var percentage = _percentage;
            if (percentage is not null)
            {
                _maximum = (long)(value / (percentage / 100.0));
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum health.
    /// </summary>
    public long? Maximum
    {
        get => _maximum;
        set
        {
            _maximum = value;
            if (value is null)
            {
                return;
            }

            var amount = _amount;
            var percentage = _percentage;

            if (amount is not null)
            {
                if (amount > value)
                {
                    amount = _amount = value;
                    _percentage = 100;
                    return;
                }

                _percentage = (byte)((amount / (double)value) * 100);
                return;
            }

            if (percentage is not null)
            { // ? would this be correct?
                _amount = (long)((percentage / 100.0) * value);
            }
        }
    }
}