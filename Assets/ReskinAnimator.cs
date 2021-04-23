using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReskinAnimator : MonoBehaviour
{
    public string spriteSheetName;
    bool change = false;
    Sprite previous;

    private void Start()
    {
        //spriteSheetName = GetComponent<SpriteRenderer>().name.Split(char[]     }
        previous = null;
    }
    private void LateUpdate()
    {
        if (!change)
        {
            var subSprites = Resources.LoadAll<Sprite>("Sprites/NPCs/" + spriteSheetName);
            var renderer = GetComponent<SpriteRenderer>();
            var spriteName = renderer.sprite.name;
            //Debug.Log(spriteName.Substring(spriteName.Length - 2));
            var newSprite = Array.Find(subSprites, item => item.name.Substring(item.name.Length - 2) == spriteName.Substring(spriteName.Length - 2));
            //if (newSprite != previous)
            renderer.sprite = newSprite;
            previous = newSprite; 
        }
    }
}
