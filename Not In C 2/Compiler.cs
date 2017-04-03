using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Not_In_C_2
{
    static class Compiler
    {
        static bool errored = false;
        static void CompilerError(string message, int linenum)
        {
            errored = true;
            int l = linenum;
            Console.Write("ERROR: {0}", message);
            Console.Write(" on line {0}", l);
            Console.WriteLine();            
        }
        static void expand(string namewithat, int line)
        {
            string name = namewithat.Split('@')[1];
            foreach (string s in varnames)
                if (s == name) return;
            CompilerError("\"" + name + "\" isn't a variable", line);
        }
        public static bool st1 = false;
        public static string[] st1l;
        static List<string> varnames = new List<string>();
        static List<string> functions = new List<string>();
        public static string[] lines(string filepath)
        {
            Console.WriteLine("Gathering lines and inclusions...");
            string[] lines = Step1(filepath); //gathering up all the lines and inclusions
            st1 = true;
            st1l = lines;
            Console.WriteLine("Done!");
            Console.WriteLine("Checking for errors...");
            step2(lines); //to throw a runtime error if necessary
            if (errored)
            {
                string s = null;
                foreach (string t in lines)
                    s += t + "\n";
                Console.WriteLine("Press c to show the compiled lines");
                if (Console.ReadKey(true).KeyChar == 'c') { Console.WriteLine("\n{0}", s); }
                Console.ReadKey();
                Environment.Exit(1);
            }
            Console.WriteLine("Done!");
            Console.WriteLine("Starting program.....");
            return lines; //for interpreter
        }
        public static bool isEmpty(this string i)
        {
            return string.IsNullOrWhiteSpace(i);
        }
        static string[] Step1(string filepath)
        {
            int linecount = 1;
            if (!File.Exists(filepath)) Interpreter.Error("File to start not found", false);
            List<string> toReturn = new List<string>();
            List<string> files = new List<string>();
            files.Add(filepath);
            int counter = 0;
            foreach (string l in File.ReadAllLines(filepath))
            {
                if (l.StartsWith("include"))
                {
                    try
                    {
                        if (!File.Exists(l.Split(' ')[1])) Interpreter.Error("File \" " + l.Split(' ')[1] + "\" not found", linenum: linecount);
                        files.Insert(counter, l.Split(' ')[1]);
                        counter++;
                    }
                    catch (IndexOutOfRangeException) { Interpreter.Error("Must have file to include"); }
                }
                linecount++;
            }
            bool b = false, breaking = false;
            do
            {
                counter = 0;
                foreach (string s in files)
                {
                    b = false;
                    foreach (string t in File.ReadAllLines(s))
                    {
                        if (t.StartsWith("include"))
                        {
                            try
                            {
                                if (!File.Exists(t.Split(' ')[1])) Interpreter.Error("File not found");
                                b = true;
                                files.Insert(counter, t.Split(' ')[1]);
                                bool o = false;
                                for (int x = 0; x < files.ToArray().Length; x++)
                                    if (files[x] == t.Split(' ')[1]) { if (o) files.Remove(files[x]); else o = true; b = false; }
                                counter++;
                                breaking = true;
                            }
                            catch (IndexOutOfRangeException) { Interpreter.Error("Must have file to include"); }
                        }
                        linecount++;
                    }
                    if (breaking) { breaking = false; break; }
                }
            } while (b);
            foreach (string file in files)
                foreach (string line in File.ReadAllLines(file))
                    if (!(line.StartsWith("include") ||line.StartsWith(";") || line.isEmpty())) toReturn.Add(line);
            return toReturn.ToArray();
        } 
        static void step2(string[] lines) 
        {
            int linecount = 1;
            int bracketcount = 0;
            foreach (string line in lines)
            {
                if (line == "{") bracketcount++;
                else if (line == "}") bracketcount--;
                string[] split = line.Split(' ');
                for (int x = 0; x < split.Length; x++) {
                    if (split[x].StartsWith("@"))
                        expand(split[x], linecount);
                }
                string key = split[0];
                if (key.isSysOrVar())
                {
                    if (key.isDecl()) {
                        if (key == "func")
                        {
                            if (!functions.Contains(line.Split(' ')[1])) functions.Add(line.Split(' ')[1]);
                            else CompilerError("Function already exists with name \"" + line.Split(' ')[1] + "\"", linecount);
                        }
                        if (!varnames.Contains(line.Split(' ')[1])) varnames.Add(line.Split(' ')[1]);
                        else CompilerError("Variable already exists with name \"" + line.Split(' ')[1] + "\"", linecount);
                    }
                }
                else if (functions.Contains(key)) ;
                else CompilerError("Function \"" + line.Split(' ')[0] + "\" not recognized", linecount);
                linecount++;
            }
            if (bracketcount != 0) Interpreter.Error("Unbalanced brackets", false);
        }
        static bool isSysOrVar(this string t)
        {
            if (t == "write") return true;
            else if (t == "writeline") return true;
            else if (t == "set") return true;
            else if (t == "pause") return true;
            else if (t == "clear") return true;
            else if (t == "goto") return true;
            else if (t == "comp") return true;
            else if (t == "scan") return true;
            else if (t == "if") return true;
            else if (t == "{") return true;
            else if (t == "}") return true;
            else if (isDecl(t)) return true;
            else return false;
        }
        static bool isDecl(this string t)
        {
            if (t == "int") return true;
            else if (t == "bool") return true;
            else if (t == "string") return true;
            else if (t == "double") return true;
            else if (t == "lab") return true;
            else if (t == "func") return true;
            else return false;
        }
    }
}
