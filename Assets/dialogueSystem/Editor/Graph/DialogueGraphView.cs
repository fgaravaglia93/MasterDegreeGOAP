﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using ObjectField = UnityEditor.UIElements.ObjectField;
using EnumField = UnityEditor.UIElements.EnumField;

namespace DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        //public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;
        public DialogueGraphView(DialogueGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            AddElement(GetEntryPointNodeInstance());
            AddSearchWindow(editorWindow);
        }



        private void AddSearchWindow(DialogueGraph editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        /*public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }*/

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            if (commentBlockData == null)
                commentBlockData = new CommentBlockData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        /*public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = "string" };
            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Blackboard.Add(container);
        }*/

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void CreateNewDialogueNode(string nodeTitle, string nodeDialogue, MoodType mood, Vector2 position)
        {
            AddElement(CreateNode(nodeTitle, nodeDialogue, null, mood, position));
        }

        public DialogueNode CreateNode(string nodeTitle, string nodeDialogue, Sprite face, MoodType mood, Vector2 position)
        {
            
            var tempDialogueNode = new DialogueNode()
            {
                title = nodeTitle,
                DialogueText = nodeDialogue,
                face = face,
                mood = mood,
                GUID = Guid.NewGuid().ToString()
            };


            switch (mood)
            {
                case MoodType.Neutral:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.91f);
                    break;
                case MoodType.Joy:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(1f, 1f, 0f, 0.91f);
                    tempDialogueNode.mood = MoodType.Joy;
                    break;
                case MoodType.Angry:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(1f, 0f, 0f, 0.91f);
                    break;
                case MoodType.Sad:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0f, 0f, 1f, 0.91f);
                    break;
                case MoodType.Fear:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0.5f, 0f, 0.5f, 0.91f);
                    break;
                case MoodType.Disgust:
                    tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0, 0.5f, 0, 0.91f);
                    break;
                default:
                    break;
            }

            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
                       
            var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);

            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize));

            var insertImage = new ObjectField();
            insertImage.objectType = (typeof(Sprite));
            
            if (face != null)
                insertImage.value = face;
            
          //  insertImage = tempDialogueNode.face;
            tempDialogueNode.mainContainer.Add(insertImage);
            tempDialogueNode.mainContainer.Add(insertImage.contentContainer);
           
            //tempDialogueNode.mainContainer.Add();

            //Set Image
            //Sprite spriteFace;
            insertImage.RegisterValueChangedCallback(evt =>
            {
                    tempDialogueNode.face = (Sprite)evt.newValue;
                    tempDialogueNode.mainContainer.Add(insertImage.contentContainer);

                   
            });

            //Set title
            var textFieldTitle = new TextField("");
            textFieldTitle.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.title = evt.newValue;
            });
            textFieldTitle.SetValueWithoutNotify(tempDialogueNode.title);

            //Set field for the associated emotion
            EnumField moodField = new EnumField(mood);
            moodField.value = tempDialogueNode.mood;

            moodField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.mood = (MoodType)evt.newValue;
                switch ((MoodType)evt.newValue)
                {
                    case MoodType.Neutral:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.91f);
                        break;
                    case MoodType.Joy:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(1f, 1f, 0f, 0.91f);
                        break;
                    case MoodType.Angry:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(1f, 0, 0, 0.91f);
                        break;
                    case MoodType.Sad:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0f, 0f, 1f, 0.91f);
                        break;
                    case MoodType.Fear:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0.5f, 0, 0.5f, 0.91f);
                        break;
                    case MoodType.Disgust:
                        tempDialogueNode.Q<VisualElement>("title").style.backgroundColor = new Color(0, 0.5f, 0, 0.91f);
                        break;
                    default:
                        break;
                }
            });
            tempDialogueNode.mainContainer.Add(moodField);
            //Set Dialogue Text Field
            var textFieldDialogue = new TextField("");
            textFieldDialogue.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.DialogueText = evt.newValue;

            });
            textFieldDialogue.SetValueWithoutNotify(tempDialogueNode.DialogueText);

            tempDialogueNode.mainContainer.Add(textFieldTitle);
            tempDialogueNode.mainContainer.Add(textFieldDialogue);


            var button = new Button(() => { AddChoicePort(tempDialogueNode,"",MoodType.Neutral); })
            {
                text = "Add an answer"
            };
            tempDialogueNode.titleButtonContainer.Add(button);

            return tempDialogueNode;
        }


        public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "", MoodType savedMoodPort = MoodType.Neutral, Trait traitSaved
            = null)
        {
            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);

            

            var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            var outputPortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;
            //Default string to pass at node creation
            if (overriddenPortName == "")
                generatedPort.name = $"Option {outputPortCount + 1}_" + savedMoodPort;

            var textField = new TextField()
            {
                name = string.Empty,
                value = outputPortName
            };
            //textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));

            EnumField moodAnswer = new EnumField(savedMoodPort);
            moodAnswer.style.width = 80;
            moodAnswer.value = savedMoodPort;
            generatedPort.contentContainer.Add(moodAnswer);

            //activate only if a trait is associated to that NPC
            var insertTrait = new ObjectField();
            insertTrait.objectType = (typeof(Trait));
            insertTrait.style.width = 40;
            string traitName = "";
            if (traitSaved != null)
            {
                insertTrait.value = traitSaved;
                traitName = traitSaved.name;
            }
            
            
            MoodType moodName = savedMoodPort;
           

            textField.RegisterValueChangedCallback(evt => 
            {
                outputPortName = evt.newValue;
                //"_" used to split the port name into choice button text and associated mood
                if(insertTrait.value == null)
                     generatedPort.portName = evt.newValue + "_" + moodName;
                else
                    generatedPort.portName = evt.newValue + "_" + moodName + "_" + traitName;
                Debug.Log("Stringa passata text: " + generatedPort.portName);
            });

            moodAnswer.RegisterValueChangedCallback(evt =>
            {
                moodName = (MoodType)evt.newValue;
                //"_" used to split the port name into choice button text and associated mood
                
                if (insertTrait.value == null)
                    generatedPort.portName = outputPortName + "_" + moodName;
                else
                    generatedPort.portName = outputPortName + "_" + moodName + "_" + traitName;
                Debug.Log("Stringa passata mood: "+ generatedPort.portName);
            });

            insertTrait.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != null)
                {
                    traitName = ((Trait)evt.newValue).name;
                    //"_" used to split the port name into choice button text and associated mood
                    generatedPort.portName = outputPortName + "_" + moodName + "_" + traitName;
                }
                else
                    generatedPort.portName = outputPortName + "_" + moodName;
                Debug.Log("Stringa passata Trait: " + generatedPort.portName);

            });

            generatedPort.contentContainer.Add(insertTrait);
            generatedPort.contentContainer.Add(moodAnswer);
            generatedPort.contentContainer.Add(textField);

            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            
            nodeCache.outputContainer.Add(generatedPort);
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
          
        }

        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntyPoint = true
            };

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }
    }
}