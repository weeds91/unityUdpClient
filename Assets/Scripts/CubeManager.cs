using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject cubePrefab;
    private List<GameObject> cubes = new List<GameObject>();
    private Vector3 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.position;
    }

    public void UpdateCubes(NetworkMan.Player[] players)
    {
        foreach (NetworkMan.Player p in players)
        {
            GameObject thisCube = GetCube(p.id);
            if (thisCube == null)
                CreateCube(p);
            else
            {
                UpdateColor(thisCube, p.color.R, p.color.B, p.color.G);
                UpdatePosition(thisCube, p.position.X, p.position.Y, p.position.Z);
                UpdateRotation(thisCube, p.rotation.X, p.rotation.Y, p.rotation.Z);
            }
        }
    }

    void UpdateColor(GameObject cube, float r, float g, float b)
    {
        cube.GetComponent<CubeScript>().SetColor(r, g, b);
    }

    void UpdatePosition(GameObject cube, float x, float y, float z)
    {
        cube.GetComponent<CubeScript>().SetPosition(x, y, z);
    }

    void UpdateRotation(GameObject cube, float x, float y, float z)
    {
        cube.GetComponent<CubeScript>().SetRotation(x, y, z);
    }

    void CreateCube(NetworkMan.Player p)
    {
        Vector3 receivedPosition = new Vector3(p.position.X, p.position.Y, p.position.Z);
        //GameObject newCube = Instantiate(cubePrefab, spawnPosition, transform.rotation);
        GameObject newCube = Instantiate(cubePrefab, receivedPosition, transform.rotation);
        //spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + 1, spawnPosition.z);
        newCube.GetComponent<CubeScript>().SetValues(p.id, p.color.R, p.color.B, p.color.G);
        cubes.Add(newCube);
    }

    public void RemoveCube(NetworkMan.Player p)
    {
        foreach (GameObject c in cubes)
        {
            if (p.id == c.GetComponent<CubeScript>().GetId())
            {
                Destroy(c, 0.2f);
                cubes.Remove(c);
            }
        }
    }

    public GameObject GetCube(string id)
    {
        foreach (GameObject c in cubes)
        {
            if (c.GetComponent<CubeScript>().GetId() == id)
            {
                //Debug.Log("Cube Manager: Found ID");
                return c;
            }
        }
        //Debug.Log("Cube Manager: No Such ID");
        return null;
    }
}
