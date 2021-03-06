﻿using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Redis.Services.Cache.Interfaces;
using Splitio.Services.Shared.Interfaces;

namespace Splitio.Redis.Services.Cache.Classes
{
    public class RedisEventsCache : RedisCacheBase, ISimpleCache<Event>
    {
        private const string eventKeyPrefix = "events";
        private readonly string _machineName;
        private readonly string _machineIP;
        private readonly string _sdkVersion;

        public RedisEventsCache(IRedisAdapter redisAdapter, string machineName, string machineIP, string sdkVersion, string userPrefix = null)
            : base(redisAdapter, userPrefix)
        {
            _machineName = machineName;
            _machineIP = machineIP;
            _sdkVersion = sdkVersion;
        }

        public void AddItem(Event item)
        {
            var key = redisKeyPrefix + eventKeyPrefix;
            var eventJson = JsonConvert.SerializeObject(new
            {
                m = new { s = _sdkVersion, i = _machineIP, n = _machineName },
                e = item
            });
            redisAdapter.ListRightPush(key, eventJson);
        }
    }
}