using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BLL.Services.Advanced;

public interface IApplicationAdvancedService
{
}

public class ApplicationAdvancedService : IApplicationAdvancedService
{
    #region Fields

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Ctor

    public ApplicationAdvancedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region Methods

    #endregion
}