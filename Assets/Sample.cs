using UnityEngine;
using System.Collections;

using UnityORM;

public class Sample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SqliteInit.InitSqlite();
		
		
		FieldLister lister = new FieldLister();
		
		UserData data = new UserData();
		data.ID = 2;
		data.Name = "hoge";
		data.Hoge = "fuga";
		
		var info = lister.listUp<UserData>();
		
		Debug.Log(info);
		
		var sqlMaker = new SQLMaker();
		string insert = sqlMaker.GenerateInsertSQL(info,data);
		string update = sqlMaker.GenerateUpdateSQL(info,data);
		
		Debug.Log("Insert = " + insert);
		Debug.Log("Update = " + update);
		
		DBMapper mapper = new DBMapper(SqliteInit.Evolution.Database);
		
		mapper.UpdateOrInsert(data);
		
		UserData[] fromDb = mapper.Read<UserData>("SELECT * FROM UserData WHERE ID={0}".SQLFormat(2));
		Debug.Log(fromDb[0].Hoge);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class UserData{
	public int ID;
	
	[Ignore]
	public string Name;
	
	public string Hoge{get;set;}
}