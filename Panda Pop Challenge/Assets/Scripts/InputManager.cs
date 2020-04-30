using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding {
    public class InputManager : MonoBehaviour
    {
        //This class is in charge of the Input activity. (Mouse in this case).

        void Update()
        {
            GetMouseClick();
        }


        #region Mouse Function
        private void GetMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Get a Ray on the mouse position. (Left click)
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    //Send the clicked object to the pathfinder.
                    NodeCreator.instance.SetPath(hit.transform);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                //Get a Ray on the mouse position. (Right click)
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    //Change this node behavior.
                    StartCoroutine(NodeCreator.instance.ChangeNodeBehavior(hit.transform));
                }
            }
        }
    }
    #endregion
}
