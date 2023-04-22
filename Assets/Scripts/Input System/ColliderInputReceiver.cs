using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInputReceiver : InputReceiver
{
    private Vector3 clickPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("CLICK");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("HIT");
                clickPosition = hit.point;
                
                OnInputReceived();
            }
        }
    }


    public override void OnInputReceived()
    {
        //Debug.Log("OnInputReceived CALLED");
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(clickPosition, null, null);
        }
    }
}
