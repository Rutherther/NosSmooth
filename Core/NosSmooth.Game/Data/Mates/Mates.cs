//
//  Mates.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;
using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Data.Mates;

/// <summary>
/// Game mates state.
/// </summary>
public class Mates : IEnumerable<Mate>
{
    private ConcurrentDictionary<long, Partner> _partners;
    private ConcurrentDictionary<long, Pet> _pets;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mates"/> class.
    /// </summary>
    public Mates()
    {
        _partners = new ConcurrentDictionary<long, Partner>();
        _pets = new ConcurrentDictionary<long, Pet>();
    }

    /// <summary>
    /// Gets all of the partners belonging to the character.
    /// </summary>
    public IEnumerable<Partner> Partners => _partners.Values;

    /// <summary>
    /// Gets all of the pets belonging to the character.
    /// </summary>
    public IEnumerable<Pet> Pets => _pets.Values;

    /// <summary>
    /// Gets the current skill of pet, if there is any.
    /// </summary>
    public Skill? PetSkill { get; internal set; }

    /// <summary>
    /// Get sthe current skills of partner(' sp).
    /// </summary>
    public IReadOnlyList<Skill>? PartnerSkills { get; internal set; }

    /// <summary>
    /// Gets the current pet of the client.
    /// </summary>
    public PartyPet? CurrentPet { get; internal set; }

    /// <summary>
    /// Gets the current partner of the client.
    /// </summary>
    public PartyPartner? CurrentPartner { get; internal set; }

    /// <inheritdoc />
    public IEnumerator<Mate> GetEnumerator()
        => _partners.Values.Cast<Mate>().Concat(_pets.Values.Cast<Mate>()).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Sets pet of the given id.
    /// </summary>
    /// <param name="pet">The pet.</param>
    internal void SetPet(Pet pet)
    {
        _pets[pet.MateId] = pet;
    }

    /// <summary>
    /// Sets partner of the given id.
    /// </summary>
    /// <param name="partner">The partner.</param>
    internal void SetPartner(Partner partner)
    {
        _partners[partner.MateId] = partner;
    }

    /// <summary>
    /// Clears partners and pets.
    /// </summary>
    internal void Clear()
    {
        _partners.Clear();
        _pets.Clear();
    }
}