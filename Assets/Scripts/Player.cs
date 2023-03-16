using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    //Move the player
    private void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0) * speed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? transform.position.x * -1 : transform.position.x , Mathf.Clamp(transform.position.y, -4, 2), 0);
    }

    //Determine horizontal movement
    private float MoveHorizontal()
    {
        switch (Input.GetAxis("Horizontal"))
        {
            case >0:
                return Input.GetAxis("Horizontal");
            //return transform.position.x > 10 ? 0 : Input.GetAxis("Horizontal");
            case < 0:
                return Input.GetAxis("Horizontal");
            //return transform.position.x < -10 ? 0 : Input.GetAxis("Horizontal");
            case 0:
            default:
                return 0;
        }
    }

    //Determine vertical movement
    private float MoveVertical()
    {
        switch (Input.GetAxis("Vertical"))
        {
            case >0:
                return Input.GetAxis("Vertical");
                //return transform.position.y > 2 ? 0 : Input.GetAxis("Vertical");
            case <0:
                //return transform.position.y < -4 ? 0 : Input.GetAxis("Vertical");
            case 0:
            default:
                return 0;
        }
    }
}