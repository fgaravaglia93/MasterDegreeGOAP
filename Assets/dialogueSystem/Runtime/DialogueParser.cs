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
        public bool flagNPC;
        [SerializeField]
        private DialogueContainer dialogue;
        [SerializeField]
        private TextMeshProUGUI dialogueText;
        [SerializeField]
        private Button choicePrefab;
        [SerializeField]
        private Transform buttonContainer;

        void Start()
        {
            //Start dialogue from the root node (NodeLinks.First())
            var dialogueData = dialogue.NodeLinks.First();
            StartDialogue(dialogueData.TargetNodeGUID);
        }

        public void StartDialogue(string dialogueDataGUID)
        {
            var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == dialogueDataGUID).DialogueText;
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == dialogueDataGUID);
            // dialogueText.text = ProcessProperties(text);
            dialogueText.text = text;
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            int nChoices = choices.Count();

            //This handle the trait option

            //serve un sistema più avanzato per indicare la tranform
            int j = 0;
            bool skip = false;
            foreach (NodeLinkData choice in choices)
            {
                Debug.Log(choice.PortName);
                //buttonContainer.Translate(new Vector3(300 * j, 0f, 0f), Space.World);

                //se ho trait instanza bottone e dimmi di non istanziare il successivo
                if (!skip)
                {
                    j++;

                    if (choice.trait == null)
                    {
                        Button button = InstanciateButtonChoice(choice, nChoices, j);
                    }
                    else
                    {
                        //check trait, se ce'ho istanzialo
                        Button button;
                        Debug.Log("not trait");
                        if (flagNPC)
                        {
                            skip = true;
                            button = InstanciateButtonChoice(choice, nChoices, j+1);
                        }

                    }


                }
                
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
                new Vector3(button.GetComponent<RectTransform>().anchoredPosition.x + 600, 0f, 0f);
            else
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(button.GetComponent<RectTransform>().anchoredPosition.x + 300 * j, 0f, 0f);

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
