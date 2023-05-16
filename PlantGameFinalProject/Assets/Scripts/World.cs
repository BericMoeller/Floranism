using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Threading;
using Unity.VisualScripting;

public class World : MonoBehaviour
{
    public string filePath;
    public float tileSize = 1.2F;
    public GameObject playerObject;
    public GameObject enemyObject1;
    public GameObject CameraObject;
    public GameObject HealthObject;
    private List<GameObject> enemies = new List<GameObject>();
    private int baseX = 0;
    private int basey = 0;
    private int lengthOfWorld;
    private float x;
    private float y;
    private List<GameObject> gamePieces;
    // Start is called before the first frame update
    void Start()
    {
        playerObject.SetActive(false);
        enemyObject1.SetActive(false);
        enemies.Add(enemyObject1);
        BuildWorld();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLayers();
    }

    void BuildWorld()
    {
        if(File.Exists(filePath))
        {
            int count = 0;
            string worldData = File.ReadAllText(filePath);
            List<GameObject> objects = new List<GameObject>();
            gamePieces = new List<GameObject>();
            for (int i = 0; i < worldData.Length; i++)
            {
                bool endTile = false;
                bool endLine = false;
                count++;
                 
                switch (worldData[i])
                {
                    case '/':
                        endLine = true;
                        break;

                    case '_':
                        endTile = true;
                        break;

                    case 'f':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;

                    case 'w':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;
                    case 'd':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;
                    case 'e':
                        //Debug.Log("Enemy at index: "+ enemies[worldData[i + 1]-1]+"; Enemies Total: "+enemies.Count);
                        objects.Add(enemies[0]);
                        break;

                    case 'b':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;

                    case 'm':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;

                    case 't':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        break;

                    case 'i':
                        objects.Add(GameObject.Find(worldData[i] + "" + worldData[i + 1]));
                        objects.Add(playerObject);

                        break;

                    default:

                        break;
                }
                if (endTile)
                {
                    GameObject[] newObjects = new GameObject[objects.Count];
                    for(int j = 0; j < objects.Count; j++)
                    {
                        GameObject newObject = NewWorldObject(objects[j], x, y);
                        newObject.SetActive(true);
                        if (newObject.CompareTag("Player")){
                            CameraObject.GetComponent<CameraTracker>().UpdatePlayer(newObject);
                            HealthObject.GetComponent<CameraTracker>().UpdatePlayer(newObject);
                            HealthObject.GetComponent<HealthBarController>().UpdatePlayer(newObject);
                        }
                        if (!(objects[j].CompareTag("Floor") || objects[j].CompareTag("Decoration")))
                        {
                            gamePieces.Add(newObject);
                        }
                    }
                    x += tileSize;
                    objects = new List<GameObject>();
                }else if (endLine)
                {
                    x = baseX;
                    y -= tileSize;
                    lengthOfWorld = count;
                    count = 0;
                }
            }
        }
        else
        {
            while (true)
            {
                Debug.Log("AHHHHH");
            }
        }
    }
    public GameObject NewWorldObject(GameObject newObject, float x, float y)
    {
        return Instantiate(newObject, new Vector3(x, y, 0.0F), new Quaternion(0, 0, 0, 0));
    }

    public void DestroyWorld()
    {
        float size = lengthOfWorld * tileSize;
        Collider2D[] hits = Physics2D.OverlapBoxAll(new Vector2(baseX - (size / 2), basey - (size / 2)), new Vector2(size,size), 0F);
        for(int i  = 0; i < hits.Length; i++)
        {
            Destroy(hits[i].gameObject);
        }
    }
    private void UpdateLayers()
    {
        float height;
        int order;
        for(int i  = 0; i < gamePieces.Count; i++)
        {
            if (gamePieces[i])
            {
                height = basey - gamePieces[i].transform.position.y;
                order = 2 * (int)(height / tileSize);
                if (gamePieces[i].CompareTag("Wall"))
                {
                    order++;
                }
                else if (gamePieces[i].CompareTag("Wall2")) {
                    order += 2;
                }
                gamePieces[i].GetComponent<SpriteRenderer>().sortingOrder = order;
            }
        }
    }
    public void UpdateLevel(string filePathUpdate)
    {
        filePath = filePathUpdate;
        BuildWorld();
    }
}