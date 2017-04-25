﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing.Classes
{
    public class StartsWithMatcher : IMatcher
    {
        private HashSet<string> itemsToCompare = new HashSet<string>();

        public StartsWithMatcher(List<string> compareTo)
        {
            if (compareTo == null)
            {
                throw new ArgumentNullException("StartsWithMatcher does not allow null list as input.");
            }

            itemsToCompare.UnionWith(compareTo);
        }

        public bool Match(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return itemsToCompare.Any(i => key.StartsWith(i));
        }

        public bool Match(List<string> key)
        {
            return false;
        }

        public bool Match(DateTime key)
        {
            return false;
        }

        public bool Match(long key)
        {
            return false;
        }
    }
}