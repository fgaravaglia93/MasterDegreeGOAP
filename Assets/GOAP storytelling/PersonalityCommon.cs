using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityCommon : MonoBehaviour
{
    public Personality m_personality;

    [HideInInspector]
    void awake()
    {
        m_personality.Init();

    }
}

