using System;
using System.Linq;
using System.Reflection;
using BLL.Handlers.Base;
using Common.Attributes;
using Common.Exceptions;
using Common.Extensions;
using Common.Models;
using Common.Models.Base;
using Domain.Entities.Base;
using DTO.Models.DomainInfo;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers.App;

public interface IDomainInfoHandler
{
    IDtoResultBase Read(DomainInfoReadDto data);
    IDtoResultBase ReadAssemblyQualifiedNames();
}

public class DomainInfoHandler : HandlerBase, IDomainInfoHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;

    #endregion

    #region Controller

    public DomainInfoHandler(ILogger<HandlerBase> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Methods

    public IDtoResultBase Read(DomainInfoReadDto data)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        var type = Type.GetType(data.AssemblyQualifiedName);
        if (type == null || !type.Namespace!.StartsWith(Consts.DomainNamespace))
            throw new CustomException(Localize.Error.TypeNotFound);

        var properties = type?.GetProperties();

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

        return new DomainInfoReadResultDto
        {
            PropertiesValueTypes = properties
                ?.Where(_ =>
                    _.GetCustomAttribute<AllowFilterExpressionAttribute>(true) != null ||
                    _.GetCustomAttribute<AllowFilterSortAttribute>(true) != null)
                .ToDictionary(_ => _.Name, __ => __.GetValueType())
        };
    }

    public IDtoResultBase ReadAssemblyQualifiedNames()
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadAssemblyQualifiedNames)));

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadAssemblyQualifiedNames)));

        return new DomainInfoReadAssemblyQualifiedNamesResultDto
        {
            AssemblyQualifiedNames = typeof(EntityBase<Guid>).Assembly.GetTypes()
                .Where(_ => _.Namespace != null && _.Namespace.StartsWith(Consts.DomainNamespace))
                .Select(_ => _.AssemblyQualifiedName)
        };
    }

    #endregion
}