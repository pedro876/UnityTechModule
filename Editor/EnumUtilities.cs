using System.IO;
using UnityEditor;

public static class EnumUtilities
{
    #region Write Methods

    public static void WriteEnum(string enumName, string path, string[] elements, string namespaceName = "")
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        bool writeNamespace = !string.IsNullOrEmpty(namespaceName);
        string space = writeNamespace ? "\t" : string.Empty;
        path = Path.Combine(path, enumName) + ".cs";

        using (StreamWriter sw = new(path, false))
        {
            if (writeNamespace)
            {
                sw.WriteLine($"namespace {namespaceName}");
                sw.WriteLine("{");
            }

            sw.WriteLine($"{space}public enum {enumName} //This script is autogenerate");
            sw.WriteLine(space + "{");

            for (int i = 0; i < elements.Length; i++)
            {
                sw.WriteLine($"{space}\t{elements[i]} = {i},");
            }

            sw.WriteLine(space + "}");

            if (writeNamespace) sw.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }

    public static void WriteEnum(string enumName, string path, string[] elements, int[] indices, string namespaceName = "")
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        bool writeNamespace = !string.IsNullOrEmpty(namespaceName);
        string space = writeNamespace ? "\t" : string.Empty;
        path = Path.Combine(path, enumName) + ".cs";

        using (StreamWriter sw = new(path, false))
        {
            if (writeNamespace)
            {
                sw.WriteLine($"namespace {namespaceName}");
                sw.WriteLine("{");
            }

            sw.WriteLine($"{space}public enum {enumName} //This script is autogenerate");
            sw.WriteLine(space + "{");

            for (int i = 0; i < elements.Length; i++)
            {
                sw.WriteLine($"{space}\t{elements[i]} = {indices[i]},");
            }

            sw.WriteLine(space + "}");

            if (writeNamespace) sw.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }

    #endregion
}
