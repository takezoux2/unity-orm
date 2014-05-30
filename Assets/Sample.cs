using UnityEngine;
using System.Collections;

using UnityORM;
using System;

public class Sample : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        SqliteInit.InitSqlite ();
    }

    private UserData[] InsertUserData ()
    {
        UserData[] data = new UserData[2];

        data [0] = new UserData ();
        data [0].ID = 1;
        data [0].Name = "Tarou";
        data [0].Hoge = "fuga";
        data [0].Age = 32;
        data [0].LastUpdated = new DateTime (2013, 4, 1);
        data [0].NestedClass.Fuga = "bbbb";
        data [0].NestedClass.Hoge = 23;

        data [1] = new UserData ();
        data [1].ID = 2;
        data [1].Name = "Jirou";
        data [1].Hoge = "wahoo";
        data [1].Age = 11;
        data [1].AddressData = "aaaaa";
        data [1].LastUpdated = new DateTime (2013, 5, 1);

        Debug.Log (data [0]);

        return data;
    }

    private void ShowInsertQuery ()
    {
        UserData[] data = InsertUserData ();
        var sqlMaker = new SQLMaker ();
        FieldLister lister = new FieldLister ();
        var info = lister.ListUp<UserData> ();
        Debug.Log (info);

        string insert = sqlMaker.GenerateInsertSQL (info, data [0]);
        Debug.Log ("Insert = " + insert);
    }

    private void ShowUpdateQuery ()
    {
        UserData[] data = InsertUserData ();
        var sqlMaker = new SQLMaker ();
        FieldLister lister = new FieldLister ();
        var info = lister.ListUp<UserData> ();
        Debug.Log (info);

        string update = sqlMaker.GenerateUpdateSQL (info, data [0]);
        Debug.Log ("Update = " + update);
    }

    private void ShowSelectAllJSON ()
    {
        UserData[] data = InsertUserData ();
        DBMapper mapper = new DBMapper (SqliteInit.Evolution.Database);
        mapper.UpdateOrInsertAll (data);
        UserData[] fromDb = mapper.Read<UserData> ("SELECT * FROM UserData;");
        Debug.Log (fromDb [0]);
        Debug.Log (fromDb [1]);

        JSONMapper jsonMapper = new JSONMapper ();
        string json = jsonMapper.Write<UserData> (fromDb);
        UserData[] fromJson = jsonMapper.Read<UserData> (json);
        Debug.Log ("Json = " + json);
        Debug.Log (fromJson [0]);
        Debug.Log (fromJson [1]);
    }

    // Update is called once per frame
    void Update ()
    {
    }

    void OnGUI ()
    {
        if (GUI.Button (new Rect (10, 10, 150, 50), "Insert Query"))
        {
            ShowInsertQuery ();
        }

        if (GUI.Button (new Rect (10, 70, 150, 50), "Update Query"))
        {
            ShowUpdateQuery ();
        }

        if (GUI.Button (new Rect (10, 130, 150, 50), "Select All JSON"))
        {
            ShowSelectAllJSON ();
        }
    }
}

public class UserData
{
    public long ID;
    public string Name;
    public int Age;

    [Ignore]
    public string Hoge{ get; set; }

    [MetaInfoAttirbute(NameInJSON = "address_data")]
    public string AddressData{ get; set; }

    public DateTime LastUpdated;
    public Nested NestedClass = new Nested ();

    public override string ToString ()
    {
        return "ID:" + ID + " Name:" + Name + " Hoge:" + Hoge + " Age:" + Age + " Address:" + AddressData +
            " LastUpdated:" + LastUpdated + " NestedClass:" + NestedClass;
    }
}

public class Nested
{
    public int Hoge;
    public string Fuga;

    public override string ToString ()
    {
        return "Hoge : " + Hoge + " Fuga:" + Fuga;
    }
}
