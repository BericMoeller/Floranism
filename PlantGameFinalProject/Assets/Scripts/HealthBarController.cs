using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public GameObject playerObject;
    public PlayerController playerController;
    public float ratio;
    public int maxHealth;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void UpdatePlayer(GameObject player)
    {
        playerObject = player;
        playerController = playerObject.GetComponent<PlayerController>();
        maxHealth = playerController.MAX_HEALTH;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (playerObject != null)
        {
            animator.SetFloat("HealthVal", (float)(playerController.health) / maxHealth);
        }
    }
}
