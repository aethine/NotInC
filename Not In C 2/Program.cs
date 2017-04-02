using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Not_In_C_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Not In C Compiler/Interpreter 6.0";
            Console.WriteLine("Enter Filepath");
            Interpreter.run(Compiler.lines(Console.ReadLine()));
        }
    }
}
