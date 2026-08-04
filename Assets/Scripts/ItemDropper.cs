using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour {

    public enum EnemyGroup
    {
        A, B, C, D, X
    }
    public static ItemDropper itemDropper;

    public GameObject YellowRupee;
    public GameObject BlueRupee;
    public GameObject Bombs;
    public GameObject Heart;
    public GameObject Fairy;
    public GameObject Clock;
    public GameObject[][] dropList;
    //public static GameObject

    private bool obtainedFairy;

    // Use this for initialization
    void Awake ()
    {
        if (itemDropper == null) itemDropper = this;
        else Destroy(gameObject);
        dropList = new GameObject[][] { new GameObject[] { YellowRupee, Heart, YellowRupee, Fairy, YellowRupee, Heart, Heart, YellowRupee, YellowRupee, Heart },
                                        new GameObject[] { Bombs, YellowRupee, Clock, YellowRupee, Heart, Bombs, YellowRupee, Bombs, Heart, Heart },
                                        new GameObject[] {YellowRupee, Heart, YellowRupee, BlueRupee, Heart, Clock, YellowRupee, YellowRupee, YellowRupee, BlueRupee},
                                        new GameObject[] {Heart, Fairy, YellowRupee, Heart, Fairy, Heart, Heart, Heart, YellowRupee, Heart } };
    }

    // Mimicking drop strategy in accordance to https://kb.speeddemosarchive.com/The_Legend_of_Zelda
    // groups of 10 kills without taking damage drops blue rupee (or bombs if killed by bombs), 
    // 16 kills drops fairy but a special case if fairy can't drop due to group X kill
    public void DropItem(float dropPercent, EnemyGroup group, Transform t, string tag)
    {
        int noHitKills = Player.player.KillConfirm(tag);
        if (obtainedFairy && !Player.player.NoHitRollover) obtainedFairy = false;
        GameObject go = null;
        // Group X can't drop anything, even special stuff
        if (group == EnemyGroup.X)
        {
            return;
        }
        if (noHitKills == 10)
        {
            if (tag == "Bomb")
            {
                go = Instantiate(Bombs, t.position, Bombs.transform.rotation);
            }
            else
            {
                go = Instantiate(BlueRupee, t.position, BlueRupee.transform.rotation);
            }
            Player.player.ResetKillCount();
        }
        else if (noHitKills == 6 && Player.player.NoHitRollover && !obtainedFairy)
        {
            Vector3 SpawnPoint = t.position;
            SpawnPoint.y = Camera.main.transform.position.y;
            go = Instantiate(Fairy, SpawnPoint, Fairy.transform.rotation);
            obtainedFairy = true;
            Player.player.ResetKillCount();
        }
        else if (Random.Range(0, 100 / (int)(100 * dropPercent)) == 0)
        {
            GameObject prefab = dropList[(int)group][(noHitKills + (1 * Random.Range(0, 2))) % 10];
            Vector3 SpawnPoint = t.position;
            if (prefab.GetComponent<Collectible>().item == Collectible.ItemType.Fairy) SpawnPoint.y = Camera.main.transform.position.y;
            go = Instantiate(prefab, SpawnPoint, prefab.transform.rotation);
        }
        if (go != null) go.transform.SetParent(t.parent);
    }
}
