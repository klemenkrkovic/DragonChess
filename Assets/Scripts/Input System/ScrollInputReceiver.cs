using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScrollInputReceiver : MonoBehaviour
{
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private ChessGameController gameController;


    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y == 1 && boardAnchor.position.y != -11)
            {
                boardAnchor.Translate(Vector3.up * -11);
            }
            else if (Input.mouseScrollDelta.y == -1 && boardAnchor.position.y != 11)
            {
                boardAnchor.Translate(Vector3.up * 11);
            }

            //Debug.Log(Input.mouseScrollDelta.y);
            gameController.ChangeActiveBoard(Input.mouseScrollDelta.y);
        }
    }
}
