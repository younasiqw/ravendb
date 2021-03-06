// -----------------------------------------------------------------------
//  <copyright file="DatabaseUpdateCommandHandler.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;

using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Database.Raft.Commands;
using Raven.Database.Raft.Util;
using Raven.Database.Server.Tenancy;
using Raven.Database.Util;
using Raven.Json.Linq;

namespace Raven.Database.Raft.Storage.Handlers
{
    public class DatabaseUpdateCommandHandler : CommandHandler<DatabaseUpdateCommand>
    {
        public DatabaseUpdateCommandHandler(DocumentDatabase database, DatabasesLandlord landlord)
            : base(database, landlord)
        {
        }

        public override void Handle(DatabaseUpdateCommand command)
        {
            command.Document.AssertClusterDatabase();

            var key = DatabaseHelper.GetDatabaseKey(command.Document.Id);

            var documentJson = Database.Documents.Get(key);
            if (documentJson != null)
            {
                var document = documentJson.DataAsJson.JsonDeserialization<DatabaseDocument>();
                if (document.IsClusterDatabase() == false)
                    throw new InvalidOperationException(string.Format("Local database '{0}' is not cluster-wide.", DatabaseHelper.GetDatabaseName(command.Document.Id)));
            }

            Landlord.Protect(command.Document);
            var json = RavenJObject.FromObject(command.Document);
            json.Remove("Id");

            Database.Documents.Put(key, null, json, new RavenJObject(), null);
        }
    }
}
