using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public NetworkMan networkMan;
    private GameObject player;
    private Vector3 newPosition;
    private Vector3 newRotationEuler;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("RESCALING");
            player.transform.localScale = new Vector3(player.transform.localScale.x - 0.1f, player.transform.localScale.y - 0.1f, player.transform.localScale.z - 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newPosition += new Vector3(0,1,0);
            networkMan.SendPlayerPosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newPosition += new Vector3(0, -1, 0);
            networkMan.SendPlayerPosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newPosition += new Vector3(-1, 0, 0);
            networkMan.SendPlayerPosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newPosition += new Vector3(1, 0, 0);
            networkMan.SendPlayerPosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            newRotationEuler += new Vector3(-30, 0, 0);
            networkMan.SendPlayerRotation(newRotationEuler);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            newRotationEuler += new Vector3(30, 0, 0);
            networkMan.SendPlayerRotation(newRotationEuler);
        }
    }

    public void SetPlayer(GameObject go)
    {
        Debug.Log("PLAYER SET");
        player = go;
        newPosition = player.transform.position;
        newRotationEuler = player.transform.rotation.eulerAngles;
    }
}
