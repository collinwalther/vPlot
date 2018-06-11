using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// To run on mac, use the following command:
// open vPlot.app --args [your args here]
public class Startup : MonoBehaviour
{
    public static string errorMsg;

    public void Start()
    {
        errorMsg = "Usage:\n" +
            "\tvPlot --client [IP] [PORT]\n" +
            "\tOR\n" +
            "\tvPlot --server [PORT]\n" +
            "\tOR\n" +
            "\tvPlot --file [path]";

        string[] args = Environment.GetCommandLineArgs();
        if (args[1] == "--client")
        {
            HandleClientStartup(args);
        }
        else if (args[1] == "--server")
        {
            HandleServerStartup(args);
        }
        else if (args[1] == "--file")
        {
            HandleFileStartup(args);
        }
    }

    static void HandleClientStartup(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
        try
        {
            ClientPlot.IP = args[2];
            ClientPlot.port = ushort.Parse(args[3]);
        }
        catch (Exception)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("ClientPlotTestScene");
    }

    static void HandleServerStartup(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
        try
        {
            ServerPlot.port = ushort.Parse(args[2]);
        }
        catch (Exception)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("ServerPlotTestScene");
    }

    static void HandleFileStartup(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
        CSVPlot.PathToCsvFile = args[2];
        UnityEngine.SceneManagement.SceneManager.LoadScene("Simple4SplitScene");
    }
}
