using SQLite4Unity3d;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public class DBManagerG {

    private SQLiteConnection _connection;

    public static string sql;

    public DBManagerG(string DatabaseName)
    {

#if UNITY_EDITOR
        var dbPath = string.Format(@"{0}/{1}", Application.streamingAssetsPath ,DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
    }

    ~DBManagerG()
    {
        _connection.Close();
    }

    public void CreateTable<T>(string tbName)
    {  }

    public void Query()
    {
        Debug.Log("Query:");

        //var query = _connection.Table<T>().Where(v => v.Id.Equals(theId));
        //_connection.CreateTable

        _connection.CreateTable<Stock>();

        Stock stock = new Stock { Symbol = "22", type = eType.monday };
        Stock stock2 = new Stock { Symbol = "11", type = eType.friday };

        _connection.Insert(stock);
        _connection.Insert(stock2);

        stock.Symbol = "33";
        _connection.Update(stock);

        //string tableName = "Stock";
        //string sql = string.Format(@"SELECT * FROM {0}", tableName);
        //var query1 = _connection.Query((TableMapping)Stock, sql);


        List<Stock> stocks = _connection.Table<Stock>().ToList();
        //var stocks = _connection.Table<Stock>();

        //List<Stock> stocks = _connection.Query<Stock>("SELECT * FROM S");

        //Debug.Log(stocks.ToList<Stock>()[2].Symbol);

        foreach (var a in stocks)
        {
            Debug.Log("Stock: " + a.Symbol);
            Debug.Log("Enum.Type: " + a.type.ToString());
        }

        string sql = string.Format("SELECT * FROM S WHERE type={0}", "\'monday\'");
        Debug.Log(sql);
        List<Stock> m_stock = _connection.Query<Stock>(sql);

        Debug.Log(m_stock[0].type.ToString());

        _connection.DropTable<Stock>();
    }

    public int Excute(string theQuery)
    {
        int s = _connection.Execute(theQuery);

        return s;
    }

}

[Table ("S")]
public class Stock
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [MaxLength(8)]
    public string Symbol { get; set; }
    [EnumType, Column("type")]
    public eType type { get; set; }

    public void ISay(string theWord)
    {
        Debug.Log("i am a boy");
    }
}

[Table ("V")]
public class Valuation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int StockId { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
}

public enum eType
{
    sunday,
    monday,
    wed,
    friday
}
