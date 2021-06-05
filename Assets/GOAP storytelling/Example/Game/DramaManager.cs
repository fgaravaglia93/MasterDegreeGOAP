using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime;

[System.Serializable]
 public struct StoryEvent
{
    public DialogueContainer narrative;
    public Trait trait;
    public MoodType changeMoodTo;
    public GameObject actor;
    public List<GameObject> npcsAffected;

}
public class DramaManager : MonoBehaviour
{
    [SerializeField]
    private List<StoryEvent> storyEvents;
    public static DramaManager instance = null;
    public float eventRepetition;
    [Range(0,1)]
    public float chanceToOccur;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        EventGeneration();
    }

    void EventGeneration()
    {
        StartCoroutine("NextEvent");
    }


    //Random Spawn Of Story events as a dialogue
    IEnumerator NextEvent()
    {
        yield return new WaitForSeconds(eventRepetition);

        bool exit = true;

        while (storyEvents.Count>0 && exit)
        {
            var occurred = Random.Range(0f, 1f);
            int storyEvent;
            
            if (occurred <= chanceToOccur && !DialogueParser.instance.dialogueOnGoing)
            {
                storyEvent = Random.Range(0, storyEvents.Count);
                DialogueParser.instance.interactable = true;
                DialogueParser.instance.storyEvent = true;
                StoryEvent storyE = storyEvents.ToArray()[0];
                
                //add trait to related NPCs
                foreach(GameObject npc in storyE.npcsAffected)
                {
                    if (npc != null)
                    {
                        if(storyE.trait.name != null)
                        {
                            TraitData traitData = new TraitData();
                            traitData.name = storyE.trait.name;
                            npc.GetComponentInChildren<ParticleSystem>().time = 0;
                            npc.GetComponentInChildren<ParticleSystem>().Play();
                            npc.GetComponent<Moody5Agent>().m_personality.AddTrait(traitData);
                        }

                        DisplayManager.instance.ChangeMood(npc, storyE.changeMoodTo,5);
                    }
                }
                exit = true;
            }

            yield return new WaitForSeconds(eventRepetition);

        }

    }
}
