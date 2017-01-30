﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Cache.Interfaces;
using Moq;
using Splitio.Domain;
using System.Collections.Generic;
using Newtonsoft.Json;
using Splitio.Services.Cache.Classes;
using StackExchange.Redis;


namespace Splitio_Tests.Unit_Tests.Cache
{
    [TestClass]
    public class RedisSplitCacheTests
    {
        private const string splitKeyPrefix = "SPLITIO.split.";
        private const string splitsKeyPrefix = "SPLITIO.splits.";

        [TestMethod]
        public void AddAndGetSplitTest()
        {
            //Arrange
            var splitName = "test_split";
            var split = new Split() { name = "test_split", changeNumber = 121291, killed = false, seed = 4324324, defaultTreatment = "on", conditions = new List<ConditionDefinition>(), status = StatusEnum.ACTIVE, trafficTypeName = "test" };
            var splitJson = JsonConvert.SerializeObject(split);
            var redisAdapterMock = new Mock<IRedisAdapter>();          
            redisAdapterMock.Setup(x => x.Set(splitKeyPrefix + "test_split", splitJson)).Returns(true);
            redisAdapterMock.Setup(x => x.Get(splitKeyPrefix + "test_split")).Returns(splitJson);
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            splitCache.AddSplit(splitName, split);
            var result = (Split)splitCache.GetSplit(splitName);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(splitName, result.name);
        }

        [TestMethod]
        public void AddDuplicateSplitTest()
        {
            //Arrange
            var splitName = "test_split";
            var split = new Split() { name = "test_split", changeNumber = 121291, killed = false, seed = 4324324, defaultTreatment = "on", conditions = new List<ConditionDefinition>(), status = StatusEnum.ACTIVE, trafficTypeName = "test" };
            var splitJson = JsonConvert.SerializeObject(split);
            var redisAdapterMock = new Mock<IRedisAdapter>();
            redisAdapterMock.Setup(x => x.Keys(splitKeyPrefix + "*")).Returns(new RedisKey[]{"test_split"});
            redisAdapterMock.Setup(x => x.Get(It.IsAny<RedisKey[]>())).Returns(new RedisValue[]{splitJson});
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            var split1 = new Split() { name = splitName };
            splitCache.AddSplit(splitName, split1);
            var split2 = new Split() { name = "test_split_2" };
            splitCache.AddSplit(splitName, split2);
            var result = splitCache.GetAllSplits();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(((Split)result[0]).name, split1.name);
            Assert.AreNotEqual(((Split)result[0]).name, split2.name);
        }

        [TestMethod]
        public void GetInexistentSplitTest()
        {
            //Arrange
            var splitName = "test_split";
            var redisAdapterMock = new Mock<IRedisAdapter>();
            string value = null;
            redisAdapterMock.Setup(x => x.Get(splitKeyPrefix + "test_split")).Returns(value);
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            var result = splitCache.GetSplit(splitName);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveSplitTest()
        {
            //Arrange
            var splitName = "test_split";
            var redisAdapterMock = new Mock<IRedisAdapter>();
            redisAdapterMock.Setup(x => x.Del(splitKeyPrefix + "test_split")).Returns(true);
            string value = null;
            redisAdapterMock.Setup(x => x.Get(splitKeyPrefix + "test_split")).Returns(value);
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            var isRemoved = splitCache.RemoveSplit(splitName);
            var result = splitCache.GetSplit(splitName);

            //Assert
            Assert.IsTrue(isRemoved);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SetAndGetChangeNumberTest()
        {
            //Arrange
            var changeNumber = 1234;
            var redisAdapterMock = new Mock<IRedisAdapter>();
            redisAdapterMock.Setup(x => x.Set(splitsKeyPrefix + "till", changeNumber.ToString())).Returns(true);
            redisAdapterMock.Setup(x => x.Get(splitsKeyPrefix + "till")).Returns(changeNumber.ToString());
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            splitCache.SetChangeNumber(changeNumber);
            var result = splitCache.GetChangeNumber();

            //Assert
            Assert.AreEqual(changeNumber, result);
        }

        [TestMethod]
        public void GetAllSplitsTest()
        {
            //Arrange
            var splitName = "test_split";
            var splitName2 = "test_split2";
            var split = new Split() { name = "test_split", changeNumber = 121291, killed = false, seed = 4324324, defaultTreatment = "on", conditions = new List<ConditionDefinition>(), status = StatusEnum.ACTIVE, trafficTypeName = "test" };
            var split2 = new Split() { name = "test_split2", changeNumber = 121291, killed = false, seed = 4324324, defaultTreatment = "on", conditions = new List<ConditionDefinition>(), status = StatusEnum.ACTIVE, trafficTypeName = "test" };
            var splitJson = JsonConvert.SerializeObject(split);
            var splitJson2 = JsonConvert.SerializeObject(split);
            var redisAdapterMock = new Mock<IRedisAdapter>();
            redisAdapterMock.Setup(x => x.Set(It.IsAny<string>(), splitJson)).Returns(true);
            redisAdapterMock.Setup(x => x.Set(It.IsAny<string>(), splitJson2)).Returns(true);
            redisAdapterMock.Setup(x => x.Keys(splitKeyPrefix + "*")).Returns(new RedisKey[] { "test_split", "test_split2" });
            redisAdapterMock.Setup(x => x.Get(It.IsAny<RedisKey[]>())).Returns(new RedisValue[] { splitJson, splitJson2 });
            var splitCache = new RedisSplitCache(redisAdapterMock.Object);

            //Act
            splitCache.AddSplit(splitName, new ParsedSplit() { name = splitName });
            splitCache.AddSplit(splitName2, new ParsedSplit() { name = splitName2 });

            var result = splitCache.GetAllSplits();

            //Assert
            Assert.AreEqual(2, result.Count);
        }
    
    }
}