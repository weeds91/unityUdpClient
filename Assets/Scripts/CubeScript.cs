using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{

    public float speed = 0.2f;
    private string id;
    private Color cubeColor;

    void Update()
    {
        //transform.Rotate(0, speed, 0, Space.World);
    }

    public void SetValues(string _id, float r, float g, float b)
    {
        id = _id;
        cubeColor = new Color(r, g, b);
        GetComponent<MeshRenderer>().material.color = new Color(r, g, b, 1);
    }

    public void SetColor(float r, float g, float b)
    {
        cubeColor = new Color(r, g, b);
        GetComponent<MeshRenderer>().material.color = new Color(r, g, b);
    }

    public void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x,y,z);
    }

    public void SetRotation(float x, float y, float z)
    {
        Quaternion newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(x, y, z);
        transform.rotation = newRotation;
    }

    public string GetId()
    {
        return id;
    }
}
