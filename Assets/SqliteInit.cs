using System;
using UnityEngine;
using System.IO;
using UnityORM;
using System.Collections.Generic;

public static class SqliteInit
{
    public static DBEvolution Evolution;
    static bool alreadyInited = false;
    
    public static void InitSqlite ()
    {
        if (alreadyInited)
        {
            return;
        }
        alreadyInited = true;

        string path = Application.dataPath + "/StreamingAssets";
        Evolution = new DBEvolution (
            //Path.Combine(Application.persistentDataPath,"sample_data.db"));
            Path.Combine (path, "sample_data.db"));

        Evolution.RecreateTableIfHashDiffers = true;// You shoud set false when you release application to avoid suddon drop table.

        // Init table
        Evolution.Evolute ("SampleTable",// Traget table
            new List<string> (){ // These sql is executed at once.
            @"CREATE TABLE SampleTable(id INTEGER,nickname TEXT,lastLogin INTEGER, score FLOAT);",
            @"ALTER TABLE SampleTable ADD COLUMN age INTEGER;"
        });

        Evolution.Evolute ("OtherTable",
            new List<string> (){
            @"CREATE TABLE OtherTable(key TEXT,val TEXT);",
            @"INSERT INTO OtherTable VALUES ('a','b')"
        });

        Evolution.Evolute ("UserData",
            new List<string> (){
            //@"CREATE TABLE UserData(id INTEGER PRIMARY_KEY,name TEXT,hoge TEXT); "
            new SQLMaker().GenerateCreateTableSQL<UserData>(ClassDescRepository.Instance.GetClassDesc<UserData>()) + " "
        });
    }
}
