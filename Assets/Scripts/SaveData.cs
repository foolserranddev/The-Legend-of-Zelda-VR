using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class SaveData : MonoBehaviour
{
    public static float SAVE_VERSION = 0.03f;
    public static SaveData saveData;
    public AllData data;

    public AllData [] dataList = new AllData[3];
    public bool OpeningData = false;
    public bool Reloading = false;
    private int saveIndex = 0;

    public void Awake()
    {
        if (saveData == null)
        {
            saveData = this;
        }
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartOver();
        }
        else
        {
            data = Load("SaveData.dat");
            if (data == null) data = new AllData();
            ObjectList.objectList.Initialize();
        }
    }

    public void StartOver()
    {
        saveData.data = new AllData();
        dataList[0] = Load("SaveData0.dat");
        dataList[1] = Load("SaveData1.dat");
        dataList[2] = Load("SaveData2.dat");
        ObjectList.objectList.UpdateFromSave();
    }


    public void SelectSavedata(int d)
    {
        saveIndex = d;
        data = dataList[d];
        ObjectList.objectList.UpdateFromSave();
        SceneManager.LoadScene(1);
    }

    public void Reload()
    {
        Debug.Log("Reloading " + data.fileName);
        data = Load(data.fileName);
        if (data != null)
        {
            Reloading = true;
            ObjectList.objectList.UpdateFromSave();
            SceneManager.LoadScene(1);
        }
        else
        {
            //TODO: Flash an Error
            Debug.Log("Null Save File");
        }
    }

    void Update()
    {
        Reloading = false;
    }

    public AllData Load(string filename)
    {
        Reloading = false;
        AllData d = null;
        try
        {
            if (File.Exists(Application.persistentDataPath + "/" + filename))
            {
                Debug.Log("Loading " + Application.persistentDataPath + "/" + filename);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + filename, FileMode.Open);
                d = (AllData)bf.Deserialize(file);
                d.fileName = filename;
            }
            else
            {
                Debug.Log(filename + " Save Data Not Found");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            Debug.Log("Caught Exception in Loading Save Data " + filename);
            d = null;
        }
        if (d == null || d.saveVersion != SAVE_VERSION)
        {
            if (d != null) Debug.Log("Save Version Didn't Match for " + filename + ". Starting from Scratch");
        }

        return d;
    }

    public void Save()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 0 || data == null || data.fileName == "") return;
        data.pd = Player.player.pd;
        Debug.Log("Saving " + Application.persistentDataPath + "/" + data.fileName);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + data.fileName);
            bf.Serialize(file, data);
            file.Close();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            Debug.Log("Caught Exception Trying to Save file " + data.fileName);
        }
    }

    public void CreateSave(string name, int index)
    {
        Debug.Log("Creating Save for " + name + " on SaveData" + index + ".dat");
        dataList[index] = new AllData("SaveData" + index + ".dat");
        dataList[index].saveName = name;
        data = dataList[index];
        Player.player.pd = data.pd;
        Save();
    }

    public void DeleteSave(int index)
    {
        if(File.Exists(Application.persistentDataPath + "/backupSave.dat"))
        {
            File.Delete(Application.persistentDataPath + "/backupSave.dat");
        }
        File.Move(Application.persistentDataPath + "/SaveData" + index + ".dat", Application.persistentDataPath + "/backupSave.dat");
        dataList[index] = null;
    }
}

[System.Serializable]
public class AllData
{
    public float saveVersion = SaveData.SAVE_VERSION;
    public string fileName;
    public string saveName = "Link";
    public Player.PlayerData pd = new Player.PlayerData();
    private Dictionary<string, SaveVar<bool>> bools = new Dictionary<string, SaveVar<bool>>();
    private Dictionary<string, SaveVar<bool>[]> boolArrays = new Dictionary<string, SaveVar<bool>[]>();
    private Dictionary<string, SaveVar<int>> ints = new Dictionary<string, SaveVar<int>>();
    private Dictionary<string, SaveVar<int>[]> intArrays = new Dictionary<string, SaveVar<int>[]>();

    public AllData(string name)
    {
        fileName = name;
    }

    public AllData() { }

    public void registerBool(string s, SaveVar<bool> sv)
    {
        if (bools.ContainsKey(s))
        {
            sv.val = bools[s].val;
            bools[s] = sv;
        }
        else
        {
            bools.Add(s, sv);
        }
    }

    public void registerBoolArray(string s, SaveVar<bool>[] sv)
    {
        if (boolArrays.ContainsKey(s))
        {
            SaveVar<bool>[] ba = boolArrays[s];
            for (int i = 0; i < ba.Length; i++)
            {
                sv[i].val = ba[i].val;
            }
            boolArrays[s] = sv;
        }
        else
        {
            boolArrays.Add(s, sv);
        }
    }

    public void registerInt(string s, SaveVar<int> sv)
    {
        if (ints.ContainsKey(s))
        {
            sv.val = ints[s].val;
            ints[s] = sv;
        }
        else
        {
            ints.Add(s, sv);
        }
    }

    public void registerIntArray(string s, SaveVar<int>[] sv)
    {
        if (intArrays.ContainsKey(s))
        {
            SaveVar<int>[] ia = intArrays[s];
            for (int i = 0; i < ia.Length; i++)
            {
                sv[i].val = ia[i].val;
            }
            intArrays[s] = sv;
        }
        else
        {
            intArrays.Add(s, sv);
        }
    }

}

[System.Serializable]
public class SaveVar<T>
{
    public T val;

    public SaveVar(T v)
    {
        val = v;
    }
}



