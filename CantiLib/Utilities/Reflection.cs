using System;

namespace Canti.Utilities
{
    public class Reflection
    {
        // Prints current depth in the form of "--" * depth
        private static void PrintDepth(int Depth)
        {
            for (int i = 0; i < Depth; i++)
                Console.Write("--");
        }

        // Prints out an object and all of it's public properties to the console for debug purposes
        public static void PrintObject(object Value, int Depth = 0)
        {
            PrintDepth(Depth);
            Console.WriteLine("Object Type: " + Value.GetType().Name);
            if (Value.GetType().IsArray)
            {
                PrintDepth(Depth);
                Array Objs = (Array)Value;
                Console.WriteLine("Values [{0}]:", Objs.Length);

                for (int i = 0; i < Objs.Length; i++)
                {
                    PrintDepth(Depth + 1);
                    Console.WriteLine("[{0}]:", i);
                    PrintObject(Objs.GetValue(i), Depth + 2);
                }
            }
            else
            {
                var Properties = Value.GetType().GetProperties();
                if (Properties.Length < 1)
                {
                    PrintDepth(Depth);
                    Console.WriteLine("Value: " + Value);
                }
                else
                {
                    int NestedDepth = Depth + 1;
                    PrintDepth(Depth);
                    Console.WriteLine("Properties [{0}]:", Properties.Length);
                    int o = 0;
                    foreach (var Property in Properties)
                    {
                        PrintDepth(NestedDepth + 1);
                        Console.WriteLine("[{0}] {1}:", o, Property.Name);
                        try { PrintObject(Property.GetValue(Value), NestedDepth + 2); }
                        catch { }
                        o++;
                    }
                }
            }
        }
    }
}
