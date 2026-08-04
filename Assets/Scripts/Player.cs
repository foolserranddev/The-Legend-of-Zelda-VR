using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Player : MonoBehaviour {

    public static Player player;

    public bool starterPlayer = false;

    [Header("Starting Goods")]
    public float PLAYER_SPEED = 5;
    public bool allowFlight = false;
    public float maxAllowableVerticalLift = 2;
    public bool startWithStuff = false;
    public bool startWithSword = false;
    public bool startWithArrows = false;
    public int startWithBombs = 0;
    public int startWithRupees = 0;
    public int startWithHearts = 0;
    public int startWithKeys = 0;

    public int[] levels = new int[(int)ObjectList.prefabObjects.NumObjects];
    public bool[] hasItems = new bool[(int)ObjectList.prefabObjects.NumObjects];
    public bool[] startingTriforcePieces = new bool[8];

    [Header("Body Parts")]
    public HandController leftHand;
    public HandController rightHand;
    public MeshRenderer leftPalm;
    public MeshRenderer rightPalm;
    public MeshRenderer leftWrist;
    public MeshRenderer rightWrist;
    public Material[] palms;
    public Material[] wrists;

    [Header("Audio Clips")]
    public AudioClip hurtClip;
    public AudioClip newItemClip;
    public AudioClip lowHealthClip;
    public AudioClip deathMusic;

    [Header("Other")]

    public float LifeRemaining = 3;
    public bool AtMaxHealth { get { return LifeRemaining > (float)pd.NumHeartContainers - 0.5; } }
    public string quadrant = "A8";
    public int roomTransitionCount = 0;
    const int ROOM_HISTORY_LENGTH = 6;
    public string[] pastRooms = new string[ROOM_HISTORY_LENGTH];

    [HideInInspector]
    public PlayerData pd = new PlayerData();
    public SaveVar<bool> HasBigShield = new SaveVar<bool>(false);

    public bool isMobile = true;
    public Vector3 dungeonReturnLocation;
    public Vector3 playerPlayspaceOffset;

    public bool returnFromDungeon;
    public bool InDungeon = false;
    public bool isUnderground = false;
    public bool NoHitRollover;

    private bool initialized = false;
    private StatusWindow statusWindow;
    private AudioSource worldMusicAudioSource;
    private AudioSource hurtAudioSource;
    private AudioSource lowHealthAudioSource;
    private AudioSource itemAudioSource;
    private float timeSinceLastHit;
    private Collider ignoreColliderHit;
    private Rigidbody myRigidbody;
    public int hitlessKillCount = 0;
    private int killCount = 0;
    private GameObject PrimaryItem;
    private GameObject SecondaryItem;
    private bool pauseWorldMusic;
    private bool ReceivedTriforce;
    [HideInInspector] public bool Dead = false;
    private Ladder ladder;
    private AudioClip worldMusic;
    private float ringMultiplier = 1;
    private float totalDeathRotation = 0;
    private CapsuleCollider capsuleCollider;
    private LayerMask lMask;
    private bool PlayerPositionFound;
    private Vector3 setVelocity;
    private PhysicMaterial SlipperyMaterial;
    [HideInInspector] public Vector3 startPosition;


    [HideInInspector]
    [System.Serializable]
    public class PlayerData
    {
        [HideInInspector]
        public int NumRupees = 0;
        [HideInInspector]
        public int NumBombs = 0;
        [HideInInspector]
        public int MaxBombs = 8;
        [HideInInspector]
        public int NumKeys = 0;
        [HideInInspector]
        public int NumHeartContainers = 3;
        [HideInInspector]
        public int primaryObjectIndex = -1;
        [HideInInspector]
        public int secondaryObjectIndex = -1;
        [HideInInspector]
        public int numDeaths = 0;
        [HideInInspector]
        public bool isRightHanded = true;
        [HideInInspector]
        public bool handednessDetermined = false;
        [HideInInspector]
        public bool hasLetter = false;
        [HideInInspector]
        public bool showedLetter = false;
        [HideInInspector]
        public bool hasArrows = false;
    }

    public SaveVar<bool>[] hasTriforce = new SaveVar<bool>[8];

    void Awake ()
    {
        if (player == null)
        {
            Debug.Log("Setting Global Static Player");
            player = this;
            //playerPlayspaceOffset = Camera.main.transform.position - player.transform.position;
            //playerPlayspaceOffset.y = 0;
            //player.transform.position = transform.position - player.playerPlayspaceOffset;
        }
        else
        {
            // To Overworld from Dungeon
            if (player.returnFromDungeon)
            {
                Debug.Log("Returning from Dungeon");
                player.transform.position = player.dungeonReturnLocation - player.playerPlayspaceOffset;
                player.InDungeon = false;
            }
            // To Dungeon from Overworld
            else if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                Debug.Log("Entered Dungeon");
                player.transform.position = transform.position - player.playerPlayspaceOffset;
                player.InDungeon = true;
            }
            // To Overworld from Opening
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Debug.Log("Entering Overworld from Opening");
                player.transform.position = transform.position - player.playerPlayspaceOffset;
                player.UpdateMenu(null);
                player.Load();
            }
            // To Opening
            else
            {
                Debug.Log("Returning to Opening Scene");
                player.transform.position = transform.position - player.playerPlayspaceOffset;
                player.UpdateMenu(leftHand.MenuList);
                player.Load();
            }
            player.startPosition = transform.position;
            player.returnFromDungeon = false;
            player.worldMusic = GetComponents<AudioSource>()[0].clip;
            player.worldMusicAudioSource.clip = player.worldMusic;
            player.worldMusicAudioSource.loop = true;
            player.worldMusicAudioSource.Play();
            Destroy(gameObject);
        }
    }

    private void UpdateMenu(Menu[] menu)
    {
        if (menu == null)
        {
            Debug.Log("Updating Menu for non-Opening");
            Menu m = GetComponentInChildren<SaveMenu>(true);
            if (m == null) Debug.Log("Did not Find SaveMenu in Children");
            player.leftHand.MenuList = new Menu[] { m };
            player.rightHand.MenuList = new Menu[] { m };
        }
        else
        {
            Debug.Log("Updating Menu for Opening");
            player.leftHand.MenuList = new Menu[menu.Length];
            for (int i = 0; i < menu.Length; i++)
            {
                player.leftHand.MenuList[i] = menu[i];
            }
            player.rightHand.MenuList = player.leftHand.MenuList;
        }
    }

    public AudioSource GetMusicAudioSource()
    {
        return worldMusicAudioSource;
    }

    public void IncreaseBombs()
    {
        pd.MaxBombs += 4;
    }

    private void ManageCollider()
    {
        if (Camera.main == null) return;

        // OpenXR poses are local to the tracking origin. Convert the head to this
        // collider's local space before calculating height/center so a moved,
        // rotated, or scaled playspace cannot mix world and local coordinates.
        Vector3 localHead = transform.InverseTransformPoint(Camera.main.transform.position);
        float localHeight = Mathf.Max(0.1f, localHead.y);
        capsuleCollider.height = localHeight;
        capsuleCollider.center = new Vector3(localHead.x, localHeight * 0.5f, localHead.z);
    }

    public void Initialize()
    {
        Debug.Log("Player Initializing");
        statusWindow = StatusWindow.statusWindow;
        worldMusicAudioSource = GetComponents<AudioSource>()[0];
        worldMusic = worldMusicAudioSource.clip;
        hurtAudioSource = GetComponents<AudioSource>()[1];
        hurtAudioSource.clip = hurtClip;
        lowHealthAudioSource = GetComponents<AudioSource>()[2];
        lowHealthAudioSource.clip = lowHealthClip;
        itemAudioSource = GetComponents<AudioSource>()[3];
        timeSinceLastHit = Time.time;
        ladder = ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Ladder].GetComponent<Ladder>();
        myRigidbody = GetComponent<Rigidbody>();
        lMask = LayerMask.GetMask(new string[] { "Ground" });
        startPosition = transform.position;
        capsuleCollider = GetComponent<CapsuleCollider>();
        SlipperyMaterial = capsuleCollider.sharedMaterial;
        capsuleCollider.material = null;
        ManageCollider();
        Load();
    }

    void RingUpdate()
    {
        Debug.Log("Ring Level " + ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Ring].GetComponent<InteractableObject>().itemLevelIndex);
        if (ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Ring].val)
        {
            int level = ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Ring].GetComponent<InteractableObject>().itemLevelIndex;
            ringMultiplier = level == 0 ? 0.5f : 0.25f;
            leftPalm.material = palms[level + 1];
            rightPalm.material = palms[level + 1];
            leftWrist.material = wrists[level + 1];
            rightWrist.material = wrists[level + 1];
        }
        else
        {
            ringMultiplier = 1;
            leftPalm.material = palms[0];
            rightPalm.material = palms[0];
            leftWrist.material = wrists[0];
            rightWrist.material = wrists[0];
        }
    }


    // Update is called once per frame
    void Update ()
    {
        if (myRigidbody == null) myRigidbody = GetComponent<Rigidbody>();
        if (setVelocity == Vector3.zero)
        {
            capsuleCollider.material = null;
            myRigidbody.velocity = Vector3.zero;
        }
        else
        {
            myRigidbody.velocity = setVelocity;
        }
        setVelocity = Vector3.zero;
        if (Camera.main == null) Debug.Log("camera null");
        if (player == null) Debug.Log("Player Null");
        playerPlayspaceOffset = Camera.main.transform.position - player.transform.position;
        playerPlayspaceOffset.y = 0;

        if (!PlayerPositionFound && Camera.main.transform.position != Vector3.zero)
        {
            PlayerPositionFound = true;
            transform.position -= playerPlayspaceOffset;
        }

        ManageCollider();

        string q = StandardStuff.getQuadrant(Camera.main.transform.position);
        if (q != quadrant)
        {
            bool recentRoom = false;
            for (int i = 0; i < ROOM_HISTORY_LENGTH; i++)
            {
                if (!recentRoom)
                {
                    if (q.Equals(pastRooms[i]))
                    {
                        recentRoom = true;
                        break;
                    }
                }
/*                if (recentRoom)
                {
                    pastRooms[i] = pastRooms[i + 1];
                }
*/
            }
            if (!recentRoom)
            {
                for (int i = 0; i < ROOM_HISTORY_LENGTH - 1; i++)
                {
                    pastRooms[i] = pastRooms[i + 1];
                }
                pastRooms[ROOM_HISTORY_LENGTH - 1] = q;
            }
            quadrant = q;
            roomTransitionCount++;
        }
        // If dead and spinning
        if (Dead && (Time.time - timeSinceLastHit) < 3)
        {
            transform.RotateAround(Camera.main.transform.position, Vector3.up, 10);
            totalDeathRotation += 10;
        }
        // If dead and done spinning but no menu opened yet.
        else if (totalDeathRotation > 0 && !(rightHand.MenuList[0].gameObject.activeSelf))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0) rightHand.MenuList[0].gameObject.SetActive(true);
            transform.RotateAround(Camera.main.transform.position, Vector3.up, -totalDeathRotation);
            totalDeathRotation = 0;
        }
        // Else if we've died and menu just closed
        else if (Dead && !rightHand.MenuList[0].gameObject.activeSelf)
        {
            LifeRemaining = 3;
            statusWindow.UpdateLife();
            transform.position = startPosition;
            worldMusicAudioSource.clip = worldMusic;
            worldMusicAudioSource.Play();
            Dead = false;
            isUnderground = false;
            Mobilize();
        }
        else if (Camera.main.transform.position.y < -20)
        {
            Die();
        }
        //if (LadderDown)
        //{
        //    Vector3 pos = transform.Find("Camera (eye)").position;
        //    if (Mathf.Abs(pos.x - Ladder.transform.position.x) > 0.75 || Mathf.Abs(pos.z - Ladder.transform.position.z) > 0.75)
        //    {
        //        LadderDown = false;
        //        Ladder.SetActive(false);
        //    }

        //}
        if (lowHealthAudioSource.isPlaying && (LifeRemaining / pd.NumHeartContainers) > 0.34f) lowHealthAudioSource.Stop();
        if (ReceivedTriforce && !itemAudioSource.isPlaying)
        {
            ReceivedTriforce = false;
            player.returnFromDungeon = true;
            SceneManager.LoadScene("Zelda Overworld");
            Mobilize();
            TriforceHolder.triforceHolder.gameObject.SetActive(true);
            ExplorationMap.explorationMap.gameObject.SetActive(false);
        }
        else if (pauseWorldMusic && !itemAudioSource.isPlaying)
        {
            worldMusicAudioSource.Play();
            pauseWorldMusic = false;
        }
    }

    public void RemoveShield()
    {
        HasBigShield.val = false;
        Shield shield = ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Shield].GetComponent<Shield>();
        shield.bigShield = false;
        shield.gameObject.layer = LayerMask.NameToLayer("Shield");
        if (!leftHand.isMain()) leftHand.setShield();
        else if (!rightHand.isMain()) leftHand.setShield();
    }

    public void Load()
    {
        Debug.Log("Loading Player Data from Save");
        LifeRemaining = 3;
        if (lowHealthAudioSource.isPlaying) lowHealthAudioSource.Stop();
        PrimaryItem = null;
        SecondaryItem = null;
        pd = SaveData.saveData.data.pd;

        HasBigShield.val = false;
        SaveData.saveData.data.registerBool("Player_HasBigShield", HasBigShield);

        ObjectList.objectList.prefabs[(int)ObjectList.prefabObjects.Shield].GetComponent<Shield>().bigShield = HasBigShield.val;

        for (int i = 0; i < hasTriforce.Length; i++)
        {
            hasTriforce[i] = new SaveVar<bool>(false);
        }
        SaveData.saveData.data.registerBoolArray("HasTriforce", hasTriforce);
        if (startWithStuff)
        {
            pd.NumBombs = startWithBombs;
            pd.NumRupees = startWithRupees;
            pd.NumHeartContainers = startWithHearts;
            pd.hasArrows = startWithArrows;
            LifeRemaining = startWithHearts;
            pd.NumKeys = startWithKeys;
            for (int i = 0; i < (int)ObjectList.prefabObjects.NumObjects; i++)
            {
                ObjectList.objectList.receivedObjects[i].val = hasItems[i];
                ObjectList.objectList.itemLevels[i].val = levels[i];
                InteractableObject io = ObjectList.objectList.prefabs[i].GetComponent<InteractableObject>();
                if (io != null) io.UpdateLevel(levels[i]);
            }
            for (int i = 0; i < 8; i++)
            {
                hasTriforce[i].val = startingTriforcePieces[i];
            }
        }
        if (startWithSword || ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Sword].val)
        {
            pd.handednessDetermined = true;
            pd.primaryObjectIndex = (int)ObjectList.prefabObjects.Sword;
            ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Sword].val = true;
            PrimaryItem = ObjectList.objectList.prefabs[pd.primaryObjectIndex];
        }
        RingUpdate();

        if (pd.secondaryObjectIndex != -1) SecondaryItem = ObjectList.objectList.prefabs[pd.secondaryObjectIndex];
        StatusWindow.statusWindow.UpdatePrimaryItem(pd.primaryObjectIndex);
        StatusWindow.statusWindow.UpdateSecondaryItem(pd.secondaryObjectIndex);

        StatusWindow.statusWindow.Refresh();
        ItemFrame.itemFrame.UpdateImages();
        TriforceHolder.triforceHolder.UpdateTriforce();

        if (pd.primaryObjectIndex == -1) rightHand.Empty();
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (pd.isRightHanded) rightHand.setMain();
            else leftHand.setMain();
        }

    }

    public void ObtainPrefab(ObjectList.prefabObjects prefab, int itemLevel)
    {
        ObjectList.objectList.receivedObjects[(int)prefab].val = true;
        InteractableObject io = ObjectList.objectList.prefabs[(int)prefab].GetComponent<InteractableObject>();
        itemAudioSource.clip = newItemClip;
        itemAudioSource.Play();
        if (worldMusicAudioSource.isPlaying)
        {
            pauseWorldMusic = true;
            worldMusicAudioSource.Pause();
        }
        switch(prefab)
        {
            case ObjectList.prefabObjects.Key:
                pd.NumKeys = Mathf.Min(255, pd.NumKeys + 1);
                statusWindow.UpdateKeys();
                break;
            case ObjectList.prefabObjects.Bomb:
                pd.NumBombs = Mathf.Min(pd.MaxBombs, pd.NumBombs + 4);
                statusWindow.UpdateBombs();
                break;
            case ObjectList.prefabObjects.Shield:
                if (!HasBigShield.val)
                {
                    HasBigShield.val = true;
                    Shield shield = ObjectList.objectList.prefabs[(int)prefab].GetComponent<Shield>();
                    shield.bigShield = true;
                    shield.gameObject.layer = LayerMask.NameToLayer("Big Shield");
                    if (!leftHand.isMain()) leftHand.setShield();
                    else if (!rightHand.isMain()) leftHand.setShield();

                }
                break;
            case ObjectList.prefabObjects.LionKey:
                statusWindow.UpdateKeys();
                break;
            default:

                if (prefab == ObjectList.prefabObjects.LetterOrPotion && itemLevel == 0) pd.hasLetter = true;
                else if (io != null)
                {
                    if (itemLevel > io.itemLevelIndex)
                    {
                        ObjectList.objectList.itemLevels[(int)prefab].val = itemLevel;
                        io.UpdateLevel(itemLevel);
                    }
                    if (prefab == ObjectList.prefabObjects.Ring)
                    {
                        RingUpdate();
                    }
                }
                break;
        }


        if (PrimaryItem == null && prefab == ObjectList.prefabObjects.Sword)
        {
            PrimaryItem = ObjectList.objectList.prefabs[(int)prefab];
            pd.primaryObjectIndex = (int)prefab;
            StatusWindow.statusWindow.UpdatePrimaryItem(pd.primaryObjectIndex);
        }
        else if (pd.secondaryObjectIndex == -1 && io != null && io.isHoldable)
        {
            SecondaryItem = ObjectList.objectList.prefabs[(int)prefab];
            pd.secondaryObjectIndex = (int)prefab;
            StatusWindow.statusWindow.UpdateSecondaryItem(pd.secondaryObjectIndex);
        }
        pd.handednessDetermined = leftHand.isMain() || rightHand.isMain();
        ItemFrame.itemFrame.UpdateImages();
    }

    public GameObject GetSecondaryItem()
    {
        return SecondaryItem;
    }

    public GameObject GetPrimaryItem()
    {
        return PrimaryItem;
    }

    public GameObject ChangeWeapon(bool up, ref int startIndex, ref int otherDeprecatedIndex, ref GameObject goA, ref GameObject goB, bool isPrimary)
    {
        const int NUM_SELECTABLE_ITEMS = 8;
        int increment = up ? 1 : -1;
        int idx = startIndex;
        int tries = 0;
        InteractableObject io;
        while (true)
        {
            idx += increment;
            tries += 1;
            if (tries > ObjectList.objectList.receivedObjects.Length)
            {
                Debug.Log("Something strange happened in Weapon Switching. Kicked out after " + tries + " tries.");
                return goA;
            }
            if (idx == startIndex) return goA; // if we've looped back around to starting item, return starting item
            if (idx == NUM_SELECTABLE_ITEMS) idx = startIndex == -1 ? -1 : 0; // if we've reached the end incrementing, start back from the beginning
            else if (idx < 0) idx = NUM_SELECTABLE_ITEMS - 1; // if we've gone negative in decrementing, go back to end
            if (idx == -1) return null;
            GameObject go = ObjectList.objectList.prefabs[idx];
            if (go == null) continue;
            io = go.GetComponent<InteractableObject>();
            if (ObjectList.objectList.receivedObjects[idx].val == true && io != null && io.isHoldable
                && !(idx == (int)ObjectList.prefabObjects.Bomb && pd.NumBombs == 0)
                && !(idx == (int)ObjectList.prefabObjects.BowAndArrow && !pd.hasArrows)
                && !(idx == (int)ObjectList.prefabObjects.LetterOrPotion && io.itemLevelIndex == 0 && !player.pd.hasLetter))
            {
                startIndex = idx;
                goA = ObjectList.objectList.prefabs[idx];
                if (isPrimary) StatusWindow.statusWindow.UpdatePrimaryItem(idx);
                else StatusWindow.statusWindow.UpdateSecondaryItem(idx);
                //io.UpdateLevel(ObjectList.objectList.itemLevels[(int)idx].val);
                ItemFrame.itemFrame.SelectItem(startIndex);
                return goA;
            }
        }
    }

    public GameObject ChangePrimary(bool up)
    {
        return ChangeWeapon(up, ref pd.primaryObjectIndex, ref pd.secondaryObjectIndex, ref PrimaryItem, ref SecondaryItem, true);
    }

    public GameObject ChangeSecondary(bool up)
    {
        if (pd.secondaryObjectIndex == (int)ObjectList.prefabObjects.Bomb && pd.NumBombs == 0)
        {
            pd.secondaryObjectIndex = -1;
            SecondaryItem = null;
        }
        return ChangeWeapon(up, ref pd.secondaryObjectIndex, ref pd.primaryObjectIndex, ref SecondaryItem, ref PrimaryItem, false);
    }

    public void HandleCollisions(Collider col)
    {
        if (!isMobile) return;
        float damage = 0;
        if (col.tag == "Enemy") damage = col.GetComponent<Enemy>().DamageDealt();
        else if (col.tag == "Enemy Projectile") damage = col.GetComponent<Projectile>().damagePerHit;
        else if (col.tag == "Fire" || col.tag == "Boulder") damage = 0.5f;
        if (damage > 0)
        {
            damage = damage * ringMultiplier;
            if (!Dead && !((Time.time - timeSinceLastHit) < 1))
            {
                if (ignoreColliderHit != col)
                {
                    hitlessKillCount = 0;
                    NoHitRollover = false;
                    timeSinceLastHit = Time.time;
                    hurtAudioSource?.Play();
                    LifeRemaining -= damage;
                    if (statusWindow != null) statusWindow.UpdateLife();

                    if (LifeRemaining <= 0)
                    {
                        Die();
                    }
                    else if (!lowHealthAudioSource.isPlaying && LifeRemaining <= 1) // / NumHeartContainers) <= 0.34f)
                    {
                        lowHealthAudioSource.Play();
                    }
                }
            }
        }
        else if (col.tag == "Collectible" || col.tag == "Heart Container")
        {
            Collectible c = col.GetComponent<Collectible>();
            int amount = c.getAmount();
            if (amount == 0) return;
            if (c.getCollectSound() != null)
            {
                itemAudioSource.clip = c.getCollectSound();
                itemAudioSource.Play();
            }
            switch (c.item)
            {
                case Collectible.ItemType.HeartContainer:
                    pd.NumHeartContainers += amount;
                    LifeRemaining += amount;
                    statusWindow.UpdateLife();
                    break;
                case Collectible.ItemType.Heart:
                case Collectible.ItemType.Fairy:
                    LifeRemaining = Mathf.Min(pd.NumHeartContainers, LifeRemaining + amount);
                    if (statusWindow != null) statusWindow.UpdateLife();
                    if (lowHealthAudioSource.isPlaying && (LifeRemaining / pd.NumHeartContainers) > 0.34f)
                    {
                        lowHealthAudioSource.Stop();
                    }
                    break;
                case Collectible.ItemType.Rupees:
                    AddRupees(amount);
                    break;
                case Collectible.ItemType.Bombs:
                    pd.NumBombs = Mathf.Min(pd.MaxBombs, pd.NumBombs + amount);
                    statusWindow.UpdateBombs();
                    break;
                case Collectible.ItemType.Key:
                    pd.NumKeys = Mathf.Min(255, pd.NumKeys + amount);
                    statusWindow.UpdateKeys();
                    break;
                case Collectible.ItemType.Clock:
                    Enemy[] enemies = col.transform.parent.GetComponentsInChildren<Enemy>();
                    foreach (Enemy e in enemies)
                    {
                        e.ClockStun = true;
                    }
                    break;
                case Collectible.ItemType.Triforce:
                    pauseWorldMusic = true;
                    ReceivedTriforce = true;
                    hasTriforce[amount - 1].val = true;
                    col.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
                    col.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.075f, 0);
                    TriforceHolder.triforceHolder.UpdateTriforce();
                    worldMusicAudioSource.Stop();
                    Immobilize();
                    StatusWindow.statusWindow.RedDot.GetComponent<TextureSwapper>().stop = true;
                    StatusWindow.statusWindow.fillLife();
                    break;
                case Collectible.ItemType.Compass:
                    StatusWindow.statusWindow.map.obtainCompass();
                    break;
                case Collectible.ItemType.Map:
                    StatusWindow.statusWindow.map.obtainMap();
                    break;
                case Collectible.ItemType.Bracelet:
                    ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Bracelet].val = true;
                    break;
            }
            if (!ReceivedTriforce && !c.DontDestroy) Destroy(c.gameObject);
        }
        else if ((col.tag == "Water") && ObjectList.objectList.receivedObjects[(int)ObjectList.prefabObjects.Ladder].val && !ladder.LadderDown)
        {
            ladder.PlaceLadder(col);
        }
    }

    public void Immobilize()
    {
        isMobile = false;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.useGravity = false;
    }

    private void Die()
    {
        for (int i = 0; i < ROOM_HISTORY_LENGTH; i++)
        {
            pastRooms[i] = "";
        }
        worldMusicAudioSource.Stop();
        lowHealthAudioSource.Stop();
        worldMusicAudioSource.clip = deathMusic;
        worldMusicAudioSource.Play();
        Immobilize();
        Dead = true;
        pd.numDeaths++;
    }

    public void Mobilize()
    {
        isMobile = true;
        myRigidbody.useGravity = true;
    }
    
    public void AddRupees(int num)
    {
        pd.NumRupees = Mathf.Max(Mathf.Min(255, pd.NumRupees + num), 0);
        statusWindow.UpdateRupees(num);
    }

    public void ignoreHit(Collider col)
    {
        ignoreColliderHit = col;
    }

    public void OnTriggerEnter(Collider col)
    {
        HandleCollisions(col);
    }

    void OnTriggerStay(Collider col)
    {
        HandleCollisions(col);
    }

    void OnCollisionStay(Collision col)
    {
        HandleCollisions(col.collider);
    }

    private void OnCollisionEnter(Collision col)
    {
        HandleCollisions(col.collider);
    }

    public int KillConfirm(string tag)
    {
        if (tag != "NA") hitlessKillCount = Mathf.Min(hitlessKillCount + 1, 10);
        return hitlessKillCount;
    }

    public void ResetKillCount()
    {
        if (hitlessKillCount == 10) NoHitRollover = true;
        hitlessKillCount = 0;
    }

    public void StartMusic()
    {
        worldMusicAudioSource.Play();
    }

    public void StopMusic()
    {
        worldMusicAudioSource.Stop();
    }

    public void Move(Vector3 direction, float percent)
    {
        if (isMobile)
        {
            Vector3 vel = direction * percent * PLAYER_SPEED;
            if (!allowFlight)
            {
                RaycastHit hit;
                Vector3 noHeightVelocity = vel;
                noHeightVelocity.y = 0;
                Vector3 startPoint = Camera.main.transform.position;
                startPoint.y = transform.position.y + 0.1f;
                // If ground in front, allow vertical motion
                if (Physics.SphereCast(startPoint, 0.05f, noHeightVelocity, out hit, 0.5f, lMask))
                {
                    // Only move up with equal proportion to a forward vector
                    vel.y = 1;// Mathf.Min(vel.y, Mathf.Max(vel.x, vel.z));
                }
                else
                {
                    vel.y = -1;// myRigidbody.velocity.y;
                }
            }
            setVelocity = vel;
            capsuleCollider.material = SlipperyMaterial;
        }
    }
}
