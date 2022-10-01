using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpreadSheet
{
    public string name;
    public string ID;
    public List<Sheet> sheets;
    public SpreadSheet(string name, SheetInfo[] sheets, MonoBehaviour downloadStarter)
    {
        this.name = name;
        this.sheets = new List<Sheet>();
        for (int i = 0; i < sheets.Length; i++)
        {
            this.sheets.Add(new Sheet(sheets[i].name, sheets[i].rawURL, sheets[i].onDoneDownloading, downloadStarter));
        }

        ID = Sheet.RawURLtoSpreadsheetID(sheets[0].rawURL);
    }

    public class Sheet
    {
        public string name;
        public string ID_spreadsheet;
        public string ID_sheet;
        public string rawURL;
        public Action onDoneDownloading;
        public string[,] data;

        public Sheet(string name, string rawURL, Action<string[,]> onDone, MonoBehaviour downloadStarter)
        {
            this.name = name;

            ID_spreadsheet = RawURLtoSpreadsheetID(rawURL);
            ID_sheet = RawURLtoSheetID(rawURL);

            this.rawURL = rawURL;

            downloadStarter.StartCoroutine(Download(RawURLtoCSVLink(rawURL), onDone));
        }

        IEnumerator Download(string csvDownloadURL, Action<string[,]> onDone)
        {
            UnityWebRequest request = UnityWebRequest.Get(csvDownloadURL);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                throw new Exception("Download error: " + request.error);
            }
            else
            {
                PopulateData(request.downloadHandler.text);
                onDone?.Invoke(data);
            }

            request.Dispose();
        }

        void PopulateData(string sourceCSVData)
        {
            string[] rows = sourceCSVData.Replace("\r", "").Split('\n');

            string[,] data = new string[rows[0].Split(',').Length, rows.Length];

            for (int y = 0; y < rows.Length; y++)
            {
                string[] rowCells = rows[y].Split(',');

                for (int x = 0; x < rowCells.Length; x++)
                {
                    try
                    {
                        data[x, y] = rowCells[x];
                    }
                    catch
                    {
                        Debug.Log("There's probably a \",\" in a cell in the spreadsheet");
                    }
                }
            }

            this.data = data;
        }

        public static string RawURLtoSpreadsheetID(string url)
        {
            string raw = url;
            string[] split = raw.Split('/');
            string ssID = split[split.Length - 2];
            return ssID;
        }

        public static string RawURLtoSheetID(string url)
        {
            string raw = url;
            string[] split = raw.Split('/');
            string sID;

            if(split[split.Length - 1].Contains("gid="))
            {
                string[] subSplit = split[split.Length - 1].Split('=');
                sID = subSplit[subSplit.Length - 1];
            }
            else
            {
                throw new Exception("Raw URL \"" + raw + "\" did not contain a sheet ID. (Identified by the presence of \"gid=\")");
            }

            return sID;
        }

        public static string RawURLtoCSVLink(string url)
        {
            string sID = RawURLtoSheetID(url);
            string ssID = RawURLtoSpreadsheetID(url);
            string result = "http://docs.google.com/spreadsheets/d/" + ssID + "/export?format=csv&gid=" + sID;
            return result;
        }
    }

    public struct SheetInfo
    {
        public string name;
        public string rawURL;
        public Action<string[,]> onDoneDownloading;
    }
}