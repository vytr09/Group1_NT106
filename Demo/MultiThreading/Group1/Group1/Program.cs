using System;
using System.Threading;

namespace Grou1 {
    class Program {
        public static void Main() {
            Console.WriteLine("Da luong trong C#");
            Console.WriteLine("MultiThreading in C#");
            Thread th = Thread.CurrentThread;
            th. Name = "MainThread";
            Console.WriteLine("Day la {0}", th. Name);
            Console. ReadKey();
        }
    }
}

// namespace Group1 {
//     class Program {
//         static void Main() {
//             Thread th1 = new Thread(MethodA);
//             Thread th2 = new Thread(MethodB);
//             Thread th3 = new Thread(MethodC);
            
//             th1.Start();
//             th2.Start();
//             th2.Join();
//             th3.Start();
//         }
//         static void MethodA() {
//             for (int i = 0; i < 100; i++) Console.Write("0");
//         }
//         static void MethodB() {
//             for (int i = 0; i < 100; i++) Console.Write("1");
//         }
//         static void MethodC() {
//             for (int i = 0; i < 100; i++) Console.Write("2");
//         }
//     }
// }
