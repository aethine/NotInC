using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Not_In_C_2
{
    static class Interpreter
    {
        static string[] Imem;
        static int lineNum = 0;
        static string expand(string namewithat)
        {
            string name = namewithat.Split('@')[1];
            Memory.type T = Memory.VGet(name);
            if (T == Memory.type.Int) return Memory.IGet(name).ToString();
            else if (T == Memory.type.Bool) return Memory.BGet(name).ToString();
            else if (T == Memory.type.String) return Memory.SGet(name);
            else if (T == Memory.type.Double) return Memory.DGet(name).ToString();
            throw new Exception();
        }
        static string getAllAfter(int place)
        {
            string tor = null;
            List<string> temp = new List<string>();
            try
            {
                for (int x = place + 1; test(x); x++)
                {
                    if (words[x].StartsWith("@")) temp.Add(expand(words[x]));
                    else if (words[x] == "\\n") temp.Add(Environment.NewLine);
                    else temp.Add(words[x]);
                }
                foreach (string y in temp)
                    tor += y + " ";
            }
            catch (IndexOutOfRangeException) { Error("Nothing found after place " + place); }
            return tor.Remove(tor.Length - 1);

        }
        static bool test(int place, string message = null)
        {
            if (place < words.Length) return true;
            else { if (message != null) Error(message); }
            return false;
        }
        public static void run(string[] lines)
        {
            Console.Clear(); Imem = lines;
            for (int x = 0; lineNum < Imem.Length; lineNum++) exec(Imem[lineNum]);
        }
        public static void Error(string message, bool showline = true, int linenum = -1)
        {
            int l = linenum;
            if (l < 0) l = lineNum;
            Console.Write("ERROR: {0}", message);
            if (showline) Console.Write(" on line {0}", l);
            Console.WriteLine();
            if (Compiler.st1)
            {
                if (Imem == null) Imem = Compiler.st1l;
                string s = null;
                foreach (string t in Imem)
                    s += t + "\n";
                Console.WriteLine("Press c to show the compiled lines");
                if (Console.ReadKey(true).KeyChar == 'c') { Console.WriteLine("\n{0}", s); }
                else Environment.Exit(1);
            }
            Console.ReadKey();
            Environment.Exit(1);
        } //display message then quit
        public static void execFunction(string name)
        {
            int[] lines = Memory.FGet(name);
            foreach (int s in lines) exec(Imem[s]);
        }
        static string[] words;
        static void exec(string line)
        {
            words = line.Split(' ');
            if (words[0] == "exit") Environment.Exit(0);
            else if (words[0] == "pause") Console.ReadKey(true);
            else if (words[0] == "clear") Console.Clear();
            else if (words[0] == "write") Console.Write(getAllAfter(0));
            else if (words[0] == "writeline") Console.WriteLine(getAllAfter(0));
            else if (words[0] == "int")
            {
                if (test(1, "Declaration needs a name"))
                {
                    if (test(2)) try { Memory.INew(words[1], int.Parse(words[2])); } catch (FormatException) { Error("Set is not of right type"); }
                    else { Memory.INew(words[1], 0); }
                }
            }
            else if (words[0] == "bool")
            {
                if (test(1, "Declaration needs a name"))
                {
                    if (test(2)) try { Memory.BNew(words[1], bool.Parse(words[2])); } catch (FormatException) { Error("Set is not of right type"); }
                    else { Memory.BNew(words[1], false); }
                }
            }
            else if (words[0] == "string")
            {
                if (test(1, "Declaration needs a name"))
                {
                    if (test(2)) Memory.SNew(words[1], getAllAfter(1));
                    else { Memory.SNew(words[1], null); }
                }
            }
            else if (words[0] == "double")
            {
                if (test(1, "Declaration needs a name"))
                {
                    if (test(2)) try { Memory.DNew(words[1], double.Parse(words[2])); } catch (FormatException) { Error("Set is not of right type"); }
                    else { Memory.DNew(words[1], 0.0); }
                }
            }
            else if (words[0] == "lab") { if (test(1, "Label needs a name")) Memory.LNew(words[1], lineNum); }
            else if (words[0] == "func")
            {
                if (test(1, "Funcion needs a name"))
                {
                    List<int> ls = new List<int>();
                    lineNum++;
                    if (Imem[lineNum] != "{") Error("Function must have a block attatched in next line");
                    lineNum++;
                    for (int x; Imem[lineNum] != "}"; lineNum++) ls.Add(lineNum);
                    Memory.FNew(words[1], ls[0], ls[ls.Length() - 1]);
                }

            }
            else if (words[0] == "goto") { if (test(1, "Goto must have a label name")) lineNum = Memory.LGet(words[1]); }
            else if (words[0] == "if")
            {
                bool exec = false;
                if (test(1, "If statement must have boolean"))
                {
                    if (words[1] == "false" || words[1] == "true") { exec = bool.Parse(words[1]); }
                    else if (words[1].StartsWith("@"))
                    {
                        if (Memory.VGet(words[1].Split('@')[1]) == Memory.type.Bool) { exec = Memory.BGet(words[1].Split('@')[1]); }
                        else Error("If statement must have boolean");
                    }
                    else Error("If statement must have boolean");
                    if (!exec)
                        for (int x = lineNum; Imem[lineNum] != "}"; lineNum++) ;
                }
            }
            else if (words[0] == "set")
            {
                if (test(1, "Must have variable name to set") && test(2, "Must have variable to set to"))
                {
                    Memory.type T = Memory.VGet(words[1]);
                    try
                    {
                        if (T == Memory.type.Int) { var d = new DataTable(); Memory.ISet(words[1], int.Parse(d.Compute(getAllAfter(1), "").ToString())); }
                        else if (T == Memory.type.Bool) Memory.BSet(words[1], bool.Parse(words[2]));
                        else if (T == Memory.type.String) Memory.SSet(words[1], getAllAfter(1));
                        else if (T == Memory.type.Double) { var d = new DataTable(); Memory.DSet(words[1], double.Parse(d.Compute(getAllAfter(1), "").ToString())); }
                    }
                    catch (FormatException) { Error("Set types in variable and new value do not match"); }
                }
            }
            else if (Memory.VGet(words[0]) == Memory.type.Function) { execFunction(words[0]); }
            else throw new Exception();
        }
    }
}
