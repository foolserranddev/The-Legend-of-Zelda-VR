using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour {

    public ObjectList.prefabObjects prefabEnum;
    public Material[] material;
    public Sprite [] StatusWindowIcons;
    public int itemLevelIndex = 0;
    public Vector3 InitialPositionOffset = Vector3.zero;
    public Vector3 InitialRotationOffset = Vector3.zero;
    public bool isHoldable = false;
    public HandController.HandPositionsEnum handPosition = HandController.HandPositionsEnum.Sword;

    protected HandController mainHand;

    // Use this for initialization
    public virtual void Start ()
    {
        if (itemLevelIndex != 0) // If 0, keep with the default.
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null) mr.material = material[itemLevelIndex];
        }
    }
    
    public virtual void OnEnable()
    {
        updateMesh(itemLevelIndex);
    }

    private void updateMesh(int level)
    {
        if (material.Length < 1) return;
        itemLevelIndex = level;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.material = material[itemLevelIndex];
    }

    public virtual Sprite GetSprite()
    {
        return StatusWindowIcons[itemLevelIndex];
    }

    public virtual void UpdateLevel(int level)
    {
        itemLevelIndex = level;
        if (ObjectList.objectList != null) ObjectList.objectList.itemLevels[(int)prefabEnum].val = level;
        if (StatusWindow.statusWindow != null) StatusWindow.statusWindow.UpdateSprite(prefabEnum);
        if (ItemFrame.itemFrame != null) ItemFrame.itemFrame.UpdateImages();
        updateMesh(level);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public virtual ObjectList.prefabObjects ObtainPrefab() { return prefabEnum; }

    public virtual void performAction() { }

    //public virtual void performTriggerClicked(HandController hand, ClickedEventArgs e) { }

    //public virtual void performTriggerUnclicked(HandController hand, ClickedEventArgs e) { }

    public virtual void InitialSetup(HandController MainHand, HandController Offhand)
    {
        if (MainHand != null) mainHand = MainHand;
        transform.SetParent(mainHand.transform);
        transform.localPosition = InitialPositionOffset;
        transform.localEulerAngles = InitialRotationOffset;
        mainHand.GetComponentInChildren<Animator>().SetInteger("HandPosition", (int)handPosition);
    }

    public virtual float damageDealt() { return 0; }

    public virtual void TurnOff() { transform.SetParent(ObjectList.objectList.gameObject.transform); gameObject.SetActive(false); }
    public virtual void TurnOn() { InitialSetup(null, null); gameObject.SetActive(true); }
}
