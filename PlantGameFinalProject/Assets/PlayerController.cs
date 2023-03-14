using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Vector3 velocity;
    private float FrictionConstant = 0.007F;
    Animator animator;
    bool running = false;
    int dirFacing = 0; // 0 is up, 1 is right, 2 is down, 3 is left
    SpriteRenderer SpriteRenderer;
    bool hasPressedMove;
    public int health = 0;
    public int maxHealth = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        velocity = new Vector3(0F, 0F, 0F);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hasPressedMove = false;
        if (Input.GetKey(KeyCode.W))
        {
            hasPressedMove = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            hasPressedMove = true;
        }
        if (Input.GetKey(KeyCode.A))
        {   
            hasPressedMove = true;
        }
        if (Input.GetKey(KeyCode.D))
        {   
            hasPressedMove = true;
        }
    }
    void collisionBehavior(Collision2D collision){
        //find out what side it collided with, prevent movement to that side
        //CHECK FOR MULTIPLE
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionBehavior(collision);
    }
    private void OnCollisionStay2D(Collision2D collision){
        collisionBehavior(collision);
    }
    public void Run()
    {
        animator.SetInteger("State", 1);
    }
    public void Idle()
    {
        animator.SetInteger("State", 0);
    }
    public void Jump()
    {
        animator.SetInteger("State", 2);
    }
}
