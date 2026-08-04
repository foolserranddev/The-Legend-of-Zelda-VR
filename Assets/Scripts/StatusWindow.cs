using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour {

    public static StatusWindow statusWindow;
    public AudioClip FillLoopSound;
    public AudioClip EndLoopSound;
    public AudioClip RupeeSound;
    public AudioClip FiveRupeeSound;
    public Sprite FullHeart;
    public Sprite HalfHeart;
    public Sprite EmptyHeart;
    public Sprite[] Digits;
    public GameObject GreenDot;
    public GameObject RedDot;
    public SpriteRenderer PrimaryItemRenderer;
    public SpriteRenderer SecondaryItemRenderer;
    public SpriteRenderer MapRenderer;
    public Map map;
    public GameObject TriforcePieces_ExplorationMap;
    public Text levelText;
    public SpriteRenderer tally1;
    public SpriteRenderer tally2;
    public Sprite[] tallies;

    [HideInInspector] public SpriteRenderer[] LifeSprites;
    [HideInInspector] public SpriteRenderer[] RupeeSprites;
    [HideInInspector] public SpriteRenderer[] KeySprites;
    [HideInInspector] public SpriteRenderer[] BombSprites;

    private SpriteRenderer CompassRenderer;
    private AudioSource audioSource;
    private float fillLoopTime;
    private float lifeToFill = 0;
    private bool mapShowing;
    private bool compassOn;
    private int primaryItemIdx = -1;
    private int secondaryItemIdx = -1;
    private int prevKills;
    private bool initialized = false;

    // Use this for initialization
    void Awake()
    {
        if (statusWindow == null) statusWindow = this;
        else
        {
            statusWindow.map = map;
            statusWindow.mapShowing = false;
            statusWindow.MapRenderer.sprite = null;
            statusWindow.levelText.text = levelText.text;
            Destroy(gameObject);
        }
        
        audioSource = GetComponent<AudioSource>();
        CompassRenderer = RedDot.GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        if (Player.player.hitlessKillCount != prevKills)
        {
            prevKills = Player.player.hitlessKillCount;
            tally1.sprite = prevKills > 0 ? tallies[Mathf.Min(prevKills, 5) - 1] : null;
            tally2.sprite = prevKills > 5 ? tallies[prevKills - 1] : null;
        }
        GreenDot.SetActive(!Player.player.InDungeon || !Player.player.isUnderground);
        GreenDot.transform.localPosition = map.greenDotPosition;
        if (map.playerHasMap && !mapShowing)
        {
            MapRenderer.sprite = map.mapSprite;
            mapShowing = true;
        }
        else if (!map.playerHasMap && mapShowing)
        {
            MapRenderer.sprite = null;
            mapShowing = false;
        }
        if (compassOn && !map.playerHasCompass)
        {
            CompassRenderer.enabled = false;
            compassOn = false;
        }
        else if (!compassOn && map.playerHasCompass)
        {
            CompassRenderer.enabled = true;
            compassOn = true;
            RedDot.transform.localPosition = map.compassPosition;
        }

        if (fillLoopTime != 0 && Time.time > fillLoopTime)
        {
            audioSource.clip = EndLoopSound;
            audioSource.loop = false;
            audioSource.Play();
            fillLoopTime = 0;
        }
        else if (Time.time < fillLoopTime)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = FillLoopSound;
                audioSource.Play();
            }
        }
        if (Player.player.Dead) lifeToFill = 0;
        if (lifeToFill > 0)
        {
            float lifeChange = 1f * Time.deltaTime;
            lifeToFill -= lifeChange;
            Player.player.LifeRemaining = Mathf.Min(Player.player.LifeRemaining + lifeChange, Player.player.pd.NumHeartContainers);
            UpdateLife();
            if (Player.player.AtMaxHealth || lifeToFill <= 0)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = EndLoopSound;
                audioSource.Play();
                lifeToFill = 0;
            }
            else if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = FillLoopSound;
                audioSource.Play();
            }
        }

    }

    public void UpdateLife()
    {
        int numHearts = (int)Player.player.LifeRemaining;
        if (Player.player.LifeRemaining - numHearts > 0.5) numHearts++;
        bool halfHeart = Player.player.LifeRemaining - numHearts > 0f;
        for (int i = 0; i < LifeSprites.Length; i++)
        {
            if (i < numHearts) LifeSprites[i].sprite = FullHeart;
            else if (i < Player.player.pd.NumHeartContainers) LifeSprites[i].sprite = EmptyHeart;
            else LifeSprites[i].sprite = null;
        }
        if (halfHeart)
        {
            LifeSprites[numHearts].sprite = HalfHeart;
        }
    }

    public bool isLifeFilling()
    {
        return lifeToFill > 0;
    }

    public void fillLife()
    {
        lifeToFill = Player.player.pd.NumHeartContainers - Player.player.LifeRemaining;
    }

    public void UpdateNumber(SpriteRenderer[] CountedItem, int num)
    {
        if (num < 0)
        {
            Debug.Log("Issue Updating Number in Status Window. Value = " + num);
            return;
        }
        int h = num / 100;
        int t = (num - (h * 100)) / 10;
        int o = num - (h * 100) - (t * 10);
        if (h > 0)
        {
            CountedItem[0].sprite = Digits[h];
            CountedItem[1].sprite = Digits[t];
            CountedItem[2].sprite = Digits[o];
        }
        else if (t > 0)
        {
            CountedItem[0].sprite = Digits[t];
            CountedItem[1].sprite = Digits[o];
            if (CountedItem[2] != null) CountedItem[2].sprite = null;
        }
        else
        {
            CountedItem[0].sprite = Digits[o];
            CountedItem[1].sprite = null;
            if (CountedItem[2] != null) CountedItem[2].sprite = null;
        }
    }

    public void UpdateRupees(int num)
    {
        UpdateNumber(RupeeSprites, Player.player.pd.NumRupees);
        if (num == 0) return;
        if (Mathf.Abs(num) > 5)
        {
            audioSource.clip = FillLoopSound;
            audioSource.loop = true;
            fillLoopTime = Time.time + Mathf.Abs(num) * 0.015f;
        }
        else if (num == 5)
        {
            audioSource.loop = false;
            audioSource.clip = FiveRupeeSound;
        }
        else if (num == 1)
        {
            audioSource.loop = false;
            audioSource.clip = RupeeSound;
        }
        if (num != -1)
        {
            audioSource.Play();
        }
    }

    public void UpdateBombs()
    {
        UpdateNumber(BombSprites, Player.player.pd.NumBombs);
        UpdateSprite(ObjectList.prefabObjects.Bomb);
        ItemFrame.itemFrame.UpdateImages();
    }

    public void UpdateKeys()
    {
        if (ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.LionKey].val)
        {
            KeySprites[0].sprite = Digits[10];
            KeySprites[1].sprite = null;
        }
        else
        {
            UpdateNumber(KeySprites, Player.player.pd.NumKeys);
        }
    }

    public void Refresh()
    {
        UpdateKeys();
        UpdateBombs();
        UpdateRupees(0);
        UpdateLife();
    }

    public void UpdateSprite(ObjectList.prefabObjects item)
    {
        //Debug.Log("UpdateSprite with item " + item);
        UpdateSprite((int)item);
    }

    public void UpdateSprite(int item)
    {
        if (item < 0)
        {
            if (primaryItemIdx < 0) PrimaryItemRenderer.sprite = null;
            if (secondaryItemIdx < 0) SecondaryItemRenderer.sprite = null;
            return;
        }
        //Debug.Log("UpdateSprite with index " + item);
        //Debug.Log("Current Primary is " + primaryItemIdx + " and Current Secondary is " + secondaryItemIdx);
        InteractableObject io = ObjectList.objectList.prefabs[item].GetComponent<InteractableObject>();
        if (item == primaryItemIdx)
        {
            PrimaryItemRenderer.sprite = io.StatusWindowIcons[io.itemLevelIndex];
        }
        if (item == secondaryItemIdx)
        {
            SecondaryItemRenderer.sprite = io.StatusWindowIcons[io.itemLevelIndex];
        }
    }

    public void UpdatePrimaryItem(int objectListPrefabIndex)
    {
        Debug.Log("Update Primary to item " + objectListPrefabIndex);
        primaryItemIdx = objectListPrefabIndex;
        UpdateSprite(objectListPrefabIndex);
    }

    public void UpdateSecondaryItem(int objectListPrefabIndex)
    {
        Debug.Log("Update Secondary to item " + objectListPrefabIndex);
        secondaryItemIdx = objectListPrefabIndex;
        UpdateSprite(objectListPrefabIndex);
    }
}
