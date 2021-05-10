using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime;

public class StoryEventListener : MonoBehaviour
{
    [SerializeField]
    private List<DialogueContainer> narrativeSequence;
    public static StoryEventListener instance = null;
    public float eventRepetition;
    public GameObject npc;
    public string trait;
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
        StartCoroutine("NextEvent");
    }


    IEnumerator NextEvent()
    {
        yield return new WaitForSeconds(eventRepetition);

        bool exit = true;
        while (narrativeSequence.Count>0 && exit)
        {
            var occurred = Random.Range(0f, 1f);
            int storyEvent;
            //Inputgetkey u
            //occurred > 0.6f
           if (occurred >0.6 && !GetComponent<DialogueParser>().dialogueOnGoing)
            {
                storyEvent = Random.Range(0, narrativeSequence.Count);
                GetComponent<DialogueParser>().interactable = true;
                GetComponent<DialogueParser>().storyEvent = true;

                //This will select only on Tina , sandbox
                if (npc != null)
                {
                    TraitData traitData = new TraitData();
                    traitData.name = trait;
                    npc.GetComponent<PersonalityAgent>().m_personality.AddTrait(traitData);
                }
                exit = true;
                  
            }

            yield return new WaitForSeconds(eventRepetition);

        }

    }
}
