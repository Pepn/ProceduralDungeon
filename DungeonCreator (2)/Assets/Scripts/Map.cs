using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Map))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map myScript = (Map)target;
        if (GUILayout.Button("Draw "))
        {
            myScript.DeleteMap();
            myScript.Draw();
        }
    }
}

public class Map : MonoBehaviour {

    public int[,] map;
    public int[,] rooms;
    public List<GameObject> roomList;
    public List<GameObject> pathwayList;
    public int width;
    public int height;
    [HideInInspector]
    public int currentID;

    public GameObject RoomPrefab;
    public GameObject pathwayPrefab;
    public GameObject floor;
    public Material empty;
    public GameObject ImageProcesser;
    public GameObject outputObject;

    [Tooltip("image width+height divided by this number creates the max heuristic for rooms")]
    public int maxHeuristicDivider;
    public uint distanceTillNextRoom;
    public uint multiplyRoomSize;

    [Tooltip("rooms is the data after creatingrooms on the data set")]
    public bool drawMap;

    [Tooltip("draws the image after image processing")]
    public bool drawRooms;
    public bool calcRooms;
    public bool listRooms;

    // Use this for initialization
    void Start ()
    {
        currentID = 0;
        Init();
        //TestRoom();
        if(calcRooms) CreateRoomsFromPixels();

        Draw();

        ////////////////// MAP[x,y] == 1 IS WHITE 0 IS BLACK
        //////////////////
    }

    void Init()
    {
        ImageProcesser.GetComponent<LineDetection>().Init();
        width = ImageProcesser.GetComponent<LineDetection>().map.width;
        height = ImageProcesser.GetComponent<LineDetection>().map.height;
        map = new int[width, height];
        rooms = new int[width, height];
        //Debug.Log(width + " " + height);
        //Debug.Log(ImageProcesser.GetComponent<LineDetection>().pixels[1, 1].r);
        SetMapPixels();
    }

