using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System; 
using System.IO;

public class DataLinker : MonoBehaviour
{
    string loadedDatabaseName;
    string conn;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader; 
    public string defaultDatabase;


    private void Awake()
    {
        loadedDatabaseName = defaultDatabase;
        conn = "URI=file:" + Application.dataPath + "/" + loadedDatabaseName + ".db";

        if ((File.Exists(Application.dataPath + "/" + loadedDatabaseName + ".db"))  == false  )
            Debug.LogWarning("Failed to load database!"); 
    }
  


    // ---------------- DATABASE RETRIEVING

    // Returns a string-list of all table names in database
    public List<string> GetTableNames()
    {
        List<string> returnList = new List<string>(); 
        OpenDatabase(); 
        string sqlQuery = "SELECT name FROM sqlite_master WHERE type = 'table'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())        
            returnList.Add(reader.GetValue(0).ToString());        
        CloseDatabase();
        return returnList;
    }


    // Returns a string-list of the names of the fields for a table
    public List<string> GetFieldNamesForTable(string tableName)
    {
        List<string> returnList = new List<string>();

        OpenDatabase();

        // string sqlQuery = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
        string sqlQuery = "SELECT * FROM " + tableName;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        for (int i = 0; i < reader.FieldCount; i++)        
            returnList.Add(reader.GetName(i).ToString());         
        CloseDatabase();
        return returnList;
    }


    // Returns a stirng-list of all values for a specific field in a specific table
    public List<string> GetFieldValuesForTable(string tableName, string fieldName)
    {
        List<string> returnList = new List<string>();
        OpenDatabase();

        string sqlQuery = "SELECT "+fieldName+" FROM " + tableName;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            if (reader.IsDBNull(0)) // seems unnecessary
                returnList.Add("NULL");
            else
                returnList.Add(reader.GetValue(0).ToString());
        }
        CloseDatabase();
        return returnList;
    }
    // Same as above but preset to ID for convenience
    public List<string> GetIDValuesForTable(string tableName)
    {
        List<string> returnList = new List<string>();
        OpenDatabase();

        string sqlQuery = "SELECT ID FROM " + tableName;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())        
                returnList.Add(reader.GetValue(0).ToString());        
        CloseDatabase();
        return returnList;
    }


    // Returns a string-list of all values for a specific ID in a specific table
    public List<string> GetValuesForID(string tableName, string ID)
    {
        List<string> returnList = new List<string>();
        OpenDatabase();
        string sqlQuery = "SELECT * FROM " + tableName + " WHERE ID='" + ID + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        for (int i = 0; i < reader.FieldCount; i++)
            returnList.Add(reader.GetValue(i).ToString());
        CloseDatabase();
        return returnList;
    }

    // Returns a string value for a specific field in a specific table using ID value
    public string GetEntryForTableAndFieldWithID(string tableName, string fieldName, string ID )
    { 
        string returnString = "NotFound";
        OpenDatabase(); 
        string sqlQuery = "SELECT " + fieldName + " FROM " + tableName + " WHERE ID='" + ID + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())        
            returnString = reader.GetValue(0).ToString();        
        CloseDatabase();
        return returnString;
    }














    // ---------------- DATABASE EDITING

    //public void CreateNewElement(string tableName, string fieldName, string elementName, string value)
    //{
    //    OpenDatabase();

    //    dbcmd.CommandText = "INSERT INTO " + tableName + " VALUES ('" + elementName + "','" + value + "','" + "nully" + "','" + "eh" + "')";


    //    dbcmd.ExecuteNonQuery();

    //    CloseDatabase();
    //}


    // ---------------- HELPER FUNCTIONS

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
    public void ExecuteQuery(string query)
    {
        OpenDatabase();

        dbcmd.CommandText = query;

        dbcmd.ExecuteNonQuery();

        CloseDatabase();
    }
    public void DatabaseTestScan(bool scanByID)
    {
        if (scanByID == true)
        {
            foreach (string table in GetTableNames())
                foreach (string id in GetFieldValuesForTable(table, "ID"))
                    foreach (string value in GetValuesForID(table, id))
                        Debug.Log("Table " + table + " has id: " + id + " and value " + value);
        }
        else // scan by column
        {
            foreach (string table in GetTableNames())
                foreach (string field in GetFieldNamesForTable(table))
                    foreach (string fieldValue in GetFieldValuesForTable(table, field))
                        Debug.Log("TABLE: " + table + " has FIELD: " + field + " with VALUE: " + fieldValue);
        }
    }
}
