﻿using Sparrow.Json;

namespace Raven.Server.Rachis
{
    public class AppendEntries 
    {
        public long Term { get; set; }
        public long PrevLogIndex { get; set; }
        public long PrevLogTerm { get; set; }
        public long LeaderCommit { get; set; }
        public int EntriesCount { get; set; }
    }

    public class RachisEntry
    {
        public long Term { get; set; }
        public long Index { get; set; }
        public BlittableJsonReaderObject Entry { get; set; }
        public RachisEntryFlags Flags { get; set; }
    }
}
