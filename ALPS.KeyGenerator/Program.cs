using Noise;
using System;
using System.Security.Cryptography;

namespace ALPS.KeyGenerator {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("=============================================================");
            Console.WriteLine("======================= KEY GENERATOR =======================");
            Console.WriteLine("=============================================================");

            KeyPair clientKeyPair = KeyPair.Generate();
            KeyPair serverKeyPair = KeyPair.Generate();
            var psk = new byte[32];
            using (var random = RandomNumberGenerator.Create()) {
                random.GetBytes(psk);
            }

            Console.WriteLine();

            Console.WriteLine($"Server public key:\t{BitConverter.ToString(serverKeyPair.PublicKey)}");
            Console.WriteLine($"Server private key:\t{BitConverter.ToString(serverKeyPair.PrivateKey)}");
            Console.WriteLine($"Client public key:\t{BitConverter.ToString(clientKeyPair.PublicKey)}");
            Console.WriteLine($"Client private key:\t{BitConverter.ToString(clientKeyPair.PrivateKey)}");
            Console.WriteLine($"Common PSK:\t\t{BitConverter.ToString(psk)}");

            Console.WriteLine();

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

        }
    }
}
