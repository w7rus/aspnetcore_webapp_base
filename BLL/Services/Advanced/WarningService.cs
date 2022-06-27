using System.Collections.Generic;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

public interface IWarningAdvancedService
{
    void Add(WarningModelResultEntry warningModelResultEntry);
    List<WarningModelResultEntry> GetAll();
}

public class WarningAdvancedService : IWarningAdvancedService
{
    #region Ctor

    public WarningAdvancedService(ILogger<WarningAdvancedService> logger)
    {
        _logger = logger;
        _warningModelResultEntries = new List<WarningModelResultEntry>();
    }

    #endregion

    #region Fields

    private readonly ILogger<WarningAdvancedService> _logger;
    private readonly List<WarningModelResultEntry> _warningModelResultEntries;

    #endregion

    #region Methods

    public void Add(WarningModelResultEntry warningModelResultEntry)
    {
        _warningModelResultEntries.Add(warningModelResultEntry);
    }

    public List<WarningModelResultEntry> GetAll()
    {
        return _warningModelResultEntries;
    }

    #endregion
}