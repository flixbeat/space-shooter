
using System;
using Godot;
using Godot.Collections;

public static class Utils
{
    public static readonly string savePath = "res://saves/";
    public static readonly string saveFile = "save.json";

    private static readonly Random random = new Random();
    
    public static float RandomRange(float min, float max)
    {
        return (float) random.NextDouble() * (max - min) + min;
    }

    public static int RandomNext(int min, int max)
    {
        return random.Next(min,max);
    }

    public static void SaveHighScore(int score)
    {
        string content = "{\"high_score\": \""+score+"\"}";
        
        // check for directory
        Directory dir = new Directory();
        if(!dir.DirExists(Utils.savePath))
            dir.MakeDirRecursive(Utils.savePath);

        File file = new File();
        file.Open(Utils.savePath+Utils.saveFile, File.ModeFlags.Write);
        file.StoreString(content);
        file.Close();
    }

    public static int LoadHighScore()
    {
        // check directory existence
        Directory dir = new Directory();
        if(!dir.DirExists(Utils.savePath))
        {
            GD.Print("failed to load save file");
            return 0;
        }

        // load file
        File file = new File();
        file.Open(Utils.savePath+Utils.saveFile, File.ModeFlags.Read);

        string content = file.GetAsText(); // get content
        var jsonFile = JSON.Parse(content).Result as Dictionary; // convert content to generic dictionary

        int highScore = int.Parse(jsonFile["high_score"].ToString()); // get value of key
        
        file.Close();

        return highScore;
    }
}