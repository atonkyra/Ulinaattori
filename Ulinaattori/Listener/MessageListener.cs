using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Ulinaattori.Listener {
    class MessageListener {
        private SubscriberSocket ss = null;
        private NetMQPoller poller;
        public MessageListener(string zmqURL) {
            this.ss = new SubscriberSocket(">"+zmqURL);
            this.ss.SubscribeToAnyTopic();
            ss.ReceiveReady += onReceiveReady;

            this.poller = new NetMQPoller();
            this.poller.Add(this.ss);
            this.poller.RunAsync();

        }

        public void shutdown() {
            this.poller.Stop();
            this.ss.Close();
        }

        private void onReceiveReady(object sender, NetMQ.NetMQSocketEventArgs e) {
            string topic, jsonmsg;
            var msglist = e.Socket.ReceiveMultipartStrings(2);
            topic = msglist[0];
            jsonmsg = msglist[1];
            JObject jso = JObject.Parse(jsonmsg);
            switch(topic) {
                default:
                    handleDefaultMessage(topic, jso);
                    break;
            }
        }

        private void handleDefaultMessage(string topic, JObject jso) {
            JToken innerJSON;
            if(jso.TryGetValue("json", out innerJSON)) {
                onJSONMessage(topic, jso);
            } else if(jso.TryGetValue("text", out innerJSON)) {
                onPlainMessage(topic, innerJSON.ToString());
            }
        }

        public event onPlainMessageHandler onPlainMessage;
        public delegate void onPlainMessageHandler(string topic, string message);

        public event onJSONMessageHandler onJSONMessage;
        public delegate void onJSONMessageHandler(string topic, JObject message);
    }
}
