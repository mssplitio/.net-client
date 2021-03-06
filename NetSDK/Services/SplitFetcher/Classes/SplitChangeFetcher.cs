﻿using Common.Logging;
using Splitio.Domain;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Threading.Tasks;

namespace Splitio.Services.SplitFetcher.Classes
{
    public abstract class SplitChangeFetcher : ISplitChangeFetcher
    {
        private SplitChangesResult splitChanges;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitChangeFetcher));

        protected abstract Task<SplitChangesResult> FetchFromBackend(long since);

        public async Task<SplitChangesResult> Fetch(long since)
        {
            try
            {
                splitChanges = await FetchFromBackend(since);
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught executing Fetch since={0}", since), e);
                splitChanges = null; 
            }                   
            return splitChanges;
        }
    }
}
