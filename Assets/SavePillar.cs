using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SavePillar : MonoBehaviour {

    public int SaveGameIndex;
    public GameObject[] HeartContainers;
    public GameObject[] TriforcePieces;
    public SaveSword InStone;
    public SaveSword AboveStone;
    public Text Name;
    public Text NumDeaths;

    private bool CreatedSave;
    private bool loaded;
    private SaveVar<int>[] itemLevels = new SaveVar<int>[(int)ObjectList.prefabObjects.NumObjects];
    private SaveVar<bool>[] hasTriforce = new SaveVar<bool>[8];
    AllData data;

    // Use this for initialization

    // Update is called once per frame
    void Update ()
    {
	    if (!loaded || (CreatedSave && data == null))
        {
            Reset();
        }
	}

    private void Reset()
    {
        for (int i = 0; i < itemLevels.Length; i++)
        {
            itemLevels[i] = new SaveVar<int>(0);
        }
        for (int i = 0; i < hasTriforce.Length; i++)
        {
            hasTriforce[i] = new SaveVar<bool>(false);
        }
        AboveStone.SaveIndex = SaveGameIndex;
        InStone.Reset();
        InStone.gameObject.SetActive(true);
        AboveStone.gameObject.SetActive(false);
        CreatedSave = false;
        data = null;
        Load();
    }

    private void Load()
    {
        Debug.Log("Retrieving Save Pillar Data " + SaveGameIndex);
        data = SaveData.saveData.dataList[SaveGameIndex];
        if (data != null)
        {
//            Debug.Log("Data not Null");
            InStone.gameObject.SetActive(false);
            AboveStone.gameObject.SetActive(true);

//            Debug.Log("Registering Item Levels");
            data.registerIntArray("ItemLevels", itemLevels);
            AboveStone.UpdateLevel(itemLevels[(int)ObjectList.prefabObjects.Sword].val);

//            Debug.Log("Registering Triforces");
            data.registerBoolArray("HasTriforce", hasTriforce);
            for (int i = 0; i < TriforcePieces.Length; i++)
            {
                TriforcePieces[i].SetActive(hasTriforce[i].val);
            }

//            Debug.Log("Registering Heart Containers");
            for (int i = 0; i < data.pd.NumHeartContainers; i++)
            {
                HeartContainers[i].SetActive(true);
            }
            for (int i = (int)data.pd.NumHeartContainers; i < HeartContainers.Length; i++)
            {
                HeartContainers[i].SetActive(false);
            }
//            Debug.Log("Registering Name and Deaths");
            Name.enabled = true;
            Name.text = data.saveName;

            NumDeaths.enabled = true;
            NumDeaths.text = "" + data.pd.numDeaths;
        }
        else
        {
            Debug.Log("     --Data Null");
            InStone.gameObject.SetActive(true);
            AboveStone.gameObject.SetActive(false);

            for (int i = 0; i < TriforcePieces.Length; i++)
            {
                TriforcePieces[i].SetActive(false);
            }

            for (int i = 0; i < HeartContainers.Length; i++)
            {
                HeartContainers[i].SetActive(false);
            }

            Name.enabled = false;
            NumDeaths.enabled = false;
        }
        loaded = true;
    }

    public void Register(string name)
    {
        if (name.Trim().Equals(""))
        {
            CancelRegistration();
            return;
        }
        string realName = "";
        bool realChar = false;
        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (name[i] != ' ')
            {
                realChar = true;
            }
            if (realChar) realName = name[i] + realName;
        }
        SaveData.saveData.CreateSave(realName, SaveGameIndex);
        CreatedSave = true;
        data = null;
        InStone.Reset();
        InStone.gameObject.SetActive(false);
        Load();
    }

    public void CancelRegistration()
    {
        InStone.Reset();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (data != null && col.tag == "Bomb")
        {
            Vector3 v1 = col.transform.position;
            v1.y = 0;
            Vector3 v2 = transform.position;
            v2.y = 0;
            if (Vector3.Distance(v1, v2) < 0.75f)
            {
                SaveData.saveData.DeleteSave(SaveGameIndex);
                Reset();
            }
        }
    }
}
