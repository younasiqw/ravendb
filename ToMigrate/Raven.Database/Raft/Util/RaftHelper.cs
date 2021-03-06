// -----------------------------------------------------------------------
//  <copyright file="RaftHelper.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Rachis;
using Rachis.Commands;
using Rachis.Transport;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Database.Config;
using Raven.Database.Util;

namespace Raven.Database.Raft.Util
{
    public static class RaftHelper
    {
        public static bool IsActive(this ClusterManager engine)
        {
            if (engine == null) 
                return false;

            return engine.Engine.CurrentTopology.AllNodes.Any();
        }

        public static NodeConnectionInfo GetLeaderNode(this RaftEngine engine, int waitTimeoutInSeconds = 0)
        {
            if (waitTimeoutInSeconds > 0)
                engine.WaitForLeader(waitTimeoutInSeconds * 1000);

            var leader = engine.CurrentLeader;
            if (leader == null)
            {
                if (waitTimeoutInSeconds > 0)
                    throw new InvalidOperationException(string.Format("No leader. Waited {0} seconds.", waitTimeoutInSeconds));

                return null;
            }
                

            return engine.CurrentTopology.AllNodes.Single(x => x.Name == leader);
        }

        public static bool IsLeader(this ClusterManager engine)
        {
            return engine.Engine.State == RaftEngineState.Leader;
        }

        public static bool IsClusterDatabase(this DocumentDatabase database)
        {
            if (database.IsSystemDatabase())
                return false;

            return database.Configuration.Cluster.NonClusterDatabaseMarker == false;
        }

        public static bool IsClusterDatabase(this DatabaseDocument document)
        {
            string value;
            if (document.Settings.TryGetValue(RavenConfiguration.GetKey(x => x.Cluster.NonClusterDatabaseMarker), out value) == false)
                return true;

            bool result;
            if (bool.TryParse(value, out result) == false) 
                return true;

            if (result) 
                return false;

            return true;
        }

        public static void AssertClusterDatabase(this DatabaseDocument document)
        {
            if (document.IsClusterDatabase() == false)
                throw new InvalidOperationException("Not a cluster database. Database: " + document.Id);
        }

        public static string GetNormalizedNodeUrl(string url)
        {
            return GetNodeUrl(url).AbsoluteUri.ToLower();
        }

        public static Uri GetNodeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (url.EndsWith("/") == false && url.EndsWith("\\") == false)
                url += "/";

            return new Uri(url, UriKind.Absolute);
        }

        public static string GetNodeName(Guid name)
        {
            return name.ToString();
        }

        /// <summary>
        /// Purpose of this methos is to detect if previous topo has difference with new topo, but node type(voting, promotable) is ignored.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool HasDifferentNodes(TopologyChangeCommand command)
        {
            if (command.Previous == null && command.Requested == null)
                return false;
            if (command.Previous == null || command.Requested == null)
                return true;
            var prevAllNodes = command.Previous.AllNodes.ToHashSet();
            var requestedAllNodes = command.Requested.AllNodes.ToHashSet();
            if (prevAllNodes.Count != requestedAllNodes.Count)
                return true;
            prevAllNodes.SymmetricExceptWith(requestedAllNodes);
            return prevAllNodes.Any();
        }
    }
}
