﻿//
//       Copyright (C) DataStax Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dse.ProtocolEvents.Internal
{
    /// <summary>
    /// Used internally by <see cref="ProtocolEventDebouncer"/>.
    /// </summary>
    internal class EventQueue
    {
        public volatile ProtocolEvent MainEvent;

        public IList<TaskCompletionSource<bool>> Callbacks { get; } = new List<TaskCompletionSource<bool>>();

        public IDictionary<string, KeyspaceEvents> Keyspaces { get; } = new Dictionary<string, KeyspaceEvents>();
    }
}