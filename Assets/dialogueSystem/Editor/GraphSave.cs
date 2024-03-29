﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using DialogueSystem.DataContainers;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class GraphSave
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<DialogueNode> Nodes => _graphView.nodes.ToList().Cast<DialogueNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private DialogueContainer _dialogueContainer;
        private DialogueGraphView _graphView;
        public string path = "Assets/GOAP storytelling/Example/Traits/";


        public static GraphSave GetInstance(DialogueGraphView graphView)
        {
            return new GraphSave
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(fileName, dialogueContainerObject)) return;
            // SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/dialogueSystem/Resources"))
                AssetDatabase.CreateFolder("Assets/dialogueSystem", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/dialogueSystem/Resources/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueContainerObject, $"Assets/dialogueSystem/Resources/{fileName}.asset");
            }
            else
            {
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                //container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                //container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }
        }

        private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as DialogueNode);
                var inputNode = (connectedSockets[i].input.node as DialogueNode);

                //generate 
                var message = connectedSockets[i].output.portName.Split(new char[] { '_', '_' });
                
                
                //Debug.Log(connectedSockets[i].output.portName);
                //Saving Root Node
                if (message[0] == "Next")
                {
                    dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGUID = outputNode.GUID,
                        PortName = message[0],
                        TargetNodeGUID = inputNode.GUID,

                    });
                }
                //Saving Dialogue Nodes
                else
                {
                    MoodType changeTo = MoodType.Neutral;
                    Trait traitTo = null;
                    //Get The mood of ther port
                    if(message.Length>=2)
                        changeTo = ConvertMoodFromString(message[1]);
                    //Get the trait of the port
                    if(message.Length >= 3)
                        traitTo = ConvertTraitFromString(message[2]);

                    dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGUID = outputNode.GUID,
                        PortName = message[0],
                        TargetNodeGUID = inputNode.GUID,
                        changeMoodTo = changeTo,
                        trait = traitTo
                    });

                }
            }

            foreach (var node in Nodes.Where(node => !node.EntyPoint))
            {
                dialogueContainerObject.DialogueNodeData.Add(new DialogueNodeData
                {
                    NodeGUID = node.GUID,
                    title = node.title,
                    DialogueText = node.DialogueText,
                    //face = node.face,
                    face = null,
                    mood = node.mood,
                    Position = node.GetPosition().position
                });
            }

            return true;
        }


        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>().Select(x => x.GUID)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadDialogues(string fileName)
        {
            _dialogueContainer = Resources.Load<DialogueContainer>(fileName);
            if (_dialogueContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Dialogue Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            //AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.EntyPoint).GUID = _dialogueContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.EntyPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateDialogueNodes()
        {
            foreach (var perNode in _dialogueContainer.DialogueNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode.title, perNode.DialogueText, perNode.face, perNode.mood, Vector2.zero);
                tempNode.GUID = perNode.NodeGUID;
                _graphView.AddElement(tempNode);

                var nodePorts = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName, x.changeMoodTo, x.trait));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _dialogueContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].GUID).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _dialogueContainer.DialogueNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        /*private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _dialogueContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }*/

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _dialogueContainer.CommentBlockData)
            {
                var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                     commentBlockData);
                block.AddElements(Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.GUID)));
            }
        }

        //Get MoodType associated to an answer
        private MoodType ConvertMoodFromString(string moodString)
        {

            switch (moodString)
            {
                case "Joy":
                    return MoodType.Joy;
                case "Sad":
                    return MoodType.Sadness;
                case "Angry":
                    return MoodType.Angry;
                case "Fear":
                    return MoodType.Fear;
                case "Disgust":
                    return MoodType.Disgust;
                default:
                    return MoodType.Neutral;
            }
        }

        //Get Trait associated to an answer
        public Trait ConvertTraitFromString(string traitFile)
        {
            //remove space char at start and at the end
            traitFile = traitFile.TrimStart();
            traitFile = traitFile.TrimEnd();
            //Load the trait associated to the port
            string pathTrait = "Assets/GOAP storytelling/Example/Traits/" + traitFile + ".asset";
            UnityEngine.Object data = AssetDatabase.LoadAssetAtPath(pathTrait, typeof(Trait));
            Trait trait = data as Trait;
            return trait;
        }
    }
}