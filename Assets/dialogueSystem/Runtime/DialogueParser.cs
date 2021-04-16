using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem.DataContainers;

namespace DialogueSystem.Runtime
{
    //Here we the change of mood tresholds during dialogues
    public class DialogueParser : MonoBehaviour
    {
        [SerializeField]
        private DialogueContainer narrativeSequence;
        public TextMeshProUGUI dialogueText;

        public GameObject dialogueFace;
        public Button choicePrefab;
        public Transform buttonContainer;
        
        //[SerializeField]
        private NodeLinkData dialogueData;
        public bool dialogueOnGoing = false;
        private bool lastBlock = false;

        public bool flagNPC;
        public bool interactable;

        void Start()
        {
            //Start dialogue from the root node (NodeLinks.First())
            dialogueData = narrativeSequence.NodeLinks.First();
        }

        void Update()
        {
            if(interactable && !dialogueOnGoing)
            {

                if (Input.GetButtonDown("Enter"))
                {
                    Debug.Log("interact");

                    if (!lastBlock)
                    {
                        dialogueOnGoing = true;
                        dialogueFace.transform.parent.gameObject.SetActive(true);
                        StartDialogue(dialogueData.TargetNodeGUID);
                    }
                    else
                    {
                        dialogueFace.transform.parent.gameObject.SetActive(false);
                        lastBlock = false;
                        dialogueData = narrativeSequence.NodeLinks.First();
                    }
                }
            }
        }

        public void StartDialogue(string dialogueDataGUID)
        {
            var text = narrativeSequence.DialogueNodeData.Find(x => x.NodeGUID == dialogueDataGUID).DialogueText;
            var choices = narrativeSequence.NodeLinks.Where(x => x.BaseNodeGUID == dialogueDataGUID);
            var face = narrativeSequence.DialogueNodeData.Find(x => x.NodeGUID == dialogueDataGUID).face;

            // dialogueText.text = ProcessProperties(text);
            dialogueText.text = text;
            dialogueFace.GetComponent<Image>().sprite = face;
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            int nChoices = choices.Count();

            //If leaf node
            if (nChoices == 0)
            {
                dialogueOnGoing = false;
                lastBlock = true;
            }
                
            //This handle the trait option

            //serve un sistema più avanzato per indicare la tranform
            int j = 0;
            bool skip = false;
           
            foreach (NodeLinkData choice in choices)
            {
                //Debug.Log(choice.PortName);
                //buttonContainer.Translate(new Vector3(300 * j, 0f, 0f), Space.World);

                //se ho trait instanza bottone e dimmi di non istanziare il successivo
                if (!skip)
                {
                    

                    if (choice.trait == null)
                    {
                        j++;
                        Button button = InstanciateButtonChoice(choice, nChoices, j);
                    }
                    else
                    {
                        //check trait, se ce'ho istanzialo
                        Button button;
                        //Debug.Log("not trait");
                        if (flagNPC)
                        {
                            skip = true;
                            j++;
                            button = InstanciateButtonChoice(choice, nChoices, j);
                        }

                    }


                }
                else
                    skip = false;

                //this will manage the change of mood during interaction by talking
                // if (choice.changeMoodTo == MoodType.Joy)
                //button.onClick.AddListener(() => DisplayController.instance.DisplayChange());
                //button.onClick.AddListener(() => DisplayController.instance.ChangeMoodToJoy());
            }
        }

        private Button InstanciateButtonChoice(NodeLinkData choice, int nChoices, int j)
        {
            Button button = Instantiate(choicePrefab, buttonContainer);
            var rectTransform = button.GetComponent<RectTransform>();
            if (nChoices == 1)
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x + 600, buttonContainer.GetComponent<RectTransform>().anchoredPosition.y - 50f, 0f);
            else
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x + 100 * j, buttonContainer.GetComponent<RectTransform>().anchoredPosition.x -50f, 0f);

            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.PortName;
            button.onClick.AddListener(() => StartDialogue(choice.TargetNodeGUID));
            return button;
        }

    /*  private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                  text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
             }
          return text;
         }*/
    }
    
}
