using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public GameObject player;
    public float CameraOffset;
    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, CameraOffset);
        }
    }
    public void UpdatePlayer(GameObject playerIn)
    {
        player = playerIn;
    }
}
