using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mover_example : MonoBehaviour
{
    public GameObject[] feet;

    // Start is called before the first frame update
    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 setPos = transform.position;
        
        float ySum = 0;

        //Set y pos to the average height of all feets
        for(int i = 0; i < feet.Length; i++)
        {
            ySum += feet[i].transform.position.y;
        }

        float avgY = ySum/feet.Length;

        //Set rotation to look at the average Y value
        Vector3 lookAt = transform.position + transform.forward * 2;
        lookAt.y = avgY;

        setPos += transform.forward * (speed * Time.deltaTime);

        transform.position = setPos;


    }
}
