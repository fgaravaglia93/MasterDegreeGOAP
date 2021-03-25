using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class AgentWindow : EditorWindow
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        Scr_InkCrawler.OnBringUpWindow = ShowWindow; // note: there is no () on this
       
    }

    public static void ShowWindow()
    {
        //GetWindow<AgentWindow>("Done Crawling!");
       // Debug.Log("hey");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Done Crawling!");
        
    }
}


