using ALPS.Shared;
using Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ALPS.Server {

    class Program {

        private static readonly Protocol protocol = new Protocol(
            HandshakePattern.KK,
            CipherFunction.ChaChaPoly,
            HashFunction.Blake2b,
            PatternModifiers.Psk2
        );

        public static byte[] ServerPrivateKey = new byte[]{
            0xBC, 0x88, 0xD2, 0xAB, 0x46, 0x17, 0x6C, 0x77,
            0x6B, 0x8E, 0xCC, 0x28, 0x54, 0xEC, 0xE7, 0xE2,
            0x00, 0x18, 0xFA, 0xFE, 0xB0, 0xED, 0xCE, 0x7B,
            0x8D, 0x5F, 0xB7, 0xA7, 0xCE, 0xAD, 0x06, 0x6A
        };

        public static string ClientHost = IPAddress.Loopback.ToString();

        private const string message = "ALPS -> server to client message";

        static void Main(string[] args) {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args) {

            Console.WriteLine("=============================================================");
            Console.WriteLine("========================= SERVER ============================");
            Console.WriteLine("=============================================================");

            var sendBuffer = new byte[Protocol.MaxMessageLength];
            var receiveBuffer = new byte[Protocol.MaxMessageLength];

            using (var handshakeState = protocol.Create(false, s: ServerPrivateKey, rs: SharedKeys.ClientPublicKey, psks: new[] { SharedKeys.PSK })) {

                var receiveChannel = new UdpClient(Ports.ServerListeningPort);
                var sendChannel = new UdpClient(ClientHost, Ports.ClientListeningPort);

                // Receive the first handshake message (with payload message) from the client.
                var received = await receiveChannel.ReceiveAsync();
                var (bytesRead, _, transport) = handshakeState.ReadMessage(received.Buffer, receiveBuffer);

                Console.WriteLine($"MESSAGE: {Encoding.UTF8.GetString(receiveBuffer.Take(bytesRead).ToArray())}");

                // Send the seconds handshake message (with payload message) to the server.
                var (bytesWritten, _, _) = handshakeState.WriteMessage(Encoding.UTF8.GetBytes(message), sendBuffer);
                await sendChannel.SendAsync(sendBuffer, bytesWritten);

                // TODO: Continue with additional messages using "transport"

                receiveChannel.Dispose();
                sendChannel.Dispose();

            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

        }
    }

}
