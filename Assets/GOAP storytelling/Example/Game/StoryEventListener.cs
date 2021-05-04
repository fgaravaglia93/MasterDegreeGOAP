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

        while (narrativeSequence.Count>0)
        {
            var occurred = Random.Range(0f, 1f);
            int storyEvent;
            //Inputgetkey u
            //occurred > 0.6f
            if (Input.GetKeyDown(KeyCode.U) && !GetComponent<DialogueParser>().dialogueOnGoing)
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
                  
            }

            yield return new WaitForSeconds(eventRepetition);

        }

    }
}
