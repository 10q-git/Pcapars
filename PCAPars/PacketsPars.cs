using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using SharpPcap.LibPcap;
using LightInject;

namespace PacketsPars
{

    class FileOpen
    {

        public FileOpen(string fileName)
        {
            FileName = fileName;
        }

        public void Open(string FileName)
        {
            // Create the offline device
            OfflinePacketDevice selectedDevice = new OfflinePacketDevice(FileName);

            // Open the capture file
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                // Read and dispatch packets until EOF is reached
                IoC.Registration(Container);
                communicator.ReceivePackets(0, DispatcherHandler);
            }

        }

        public static Dictionary<int, Dictionary<string, string>> returnsItems = new Dictionary<int, Dictionary<string, string>>();

        public static int count = 1;

        private void DispatcherHandler(PcapDotNet.Packets.Packet packet)
        {
            var instance = Container.GetInstance<IPars>("Ethernet");
            /*instance.Pars(packet);
            instance = Container.GetInstance<IPars>("UDP");
            instance.Pars(packet);
            instance = Container.GetInstance<IPars>("TCP");
            instance.Pars(packet);*/
            instance = Container.GetInstance<IPars>("ICMP");
            instance.Pars(packet);
            instance = Container.GetInstance<IPars>("HTTP");
            instance.Pars(packet);
        }

        private LightInject.ServiceContainer Container = new LightInject.ServiceContainer();

        private string FileName;
    }


    interface IPars
    {
        void Pars(PcapDotNet.Packets.Packet packet);

    }

    class EthenetPars : IPars
    {
        public void Pars(PcapDotNet.Packets.Packet packet)
        {
        }
    }

    class HTTPPars : IPars
    {
        public void Pars(PcapDotNet.Packets.Packet packet)
        {
            PcapDotNet.Packets.Http.HttpDatagram packetHttp = packet.Ethernet.IpV4.Tcp.Http;
            var ip = packet.Ethernet.IpV4;
            var source = ip.Source.ToString();
            var destination = ip.Destination.ToString();
            Dictionary<string, string> infoDic = new Dictionary<string, string>();
            if (packetHttp.Header != null)
            {
                infoDic.Add("Date", packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"));
                infoDic.Add("Protocol", "HTTP");
                infoDic.Add("Length", packetHttp.Length.ToString());
                infoDic.Add("Source", source);
                infoDic.Add("Destination", destination);
                if (packetHttp.IsRequest)
                {
                    PcapDotNet.Packets.Http.HttpRequestDatagram packetHttpRequest = (PcapDotNet.Packets.Http.HttpRequestDatagram)packet.Ethernet.IpV4.Tcp.Http;
                    infoDic.Add("Info", packetHttpRequest.Method.KnownMethod.ToString() + " " + packetHttpRequest.Uri.ToString() + " " + packetHttpRequest.Version.ToString());
                }
                else
                {
                    PcapDotNet.Packets.Http.HttpResponseDatagram packetHttpResponse = (PcapDotNet.Packets.Http.HttpResponseDatagram)packet.Ethernet.IpV4.Tcp.Http;
                    infoDic.Add("Info", packetHttpResponse.Version.ToString() + " " + packetHttpResponse.StatusCode.ToString());
                }

                FileOpen.returnsItems.Add(FileOpen.count++, infoDic);

            }
        }
    }

    class ICMPPars : IPars
    {
        public void Pars(PcapDotNet.Packets.Packet packet)
        {
            PcapDotNet.Packets.Icmp.IcmpDatagram packetICMP = packet.Ethernet.IpV4.Icmp;
            var ip = packet.Ethernet.IpV4;
            var source = ip.Source.ToString();
            var destination = ip.Destination.ToString();
            Dictionary<string, string> infoDic = new Dictionary<string, string>();
            if (packetICMP.IsChecksumCorrect)
            {
                infoDic.Add("Date", packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"));
                infoDic.Add("Protocol", "ICMP");
                infoDic.Add("Length", packetICMP.Length.ToString());
                infoDic.Add("Source", source);
                infoDic.Add("Destination", destination);
                infoDic.Add("Info", packetICMP.MessageTypeAndCode.ToString());
            }

            FileOpen.returnsItems.Add(FileOpen.count++, infoDic);
        }
    }

    class IoC
    {
        public static void Registration(LightInject.ServiceContainer container)
        {
            //container.Register<IPars, EthenetPars>("Ethernet");
            //container.Register<IPars, TCPPars>("TCP");
            container.Register<IPars, ICMPPars>("ICMP");
            //container.Register<IPars, UDPPars>("UDP");
            container.Register<IPars, HTTPPars>("HTTP");
        }
    }
}
