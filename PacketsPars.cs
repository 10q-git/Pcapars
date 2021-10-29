namespace PacketsPars
{
    using System.Collections.Generic;
    using LightInject;
    using PcapDotNet.Core;

    /// <summary>
    /// Работа с файлом.
    /// </summary>
    public class FileOpen
    {
        /// <summary>
        /// Словарь с результатом работы парсеров.
        /// </summary>
        private static Dictionary<int, Dictionary<string, string>> returnsItems = new Dictionary<int, Dictionary<string, string>>();

        /// <summary>
        /// Количество обработанных пакетов.
        /// </summary>
        private static int count = 1;

        /// <summary>
        /// Контейнер парсеров.
        /// </summary>
        private LightInject.ServiceContainer container = new LightInject.ServiceContainer();

        /// <summary>
        /// Имя обрабатываемого файла.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Конструктор с именем файла.
        /// </summary>
        /// <param name="nameFile">Имя файла.</param>
        public FileOpen(string nameFile)
        {
            this.fileName = nameFile;
        }

        /// <summary>
        /// Интерфейс для парсеров.
        /// </summary>
        private interface IPars
        {
            /// <summary>
            /// Запуск парсера.
            /// </summary>
            /// <param name="packet">Обрабатываемый пакет.</param>
            void Pars(PcapDotNet.Packets.Packet packet);
        }

        /// <summary>
        /// Геттер для результата работы.
        /// </summary>
        /// <returns>Dictionary<int, Dictionary<string, string>>.</returns>
        public static Dictionary<int, Dictionary<string, string>> GetReturnsItems()
        {
            return returnsItems;
        }

        /// <summary>
        /// Очистка результата.
        /// </summary>
        /// <returns>Dictionary<int, Dictionary<string, string>>.</returns>
        public static void ClearReturnsItems()
        {
            returnsItems = new Dictionary<int, Dictionary<string, string>>();
        }

        /// <summary>
        /// Геттер для количества обработанных пакетов.
        /// </summary>
        /// <returns>int</returns>
        public static int GetCount()
        {
            return count;
        }

        /// <summary>
        /// Запуск обработки файла.
        /// </summary>
        /// <param name="nameFile">Имя файла.</param>
        public void Open(string nameFile)
        {
            // Create the offline device
            OfflinePacketDevice selectedDevice = new OfflinePacketDevice(nameFile);

            // Open the capture file
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {
                // Read and dispatch packets until EOF is reached
                IoC.Registration(this.container);
                communicator.ReceivePackets(0, this.DispatcherHandler);
            }
        }

        /// <summary>
        /// Функция обработки пакета.
        /// </summary>
        /// <param name="packet">Обрабатываемый пакет.</param>
        private void DispatcherHandler(PcapDotNet.Packets.Packet packet)
        {
            var instance = this.container.GetInstance<IPars>("UDP");
            instance.Pars(packet);
            instance = this.container.GetInstance<IPars>("TCP");
            instance.Pars(packet);
            instance = this.container.GetInstance<IPars>("ICMP");
            instance.Pars(packet);
            instance = this.container.GetInstance<IPars>("HTTP");
            instance.Pars(packet);
        }

        /// <summary>
        /// TCP-парсер.
        /// </summary>
        private class TCPPars : IPars
        {
            /// <summary>
            /// Запуск парсера.
            /// </summary>
            /// <param name="packet">Обрабатываемый пакет.</param>
            public void Pars(PcapDotNet.Packets.Packet packet)
            {
                if (packet.Ethernet != null && packet.Ethernet.IpV4 != null && packet.Ethernet.IpV4.Tcp != null)
                {
                    PcapDotNet.Packets.Transport.TcpDatagram packetTCP = packet.Ethernet.IpV4.Tcp;

                    var ip = packet.Ethernet.IpV4;
                    var source = ip.Source.ToString();
                    var destination = ip.Destination.ToString();
                    Dictionary<string, string> infoDic = new Dictionary<string, string>();

                    if (packetTCP.IsValid && packetTCP.IsAcknowledgment)
                    {
                        infoDic.Add("Date", packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"));
                        infoDic.Add("Protocol", "TCP");
                        infoDic.Add("Length", packetTCP.Length.ToString());
                        infoDic.Add("Source", source);
                        infoDic.Add("Destination", destination);
                        string ack = "; ack=" + packetTCP.AcknowledgmentNumber.ToString();
                        string seq = ", seq=" + packetTCP.SequenceNumber.ToString();
                        infoDic.Add("Info", "Sourse Port: " + packetTCP.SourcePort + " -> Destination Port: " + packetTCP.DestinationPort + ack + seq + "\n\n Ethernet: MacSource: " + packet.Ethernet.Source + "      MacDestination:" + packet.Ethernet.Destination);
                    }

                    FileOpen.returnsItems.Add(FileOpen.count++, infoDic);
                }
            }
        }

        /// <summary>
        /// UDP-парсер.
        /// </summary>
        private class UDPPars : IPars
        {
            /// <summary>
            /// Запуск парсера.
            /// </summary>
            /// <param name="packet">Обрабатываемый пакет.</param>
            public void Pars(PcapDotNet.Packets.Packet packet)
            {
                if (packet.Ethernet != null && packet.Ethernet.IpV4 != null && packet.Ethernet.IpV4.Udp != null)
                {
                    PcapDotNet.Packets.Transport.UdpDatagram packetUDP = packet.Ethernet.IpV4.Udp;
                    var ip = packet.Ethernet.IpV4;
                    var source = ip.Source.ToString();
                    var destination = ip.Destination.ToString();
                    Dictionary<string, string> infoDic = new Dictionary<string, string>();
                    if (packet.Ethernet.IpV4.Transport != null && packet.Ethernet.IpV4.Transport.IsChecksumOptional && packetUDP.Dns.IsValid == false)
                    {
                        infoDic.Add("Date", packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"));
                        infoDic.Add("Protocol", "UDP");
                        infoDic.Add("Length", packetUDP.TotalLength.ToString());
                        infoDic.Add("Source", source);
                        infoDic.Add("Destination", destination);
                        infoDic.Add("Info", "Sourse Port: " + packetUDP.SourcePort + " -> Destination Port: " + packetUDP.DestinationPort + "\n\n Ethernet: MacSource: " + packet.Ethernet.Source + "      MacDestination:" + packet.Ethernet.Destination);
                    }

                    FileOpen.returnsItems.Add(FileOpen.count++, infoDic);
                }
            }
        }

        /// <summary>
        /// HTTP-парсер.
        /// </summary>
        private class HTTPPars : IPars
        {
            /// <summary>
            /// Запуск парсера.
            /// </summary>
            /// <param name="packet">Обрабатываемый пакет.</param>
            public void Pars(PcapDotNet.Packets.Packet packet)
            {
                if (packet.Ethernet != null && packet.Ethernet.IpV4 != null && packet.Ethernet.IpV4.Tcp != null && packet.Ethernet.IpV4.Tcp.Http != null)
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
                            infoDic.Add("Info", packetHttpRequest.Method.KnownMethod.ToString() + " " + packetHttpRequest.Uri.ToString() + " " + packetHttpRequest.Version.ToString() + "\n\n Ethernet: MacSource: " + packet.Ethernet.Source + "      MacDestination:" + packet.Ethernet.Destination);
                        }
                        else
                        {
                            PcapDotNet.Packets.Http.HttpResponseDatagram packetHttpResponse = (PcapDotNet.Packets.Http.HttpResponseDatagram)packet.Ethernet.IpV4.Tcp.Http;
                            infoDic.Add("Info", packetHttpResponse.Version.ToString() + " " + packetHttpResponse.StatusCode.ToString() + "\n\n Ethernet: MacSource: " + packet.Ethernet.Source + "      MacDestination:" + packet.Ethernet.Destination);
                        }

                        FileOpen.returnsItems.Add(FileOpen.count++, infoDic);
                    }
                }
            }
        }

        /// <summary>
        /// ICMP-парсер.
        /// </summary>
        private class ICMPPars : IPars
        {
            /// <summary>
            /// Запуск парсера.
            /// </summary>
            /// <param name="packet">Обрабатываемый пакет.</param>
            public void Pars(PcapDotNet.Packets.Packet packet)
            {
                if (packet.Ethernet != null && packet.Ethernet.IpV4 != null && packet.Ethernet.IpV4.Icmp != null)
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
                        infoDic.Add("Info", packetICMP.MessageTypeAndCode.ToString() + "\n\n Ethernet: MacSource: " + packet.Ethernet.Source + "      MacDestination:" + packet.Ethernet.Destination);
                    }

                    FileOpen.returnsItems.Add(FileOpen.count++, infoDic);
                }
            }
        }

        /// <summary>
        /// Для работы с IoC контейнером.
        /// </summary>
        private class IoC
        {
            /// <summary>
            /// Регистрация парсеров в IoC контейнере.
            /// </summary>
            /// <param name="container">Контейнер, в который нужно зарегистрировать парсеры.</param>
            public static void Registration(LightInject.ServiceContainer container)
            {
                container.Register<IPars, TCPPars>("TCP");
                container.Register<IPars, ICMPPars>("ICMP");
                container.Register<IPars, UDPPars>("UDP");
                container.Register<IPars, HTTPPars>("HTTP");
            }
        }
    }
}