    public void SetMapPixels()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //r should always be treshholded
                map[x, y] = (int)ImageProcesser.GetComponent<LineDetection>().pixels[x, y].r;
                rooms[x, y] = 0;
            }
        }

        Debug.Log("0,0 is: " + map[0, 0]);
    }

    public void CreateRoomsFromPixels()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    //floor
                    int s = HeuristicConnectedPixelsXY(x,y);
                    Debug.Log("HEURITSIC: " + s);
                    CreateRoom(x, y, (int)(s*multiplyRoomSize), (int)(s*multiplyRoomSize), (int)(s + distanceTillNextRoom));
                    //return;
                }
            }
        }

        CreatePathWays();
    }

    public void CreatePathWays()
    {
        for(int i = 0; i < roomList.Count-1; ++i)
        {
            if(!roomList[i].GetComponent<Room>().isConnected)
            {
                roomList[i].GetComponent<Room>().isConnected = true;
                GameObject p = Instantiate(pathwayPrefab, this.transform);
                p.GetComponent<PathWay>().begin = roomList[i].GetComponent<Room>().GetCenter();
                p.GetComponent<PathWay>().end = roomList[i + 1].GetComponent<Room>().GetCenter();//closest neighbourr.transform.position;
                p.GetComponent<PathWay>().roomIDfrom = roomList[i].GetComponent<Room>().id;
                p.GetComponent<PathWay>().roomIDto = roomList[i+1].GetComponent<Room>().id;
                p.GetComponent<PathWay>().Init();
                pathwayList.Add(p);
            }
        }
    }

    public void  CreateRoom(int posX, int posY, int sizewidth, int sizeheight, int padding)
    {
        GameObject r = Instantiate(RoomPrefab, GameObject.Find("Map").transform);
        r.GetComponent<Room>().Init(currentID, new Vector3(posX, 0, posY), new Vector2(sizewidth, sizeheight));
        roomList.Add(r);
        currentID++;

        Debug.Log("Creating room X,Y: " + posX + ", " + posY + " size: " + sizewidth);
        for (int x = posX; x < posX + sizewidth; x++)
        {
            for (int y = posY; y < posY + sizeheight; y++)
            {
                if(x < width && y < height)
                {
                    rooms[x, y] = 1;
                   // Debug.Log("Setting room[" + x + "," + y + "] = 1");
                }
            }
        }
        for (int x = posX - padding; x < posX + sizewidth + padding; x++)
        {
            for (int y = posY - padding; y < posY + padding + sizeheight; y++)
            {
                if (x < width && y < height && x > 0 && y > 0)
                {
                    map[x, y] = 1;

                }
            }
        }
    }

    public int HeuristicConnectedPixelsXY(int posX, int posY)
    {
        int sum = 0;
        int width = 0;
        int height = 0;
        for (int x = posX; map[x, posY] == 0; ++x)
        {
            width++;
            if (x + 1 == this.width) break;
        }
        for (int x = posX; map[x, posY] == 0; --x)
        {
            width++;
            if (x - 1 <= 0) break;
        }
        for (int y = posY; map[posX, y] == 0; ++y)
        {
            height++;
            if (y + 1 == this.height) break;
        }
        for (int y = posY; map[posX, y] == 0; --y)
        {
            height--;
            if (y - 1 <= 0) break;
        }
        sum = width + height;
        Debug.Log("X,Y: " + posX + ", " + posY + "sum " + sum + " multi: " + (width * height));

        sum = sum % ((this.width + this.height) / maxHeuristicDivider);

        return sum;
    }

    public void Draw()
    {
        if (drawRooms) Draw(rooms);
        if (drawMap) Draw(map);
        if (listRooms) CreateRoomsFromPixels();
    }
    //public int TotalAmountConnectedPixels(int posX, int posY)
    //{
    //    for (int x = posX; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            if (map[x, y] == 1)
    //            {
    //                //floor
    //                Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity, GetComponent<Transform>());
    //            }
    //        }
    //    }
    //    return 0;//sum ;
    //}

    public void ConnectRooms()
    {
        
    }

	
	// Update is called once per frame
	void Update () {
		if(ImageProcesser.GetComponent<LineDetection>().redrawmap)
        {
            Debug.Log("RedrawMap");
          
            DeleteMap();
            SetMapPixels();
            if(calcRooms) CreateRoomsFromPixels();
            Draw();
            ImageProcesser.GetComponent<LineDetection>().redrawmap = false;
        }
	}

    void SavePicture(int[,] pic, string msg)
    {
        Texture2D output = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                output.SetPixel(x, y, new Color((float)pic[x,y], (float)pic[x,y], (float)pic[x,y]));
            }
        }
        File.WriteAllBytes("temppic/" + msg +".png", output.EncodeToPNG());
        //outputObject.GetComponent<Renderer>().material.tex
    }

    public void DeleteMap()
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Debug.Log("deleteobject");
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }

        GameObject m = GameObject.Find("Map");
        int childs2 = m.transform.childCount;
        for (int i = childs2 - 1; i >= 0; i--)
        {
            Debug.Log("deleteobject");
            GameObject.DestroyImmediate(m.transform.GetChild(i).gameObject);
        }
        roomList.Clear();
        pathwayList.Clear();
    }

    void Draw(int[,] map)
    {
        Debug.Log("Draw");
        SavePicture(map, "blacknwhite");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x,y] == 0)
                {
                    //empty
                    Debug.Log("intitititi"); // BLACK
                    GameObject t = Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity, GetComponent<Transform>());
                    t.GetComponent<Renderer>().material = empty; // black
                }
                else if(map[x,y] == 1)
                {
                    //floor
                    Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity, GetComponent<Transform>());
                }
                else if(map[x,y] == 5)
                {
                    Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity, GetComponent<Transform>());
                }
            }
        }
    }

    internal class Label
    {
        #region Public Properties

        public int Name { get; set; }

        public Label Root { get; set; }

        public int Rank { get; set; }

        #endregion

        #region Constructor

        public Label(int Name)
        {
            this.Name = Name;
            this.Root = this;
            this.Rank = 0;
        }

        #endregion

        #region Public Methods

        internal Label GetRoot()
        {
            if (this.Root != this)
            {
                this.Root = this.Root.GetRoot();//Compact tree
            }

            return this.Root;
        }

        internal void Join(Label root2)
        {
            if (root2.Rank < this.Rank)//is the rank of Root2 less than that of Root1 ?
            {
                root2.Root = this;//yes! then Root1 is the parent of Root2 (since it has the higher rank)
            }
            else //rank of Root2 is greater than or equal to that of Root1
            {
                this.Root = root2;//make Root2 the parent

                if (this.Rank == root2.Rank)//both ranks are equal ?
                {
                    root2.Rank++;//increment Root2, we need to reach a single root for the whole tree
                }
            }
        }

        #endregion
    }
}

