using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    [Header("Move Info")]
    public Joystick joystick;
    [SerializeField] private float moveSpeed;
    private Vector3 playerMove;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMove.x = joystick.Horizontal; 
        playerMove.z = joystick.Vertical;
        playerMove.y = 0;

        Vector3 movement = playerMove * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
        anim.SetFloat("Speed", playerMove.sqrMagnitude);

        if (playerMove.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(playerMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
}
