//
//      Copyright (C) DataStax Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Cassandra.Connections;
using Cassandra.ProtocolEvents;
using Cassandra.Serialization;
using Cassandra.SessionManagement;

using Moq;

namespace Cassandra.Tests.Connections
{
    internal class FakeControlConnectionFactory : IControlConnectionFactory
    {
        public IControlConnection Create(
            IInternalCluster cluster,
            IProtocolEventDebouncer protocolEventDebouncer,
            ProtocolVersion initialProtocolVersion,
            Configuration config,
            Metadata metadata,
            IEnumerable<object> contactPoints)
        {
            var cc = Mock.Of<IControlConnection>();
            Mock.Get(cc).Setup(c => c.InitAsync()).Returns(Task.Run(() =>
            {
                var cps = new Dictionary<string, IEnumerable<IPEndPoint>>();

                foreach (var cp in contactPoints)
                {
                    if (cp is string cpStr && IPAddress.TryParse(cpStr, out var addr))
                    {
                        var host = metadata.AddHost(new IPEndPoint(addr, config.ProtocolOptions.Port));
                        host.SetInfo(BuildRow());
                        Mock.Get(cc).Setup(c => c.Host).Returns(host);
                        cps.Add(cpStr, new List<IPEndPoint> { host.Address });
                    }
                    else if (cp is IPEndPoint endpt)
                    {
                        var host = metadata.AddHost(endpt);
                        host.SetInfo(BuildRow());
                        Mock.Get(cc).Setup(c => c.Host).Returns(host);
                        cps.Add(cp.ToString(), new List<IPEndPoint> { endpt });
                    }
                }
                metadata.SetResolvedContactPoints(cps);
            }));
            Mock.Get(cc).Setup(c => c.Serializer).Returns(new SerializerManager(ProtocolVersion.V3));
            return cc;
        }

        private IRow BuildRow(Guid? hostId = null)
        {
            return new TestHelper.DictionaryBasedRow(new Dictionary<string, object>
            {
                { "host_id", hostId ?? Guid.NewGuid() },
                { "data_center", "dc1"},
                { "rack", "rack1" },
                { "release_version", "3.11.1" },
                { "tokens", new List<string> { "1" }}
            });
        }
    }
}