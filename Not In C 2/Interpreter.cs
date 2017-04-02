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
                if (test(1, "Must have variable name to set") && test(2, "Must have value to set to"))
                {
                    Memory.type T = Memory.VGet(words[1]);
                    try
                    {
                        if (T == Memory.type.Int) { var d = new DataTable(); Memory.ISet(words[1], int.Parse(d.Compute(getAllAfter(1), "").ToString())); }
                        else if (T == Memory.type.Bool) { Memory.BSet(words[1], bool.Parse(getAllAfter(1))); }
                        else if (T == Memory.type.String) Memory.SSet(words[1], getAllAfter(1));
                        else if (T == Memory.type.Double) { var d = new DataTable(); Memory.DSet(words[1], double.Parse(d.Compute(getAllAfter(1), "").ToString())); }
                    }
                    catch (FormatException) { Error("Set types in variable and new value do not match"); }
                }
            }
            else if (words[0] == "comp")
            {
                if (test(1, "Need i for integer/double or b for boolean comparison") && test(2, "Missing first compare element") && test(3, "Missing compare operator") && test(4, "Missing second compare element") && test(5, "Missing variable to output to"))
                {
                    if (words[1] == "i")
                    {
                        string sd1 = words[2], sd2 = words[4];
                        if (sd1.StartsWith("@")) sd1 = expand(sd1);
                        if (sd2.StartsWith("@")) sd2 = expand(sd2);
                        string oper = words[3], outname = words[5];
                        double d1, d2;
                        if (double.TryParse(sd1, out d1) && double.TryParse(sd2, out d2))
                        {
                            if (oper.isIComp())
                            {
                                if(Memory.VGet(outname) == Memory.type.Bool)
                                { //if everything is correct
                                    if (oper == "eq") Memory.BSet(outname, d1 == d2);
                                    else if (oper == "ls") Memory.BSet(outname, d1 < d2);
                                    else if (oper == "gr") Memory.BSet(outname, d1 > d2);
                                    else if (oper == "ne") Memory.BSet(outname, d1 != d2);
                                    else if (oper == "lq") Memory.BSet(outname, d1 <= d2);
                                    else if (oper == "gq") Memory.BSet(outname, d1 >= d2);
                                }
                            }
                            else Error("Expected an integer comparison");
                        }
                        else Error("Expected an integer or double for comparison");
                    }
                    else if (words[1] == "b")
                    {
                        bool b1, b2;
                    }
                    else Error("Need i for integer/double or b for boolean comparison");
                }
            }
            else if (Memory.VGet(words[0]) == Memory.type.Function) { execFunction(words[0]); }
            else throw new Exception();
        }
        static bool isIComp(this string t)
        {
            if (t == "eq") return true;
            else if (t == "ls") return true;
            else if (t == "gr") return true;
            else if (t == "ne") return true;
            else if (t == "lq") return true;
            else if (t == "gq") return true;
            else return false;
        }
        static bool isBComp(this string t)
        {
            if (t == "or") return true;
            else if (t == "nor") return true;
            else if (t == "and") return true;
            else if (t == "nand") return true;
            else if (t == "xor") return true;
            else if (t == "xnor") return true;
            else return false;
        }
    }
}
