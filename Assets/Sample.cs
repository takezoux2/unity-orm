using UnityEngine;
using System.Collections;

using UnityORM;

public class Sample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SqliteInit.InitSqlite();
		
		
		FieldLister lister = new FieldLister();
		
		UserData[] data = new UserData[2];
		
		data[0] = new UserData();
		data[0].ID = 1;
		data[0].Name = "Tarou";
		data[0].Hoge = "fuga";
		data[0].Age = 32;
		
		data[1] = new UserData();
		data[1].ID = 2;
		data[1].Name = "Jirou";
		data[1].Hoge = "wahoo";
		data[1].Age = 11;
		data[1].AddressData = "aaaaa";
		
		
		
		
		var info = lister.listUp<UserData>();
		
		Debug.Log(info);
		
		var sqlMaker = new SQLMaker();
		string insert = sqlMaker.GenerateInsertSQL(info,data[0]);
		string update = sqlMaker.GenerateUpdateSQL(info,data[0]);
		
		Debug.Log("Insert = " + insert);
		Debug.Log("Update = " + update);
		
		DBMapper mapper = new DBMapper(SqliteInit.Evolution.Database);
		
		mapper.UpdateOrInsertAll(data);
		
		UserData[] fromDb = mapper.Read<UserData>("SELECT * FROM UserData;");
		Debug.Log(fromDb[0]);
		Debug.Log(fromDb[1]);
		
		
		JSONMapper jsonMapper = new JSONMapper();
		
		string json = jsonMapper.Write<UserData>(fromDb);
		UserData[] fromJson = jsonMapper.Read<UserData>(json);
		
		Debug.Log("Json = " + json);
		
		Debug.Log(fromJson[0]);
		Debug.Log(fromJson[1]);
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class UserData{
	public long ID;
	
	public string Name;
	
	public int Age;
	
	[Ignore]
	public string Hoge{get;set;}
	
	[MetaInfoAttirbute(NameInJSON = "address_data")]
	public string AddressData{get;set;}
	
	
	public override string ToString ()
	{
		return "ID:" + ID + " Name:" + Name + " Hoge:" + Hoge + " Age:" + Age + " Address:" + AddressData;
	}
}