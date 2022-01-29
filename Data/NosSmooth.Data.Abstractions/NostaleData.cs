//
//  NostaleData.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Abstractions;

public record NostaleData
(
    IReadOnlyDictionary<Language.Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>> Translations,
    IReadOnlyDictionary<int, IItemInfo> Items,
    IReadOnlyDictionary<int, IMonsterInfo> Monsters,
    IReadOnlyDictionary<int, ISkillInfo> Skills,
    IReadOnlyDictionary<int, IMapInfo> Maps
);