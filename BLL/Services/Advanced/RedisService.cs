using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace BLL.Services.Advanced;

public interface IRedisService
{
    IDatabase GetDatabase();
    void SetItem<TEntity>(string key, TEntity value);
    (bool isNullOrEmpty, TEntity entity) GetItem<TEntity>(string key);
    ValueTask RemoveKeysByPattern(RedisValue serverGlob, Regex clientRegex = null, int database = -1, int batchSize = 1024);
}

public class RedisService : IRedisService
{
    #region Fields
    
    private readonly IDatabase _database;
    private readonly ConnectionMultiplexer _connectionMultiplexer;

    #endregion

    #region Ctor

    public RedisService(IOptions<RedisOptions> redisOptions)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(redisOptions.Value.ConnectionString);
        _database = _connectionMultiplexer.GetDatabase();
    }

    #endregion

    #region Methods

    public IDatabase GetDatabase() => _database;

    public void SetItem<TEntity>(string key, TEntity value)
    {
        _database.StringSet(key, JsonSerializer.Serialize(value));
    }

    public (bool isNullOrEmpty, TEntity entity) GetItem<TEntity>(string key)
    {
        var redisResult = _database.StringGet(key);
        return redisResult.IsNullOrEmpty ? (true, default) : (false, JsonSerializer.Deserialize<TEntity>(redisResult));
    }

    public async ValueTask RemoveKeysByPattern(RedisValue serverGlob, Regex clientRegex = null, int database = -1, int batchSize = 1024)
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        var db = _connectionMultiplexer.GetDatabase(database);
        var batch = new List<RedisKey>(batchSize);

        foreach (var endpoint in endpoints)
        {
            var server = _connectionMultiplexer.GetServer(endpoint);

            if (!server.IsConnected || server.IsReplica) continue;
            
            await foreach (var key in server.KeysAsync(database, serverGlob, 1024).ConfigureAwait(false))
            {
                if (clientRegex is not null && !clientRegex.IsMatch(key)) continue;
                    
                // have match; flush if we've hit the batch size
                batch.Add(key);
                if (batch.Count == batchSize) await FlushBatch().ConfigureAwait(false);
            }
                
            // make sure we flush per-server so we don't cross shards
            await FlushBatch().ConfigureAwait(false);
        }
        
        Task FlushBatch()
        {
            if (batch.Count == 0) return Task.CompletedTask;
            var keys = batch.ToArray();
            batch.Clear();
            return db.KeyDeleteAsync(keys);
        }
    }

    #endregion
}