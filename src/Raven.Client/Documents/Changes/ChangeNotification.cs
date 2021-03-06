// -----------------------------------------------------------------------
//  <copyright file="ChangeNotification.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Sparrow.Json;
using Sparrow.Json.Parsing;

namespace Raven.Client.Documents.Changes
{
    public class DocumentChange : DatabaseChange
    {
        /// <summary>
        /// Type of change that occurred on document.
        /// </summary>
        public DocumentChangeTypes Type { get; set; }

        /// <summary>
        /// Identifier of document for which notification was created.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Document collection name.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Document type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Document change vector
        /// </summary>
        public string ChangeVector { get; set; } 

        internal bool TriggeredByReplicationThread;

        public override string ToString()
        {
            return string.Format("{0} on {1}", Type, Id);
        }

        public DynamicJsonValue ToJson()
        {
            return new DynamicJsonValue
            {
                [nameof(Type)] = Type.ToString(),
                [nameof(Id)] = Id,
                [nameof(CollectionName)] = CollectionName,
                [nameof(TypeName)] = TypeName,
                [nameof(ChangeVector)] = ChangeVector
            };
        }

        internal static DocumentChange FromJson(BlittableJsonReaderObject value)
        {
            value.TryGet(nameof(CollectionName), out string collectionName);
            value.TryGet(nameof(ChangeVector), out string changeVector);
            value.TryGet(nameof(TypeName), out string typeName);
            value.TryGet(nameof(Id), out string id);
            value.TryGet(nameof(Type), out string type);

            return new DocumentChange
            {
                CollectionName = collectionName,
                ChangeVector = changeVector,
                Id = id,
                TypeName = typeName,
                Type = (DocumentChangeTypes)Enum.Parse(typeof(DocumentChangeTypes), type, ignoreCase: true)
            };
        }
    }

    [Flags]
    public enum DocumentChangeTypes
    {
        None = 0,

        Put = 1,
        Delete = 2,
        BulkInsertStarted = 4,
        BulkInsertEnded = 8,
        BulkInsertError = 16,
        DeleteOnTombstoneReplication = 32,
        Conflict = 64,
        Common = Put | Delete
    }

    [Flags]
    public enum IndexChangeTypes
    {
        None = 0,

        BatchCompleted = 1,

        IndexAdded = 8,
        IndexRemoved = 16,

        IndexDemotedToIdle = 32,
        IndexPromotedFromIdle = 64,

        IndexDemotedToDisabled = 256,

        IndexMarkedAsErrored = 512,

        SideBySideReplace = 1024,

        Renamed = 2048,
        IndexPaused = 4096,
        LockModeChanged = 8192,
        PriorityChanged = 16384
    }

    public class IndexChange : DatabaseChange
    {
        /// <summary>
        /// Type of change that occurred on index.
        /// </summary>
        public IndexChangeTypes Type { get; set; }

        /// <summary>
        /// Name of index for which notification was created
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0} on {1}", Type, Name);
        }

        public DynamicJsonValue ToJson()
        {
            return new DynamicJsonValue
            {
                [nameof(Name)] = Name,
                [nameof(Type)] = Type.ToString()
            };
        }

        internal static IndexChange FromJson(BlittableJsonReaderObject value)
        {
            value.TryGet(nameof(Name), out string name);
            value.TryGet(nameof(Type), out string type);

            return new IndexChange
            {
                Type = (IndexChangeTypes)Enum.Parse(typeof(IndexChangeTypes), type, ignoreCase: true),
                Name = name
            };
        }
    }

    public class IndexRenameChange : IndexChange
    {
        /// <summary>
        /// The old index name
        /// </summary>
        public string OldIndexName { get; set; }
    }

    public class TrafficWatchChange : DatabaseChange
    {
        public DateTime TimeStamp { get; set; }
        public int RequestId { get; set; }
        public string HttpMethod { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public int ResponseStatusCode { get; set; }
        public string RequestUri { get; set; }
        public string AbsoluteUri { get; set; }
        public string DatabaseName { get; set; }
        public string CustomInfo { get; set; }
        public int InnerRequestsCount { get; set; }
        public object QueryTimings { get; set; }
    }
}
