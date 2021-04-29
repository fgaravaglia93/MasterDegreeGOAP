using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        GameObject hero = GameObject.FindGameObjectWithTag("Hero");
        if (hero == null)
            hero = GameObject.Instantiate((GameObject)Resources.Load("Prefab/Hero"));

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject npc in npcs)
        {
            if (npc.GetComponent<MovementNPC>() != null)
                npc.GetComponent<MovementNPC>().hero = hero.transform;
        }
    }
}