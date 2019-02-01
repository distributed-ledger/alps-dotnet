using ALPS.Shared;
using Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ALPS.Client {
    class Program {

        private static readonly Protocol protocol = new Protocol(
            HandshakePattern.KK,
            CipherFunction.ChaChaPoly,
            HashFunction.Blake2b,
            PatternModifiers.Psk2
        );

        public static byte[] ClientPrivateKey = new byte[]{
            0x52, 0x45, 0x61, 0x19, 0xDC, 0xB7, 0x65, 0x49,
            0x95, 0xEE, 0xF8, 0xDD, 0x27, 0x9A, 0xAC, 0x80,
            0x02, 0xB8, 0x6E, 0xA5, 0xC6, 0x3A, 0x9A, 0xEE,
            0xFB, 0xD2, 0x28, 0xA4, 0xE5, 0xF3, 0x08, 0xA6
        };

        public static string ServerHost = IPAddress.Loopback.ToString();

        private const string message = "ALPS -> client to server message";

        static void Main(string[] args) {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args) {

            Console.WriteLine("=============================================================");
            Console.WriteLine("========================= CLIENT ============================");
            Console.WriteLine("=============================================================");

            var sendBuffer = new byte[Protocol.MaxMessageLength];
            var receiveBuffer = new byte[Protocol.MaxMessageLength];

            using (var handshakeState = protocol.Create(true, s: ClientPrivateKey, rs: SharedKeys.ServerPublicKey, psks: new[] { SharedKeys.PSK })) {

                var sendChannel = new UdpClient(ServerHost, Ports.ServerListeningPort);
                var receiveChannel = new UdpClient(Ports.ClientListeningPort);

                // Send the first message (with payload message) to the server.
                var (bytesWritten, _, _) = handshakeState.WriteMessage(Encoding.UTF8.GetBytes(message), sendBuffer);
                await sendChannel.SendAsync(sendBuffer, bytesWritten);

                // Receive the second handshake message from the server (with payload message).
                var received = await receiveChannel.ReceiveAsync();
                var (bytesRead, _, transport) = handshakeState.ReadMessage(received.Buffer, receiveBuffer);
                
                Console.WriteLine($"MESSAGE: {Encoding.UTF8.GetString(receiveBuffer.Take(bytesRead).ToArray())}");

                // TODO: Continue with additional messages using "transport"

                receiveChannel.Dispose();
                sendChannel.Dispose();

            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

        }
    }
}
