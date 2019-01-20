using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWay : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 begin;
    public Vector3 end;
    public int roomIDfrom, roomIDto;

    public Object floor;
    void Start()
    {
        
    }
    
    public void Init()
    {
        floor = Resources.Load("Prefabs/Floor", typeof(GameObject)); //load prefab from resourcers assets folder

        if(Mathf.Abs(begin.x - end.x) < Mathf.Abs(begin.z - end.z))
        {

        }

        if (begin.x <= end.x)
        {
            //for (int x = (int)begin.x; x <= (int)end.x; x++)
            {

                //Instantiate(floor, new Vector3(x, 0, begin.z), Quaternion.identity, this.gameObject.transform);
            }
            GameObject g = Instantiate(floor, begin, Quaternion.identity, this.gameObject.transform) as GameObject;
            g.transform.localScale = new Vector3(end.x - begin.x, 1, 1);
            g.transform.position += new Vector3((end.x - begin.x) / 2, 0, 0);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, end.z);
            g.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1, 1);
        }
        else
        {
            //for (int x = (int)end.x; x <= (int)begin.x; x++)
            {

                //Instantiate(floor, new Vector3(x, 0, begin.z), Quaternion.identity, this.gameObject.transform);
            }
            GameObject g = Instantiate(floor, begin, Quaternion.identity, this.gameObject.transform) as GameObject;
            g.transform.localScale = new Vector3(begin.x - end.x, 1, 1);
            g.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1, 1);
            g.transform.position -= new Vector3((begin.x - end.x) / 2, 0, 0);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, end.z);
        }
        if (begin.z <= end.z)
        {
            //for (int y = (int)begin.z; y <= (int)end.z; y++)
            {
                //Instantiate(floor, new Vector3(begin.x, 0, y), Quaternion.identity, this.gameObject.transform);
            }
            GameObject g = Instantiate(floor, begin, Quaternion.identity, this.gameObject.transform) as GameObject;
            g.transform.localScale = new Vector3(1, 1, end.z-begin.z);
            g.transform.position += new Vector3(0,0,(end.z-begin.z)/2);
            g.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1, 1);
            
        }
        else
        {
            //for (int y = (int)end.z; y <= (int)begin.z; y++)
            {

                //Instantiate(floor, new Vector3(begin.x, 0, y), Quaternion.identity, this.gameObject.transform);
            }
            GameObject g = Instantiate(floor, begin, Quaternion.identity, this.gameObject.transform) as GameObject;
            g.transform.localScale = new Vector3(1, 1, begin.z - end.z);
            g.transform.position -= new Vector3(0, 0, (begin.z - end.z) / 2);
            g.GetComponentInChildren<Renderer>().material.mainTextureScale = new Vector2(1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
