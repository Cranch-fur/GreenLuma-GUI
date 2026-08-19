using System;
using System.IO;
using System.Collections.Generic;

namespace GreenLumaGUI
{
    public static class LumaManager
    {
        private const string localDynamicLibraryPath = "GreenLuma.dll";
        public static string GetLocalDynamicLibraryPath()
        {
            return localDynamicLibraryPath;
        }


        public static string GetDynamicLibraryPath()
        {
            string clientPath = SteamManager.GetClientPath();
            if (clientPath == null || Directory.Exists(clientPath) == false)
                return null;


            return Path.Combine(clientPath, "user32.dll");
        }






        public static bool IsInstalled()
        {
            string dynamicLibraryPath = GetDynamicLibraryPath();
            if (dynamicLibraryPath == null || File.Exists(dynamicLibraryPath) == false)
                return false;


            return true;
        }






        public static string GetGamesListDirectoryPath()
        {
            string clientPath = SteamManager.GetClientPath();
            if (string.IsNullOrEmpty(clientPath) || Directory.Exists(clientPath) == false)
                return null;


            return Path.Combine(clientPath, "AppList");
        }

        public static string GetGamesListFilePath()
        {
            string gamesListDirectory = GetGamesListDirectoryPath();
            if (string.IsNullOrEmpty(gamesListDirectory))
                return null;

            return Path.Combine(gamesListDirectory, "AppList.ini");
        }


        public static List<string> GetGamesList()
        {
            string filePath = GetGamesListFilePath();
            if (string.IsNullOrEmpty(filePath) || File.Exists(filePath) == false)
                return null;


            List<string> gamesList = new List<string>();
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("["))
                        continue;

                    string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        string appIdString = parts[1].Trim();

                        if (Int64.TryParse(appIdString, out long appId))
                            gamesList.Add(appIdString);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowException(ex);
                return new List<string>();
            }


            return gamesList;
        }


        public static bool WriteGamesList(List<string> gamesList)
        {
            string directoryPath = GetGamesListDirectoryPath();
            if (string.IsNullOrEmpty(directoryPath))
                return false;

            string filePath = GetGamesListFilePath();
            if (string.IsNullOrEmpty(filePath))
                return false;
                

            if (Directory.Exists(directoryPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowException(ex);
                    return false;
                }
            }


            try
            {
                List<string> linesToWrite = new List<string>();
                linesToWrite.Add("[AppList]");

                int gamesCount = gamesList.Count;
                for (int i = 0; i < gamesCount; i++)
                {
                    linesToWrite.Add($"{i} = {gamesList[i]}");
                }

                File.WriteAllLines(filePath, linesToWrite);
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowException(ex);
                return false;
            }


            return true;
        }


        public static bool ClearGamesList()
        {
            string filePath = GetGamesListFilePath();
            if (string.IsNullOrEmpty(filePath))
                return false;

            if (File.Exists(filePath) == false)
                return true;

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowException(ex);
                return false;
            }
        }
    }
}
