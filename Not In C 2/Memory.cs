using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Not_In_C_2
{
    static class Memory
    {
        private class NInt
        {
            public string name;
            public int value;
            public NInt(string name, int value) { this.name = name; this.value = value; }
        }
        private class NBool
        {
            public string name;
            public bool value;
            public NBool(string name, bool value) { this.name = name; this.value = value; }
        }
        private class NString
        {
            public string name;
            public string value;
            public NString(string name, string value) { this.name = name; this.value = value; }
        }
        private class NDouble
        {
            public string name;
            public double value;
            public NDouble(string name, double value) { this.name = name; this.value = value; }
        }
        private class NLabel
        {
            public string name;
            public int line;
            public NLabel(string name, int line) { this.name = name; this.line = line; }
        }
        //private class NParameter
        //{
        //    public string name;
        //    public type type;
        //    public NParameter(string name, type type) { this.name = name; this.type = type; }
        //}
        private class NBlock
        {
            public int[] lines;
            public int start;
            public int end;
            public NBlock(int start, int end)
            {
                this.start = start;
                this.end = end;
                List<int> temp = new List<int>();
                for (int x = start; x <= end; x++)
                    temp.Add(x);
                lines = temp.ToArray();
            }
            public NBlock(int[] lines)
            {
                this.lines = lines;
                start = lines[0];
                end = lines[lines.Length - 1];
            }            
            public bool containsLine(int line)
            {
                foreach (int i in lines)
                    if (i == line) return true;
                return false;
            }
        }
        private class NFunction
        {
            public string name;
            //public List<NParameter> param = new List<NParameter>();
            public NBlock block;
            public NFunction(string name, int start, int end) { this.name = name; block = new NBlock(start, end); }
        }

        public static int Length<T>(this List<T> list)
        {
            int r = 0;
            foreach (T t in list)
                r++;
            return r;
        }
        public enum type { Int, Bool, String, Double, Label, Block, Function } //for easy communication between functions
        static List<NInt> Int = new List<NInt>(); //memory spot for integers
        static List<NBool> Bool = new List<NBool>(); //memory spot for booleans
        static List<NString> String = new List<NString>(); //memory spot for strings
        static List<NDouble> Double = new List<NDouble>(); //memory spot for doubles
        static List<NLabel> Label = new List<NLabel>(); //memory spot for labels
        static List<NBlock> Block = new List<NBlock>(); //memory spot for blocks
        static List<NFunction> Function = new List<NFunction>(); //memory spot for functions


        //the first letter indicates the type (I = int, B = bool, S = string, D = Double, L = Label, O = Block, F = Function, V = generic type)
        //new adds a new object to its allocated memory
        //get gets the value of an object based on the name
        //set sets the value of an object based on the name        
        public static type VGet(string name)
        {
            foreach (NInt n in Int)
                if (n.name == name) return type.Int;
            foreach (NBool n in Bool)
                if (n.name == name) return type.Bool;
            foreach (NString n in String)
                if (n.name == name) return type.String;
            foreach (NDouble n in Double)
                if (n.name == name) return type.Double;
            foreach (NLabel n in Label)
                if (n.name == name) return type.Label;
            foreach (NFunction n in Function)
                if (n.name == name) return type.Function;
            Interpreter.Error("Variable not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static bool VExists(string name)
        {
            foreach (NInt n in Int)
                if (n.name == name) return true;
            foreach (NBool n in Bool)
                if (n.name == name) return true;
            foreach (NString n in String)
                if (n.name == name) return true;
            foreach (NDouble n in Double)
                if (n.name == name) return true;
            foreach (NLabel n in Label)
                if (n.name == name) return true;
            foreach (NFunction n in Function)
                if (n.name == name) return true;
            return false;
        }
        public static void INew(string name, int value)
        {
            if (!VExists(name)) Int.Add(new NInt(name, value));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static int IGet(string name)
        {
            for (int x = 0; x < Int.Length(); x++)
                if (Int[x].name == name) return Int[x].value;
            Interpreter.Error("Int not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void ISet(string name, int value)
        {
            for (int x = 0; x < Int.Length(); x++)
                if (Int[x].name == name) { Int[x].value = value; return; }
            Interpreter.Error("Int not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void BNew(string name, bool value)
        {
            if (!VExists(name)) Bool.Add(new NBool(name, value));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static bool BGet(string name)
        {
            for (int x = 0; x < Bool.Length(); x++)
                if (Bool[x].name == name) return Bool[x].value;
            Interpreter.Error("Boolean not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void BSet(string name, bool value)
        {
            for (int x = 0; x < Bool.Length(); x++)
                if (Bool[x].name == name) { Bool[x].value = value; return; }
            Interpreter.Error("Boolean not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void SNew(string name, string value)
        {
            if (!VExists(name)) String.Add(new NString(name, value));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static string SGet(string name)
        {
            for (int x = 0; x < String.Length(); x++)
                if (String[x].name == name) return String[x].value;
            Interpreter.Error("String not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void SSet(string name, string value)
        {
            for (int x = 0; x < String.Length(); x++)
                if (String[x].name == name) { String[x].value = value; return; }
            Interpreter.Error("String not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void DNew(string name, double value)
        {
            if (!VExists(name)) Double.Add(new NDouble(name, value));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static double DGet(string name)
        {
            for (int x = 0; x < Double.Length(); x++)
                if (Double[x].name == name) return Double[x].value;
            Interpreter.Error("Double not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void DSet(string name, double value)
        {
            for (int x = 0; x < Double.Length(); x++)
                if (Double[x].name == name) { Double[x].value = value; return; }
            Interpreter.Error("Double not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void LNew(string name, int line)
        {
            if (!VExists(name)) Label.Add(new NLabel(name, line));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static int LGet(string name)
        {
            for (int x = 0; x < Label.Length(); x++)
                if (Label[x].name == name) return Label[x].line;
            Interpreter.Error("Label not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
        public static void FNew(string name, int start, int end)
        {
            if (!VExists(name)) Function.Add(new NFunction(name, start, end));
            else Interpreter.Error("Variable already exists under \"" + name + "\"");
        }
        public static int[] FGet(string name)
        {
            foreach (NFunction f in Function)
                if (f.name == name) return f.block.lines;
            Interpreter.Error("Function not found: \"" + name + "\"", true);
            throw new ArgumentException();
        }
    }
    
}
