using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversUtils
{


        public class FakeConnection : NetworkConnectionToClient
        {
            public FakeConnection(int connectionId) : base(connectionId)
            {
            }

            public override string address => "localhost";

            public override void Send(ArraySegment<byte> segment, int channelId = 0)
            {
            }
        }

}
