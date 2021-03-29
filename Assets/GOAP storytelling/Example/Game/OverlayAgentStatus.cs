using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayAgentStatus : MonoBehaviour
{
    public bool overlayInUse = false;
    Vector2 pos;
    RaycastHit2D hit;

    private void Start()
    {
       // DisplayController.instance.gameObject.transform.parent.GetChild(0).gameObject.SetActive(false);
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !overlayInUse)
        {
            pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);
            hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit && hit.collider.gameObject.CompareTag("NPC"))
            {
                Debug.Log("CLICK");
                DisplayController.instance.gameObject.transform.parent.GetChild(0).gameObject.SetActive(true);
                overlayInUse = true;
            }
        }

        else if (Input.GetButtonDown("Escape") && overlayInUse)
        {
            overlayInUse = false;
            DisplayController.instance.gameObject.transform.parent.GetChild(0).gameObject.SetActive(false);
        }
        else { }
    }
}
