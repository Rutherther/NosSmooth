//
//  NostaleDataFilesManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles;

/// <summary>
/// Nostale .NOS files manager.
/// </summary>
public class NostaleDataFilesManager
{
    private readonly NostaleDataParser _parser;
    private ILanguageService? _languageService;
    private IInfoService? _infoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleDataFilesManager"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public NostaleDataFilesManager(NostaleDataParser parser)
    {
        _parser = parser;
    }

    /// <summary>
    /// Gets the language service.
    /// </summary>
    public ILanguageService LanguageService
    {
        get
        {
            if (_languageService is null)
            {
                throw new InvalidOperationException
                    ("The language service is null, did you forget to call NostaleDataManager.Initialize?");
            }

            return _languageService;
        }
    }

    /// <summary>
    /// Gets the info service.
    /// </summary>
    public IInfoService InfoService
    {
        get
        {
            if (_infoService is null)
            {
                throw new InvalidOperationException
                    ("The info service is null, did you forget to call NostaleDataManager.Initialize?");
            }

            return _infoService;
        }
    }

    /// <summary>
    /// Initialize the info and language services.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Initialize()
    {
        if (_languageService is null)
        {
            var languageServiceResult = _parser.CreateLanguageService();
            if (!languageServiceResult.IsSuccess)
            {
                return Result.FromError(languageServiceResult);
            }
            _languageService = languageServiceResult.Entity;
        }

        if (_infoService is null)
        {
            var infoServiceResult = _parser.CreateInfoService();
            if (!infoServiceResult.IsSuccess)
            {
                return Result.FromError(infoServiceResult);
            }
            _infoService = infoServiceResult.Entity;
        }

        return Result.FromSuccess();
    }
}