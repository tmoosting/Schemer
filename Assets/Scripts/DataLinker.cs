using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System; 
using System.IO;

public class DataLinker : MonoBehaviour
{
    public static DataLinker Instance;

    string loadedDatabaseName;
    string conn;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader; 
    public string defaultDatabase;


    private void Awake()
    {
        Instance = this;
        loadedDatabaseName = defaultDatabase;
        conn = "URI=file:" + Application.dataPath + "/" + loadedDatabaseName + ".db";

        if ((File.Exists(Application.dataPath + "/" + loadedDatabaseName + ".db"))  == false  )
            Debug.LogWarning("Failed to load database!");



        foreach (string table in GetTableNames())
        {
            foreach (string field in GetFieldNamesForTable(table))
            {
                Debug.Log("Table " + table + " has field: " + field);
            } 
        }
    }


    public List<string> GetTableNames()
    {
        List<string> returnList = new List<string>(); 
        OpenDatabase(); 
        string sqlQuery = "SELECT name FROM sqlite_master WHERE type = 'table'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            returnList.Add((string)reader.GetValue(0));
        }
        CloseDatabase();
        return returnList;
    }


    public List<string> GetFieldNamesForTable(string tableName)
    {
        List<string> returnList = new List<string>();

        OpenDatabase();

        // string sqlQuery = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
        string sqlQuery = "SELECT * FROM " + tableName;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            returnList.Add((string)reader.GetName(i)); 
        }
        CloseDatabase();
        return returnList;
    }


























    // HELPER FUNCTIONS
    void OpenDatabase()
    {
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
    }
    void CloseDatabase()
    {
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
    }
}
