using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime;

[System.Serializable]
 public struct StoryComponent
{
    public DialogueContainer dialogue;
    public Trait trait;
    public List<GameObject> npcsAffected;
}
public class DramaManager : MonoBehaviour
{
    [SerializeField]
    private List<StoryComponent> storyComponents;
    public static DramaManager instance = null;
    public float eventRepetition;

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

        while (storyComponents.Count>0 && exit)
        {
            var occurred = Random.Range(0f, 1f);
            int storyEvent;
            
            if (occurred >0.6 && !GetComponent<DialogueParser>().dialogueOnGoing)
            {
                storyEvent = Random.Range(0, storyComponents.Count);
                GetComponent<DialogueParser>().interactable = true;
                GetComponent<DialogueParser>().storyEvent = true;
                StoryComponent storyC = storyComponents.ToArray()[0];
                
                //add trait to related NPCs
                foreach(GameObject npc in storyC.npcsAffected)
                {
                    if (npc != null)
                    {
                        TraitData traitData = new TraitData();
                        traitData.name = storyC.trait.name;
                        npc.GetComponentInChildren<ParticleSystem>().time = 0;
                        npc.GetComponentInChildren<ParticleSystem>().Play();
                        npc.GetComponent<PersonalityAgent>().m_personality.AddTrait(traitData);
                    }
                }
                exit = true;
            }

            yield return new WaitForSeconds(eventRepetition);

        }

    }
}
