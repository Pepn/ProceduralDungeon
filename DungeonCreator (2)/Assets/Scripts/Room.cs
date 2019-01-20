using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public int id;
    public Vector2 size;
    public Vector3 position;
    public List<Vector3> pathways;
    public bool isConnected;
    public Object floor;
    public Object wall;
    public Object pillar;

	// Use this for initialization
	void Start () {
        //CreateRoom(new Vector2(10, 20), new Vector3(0,0,0));
        isConnected = false;
	}

    public Room()
    {
    }
    public Vector3 GetCenter()
    {
        Vector3 c = new Vector3(0,0,0);
        c.x = position.x + (size.x/2);
        c.z = position.z + (size.y/2);
        return c;
    }

    public void Init(int id, Vector3 position, Vector2 size)
    {
        this.id = id;
        this.size = size;
        this.position = position;
        CreateRoom(size, position);
    }

	// Update is called once per frame
	void Update () {
		
	}

    void CreateRoom(Vector2 s, Vector3 pos)
    {
        Debug.Log("Creating Room");
        pos = position;
        s = size;

        floor = Resources.Load("Prefabs/Floor", typeof(GameObject)); //load prefab from resourcers assets folder
        pillar = Resources.Load("Prefabs/Pillar", typeof(GameObject)); wall = Resources.Load("Prefabs/Wall", typeof(GameObject));

        Instantiate(pillar, pos + new Vector3(s.x -.5f, 0, 0), Quaternion.identity, this.gameObject.transform);
        Instantiate(pillar, pos + new Vector3(-0.5f, 0, s.y), Quaternion.identity, this.gameObject.transform);
        Instantiate(pillar, pos + new Vector3(s.x - 0.5f, 0, s.y), Quaternion.identity, this.gameObject.transform);
        Instantiate(pillar, pos + new Vector3(-0.5f,0,0), Quaternion.identity, this.gameObject.transform);

        for (int x = 0; x < s.x; x++)
        {
            Instantiate(wall, pos + new Vector3(x, 0, 0), Quaternion.identity, this.gameObject.transform);
            Instantiate(wall, pos + new Vector3(x, 0, s.y), Quaternion.identity, this.gameObject.transform);
            for (int y = 0; y < s.y; y++)
            {
                Instantiate(floor, pos + new Vector3(x, 0, y), Quaternion.identity, this.gameObject.transform);
            }
        }

        for (int i = 0; i < s.y; i++)
        {
            Instantiate(wall, pos + new Vector3(0, 0, i), Quaternion.AngleAxis(90.0f, new Vector3(0, 1, 0)), this.gameObject.transform);
            Instantiate(wall, pos + new Vector3(s.x, 0, i), Quaternion.AngleAxis(90.0f, new Vector3(0, 1, 0)), this.gameObject.transform);
        }

    }
}
