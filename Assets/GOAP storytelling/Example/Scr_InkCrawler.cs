using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Scr_InkCrawler: MonoBehaviour
{
    public delegate void BringUpWindowDelegate();
    public static BringUpWindowDelegate OnBringUpWindow;

    void OnGUI()
    {
       
        BringUpWindow();

    }
    void BringUpWindow()
    {
        if (OnBringUpWindow != null)
        {
            OnBringUpWindow();
        }
    }

}
