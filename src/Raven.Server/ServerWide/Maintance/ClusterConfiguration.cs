﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Raven.Server.Config.Attributes;
using Raven.Server.Config.Categories;
using Raven.Server.Config.Settings;

namespace Raven.Server.ServerWide.Maintance
{
    public class 
        ClusterConfiguration : ConfigurationCategory
    {
        [Description("Timeout in which the node expects to recieve a heartbeat from the leader")]
        [DefaultValue(300)]
        [TimeUnit(TimeUnit.Milliseconds)]
        [ConfigurationEntry("Raven/Cluster/ElectionTimeout")]
        public TimeSetting ElectionTimeout { get; set; }

        [Description("How freuqent we sample the information about the databases and send it to the maintenancenet supervisor.")]
        [DefaultValue(250)]
        [TimeUnit(TimeUnit.Milliseconds)]
        [ConfigurationEntry("Raven/Cluster/NodeSamplePeriod")]
        public TimeSetting WorkerSamplePeriod { get; set; }

        [Description("As the maintenancenet supervisor, how freuqent we sample the information received from the nodes.")]
        [DefaultValue(500)]
        [TimeUnit(TimeUnit.Milliseconds)]
        [ConfigurationEntry("Raven/Cluster/LeaderSamplePeriod")]
        public TimeSetting SupervisorSamplePeriod { get; set; }

        [Description("As the maintenancenet supervisor, how long we wait to hear from a worker before it is timeouted.")]
        [DefaultValue(1000)]
        [TimeUnit(TimeUnit.Milliseconds)]
        [ConfigurationEntry("Raven/Cluster/RecieveFromNodeTimeout")]
        public TimeSetting RecieveFromNodeTimeout { get; set; }

        [Description("As the maintenancenet supervisor, how long we wait after we recived an exception from a worker. Before we retry.")]
        [DefaultValue(5000)]
        [TimeUnit(TimeUnit.Milliseconds)]
        [ConfigurationEntry("Raven/Cluster/OnErrorDelayTime")]
        public TimeSetting OnErrorDelayTime { get; set; }
    }
}
