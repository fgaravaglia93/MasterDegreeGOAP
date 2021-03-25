using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityCommon : MonoBehaviour
{
    [Space(10)]
    [Header("BIG 5 personality model")]

    [Tooltip("Openness to Experience.\nAgent preference on new actions")]
    [Range(-1, 1)]
    public int openness;
    [Range(-1, 1)]
    public int consciousness;
    [Range(-1, 1)]
    public int extraversion;
    [Range(-1, 1)]
    public int agreeableness;
    [Range(-1, 1)]
    public int neuroticism;


    public MoodType mood = MoodType.Neutral;
}
