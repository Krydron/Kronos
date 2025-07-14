using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ProBuilder;

public class EnemySpriteManager : MonoBehaviour
{
    Animator animator;
    [SerializeField,Range(4,32)] int animations;
    float[] rotations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Direction", 2);

        rotations = new float[animations];
        float segment = 360/animations;
        float halfSegment = segment/2;
        for (int i = 0; i < animations; i++)
        {
            rotations[i] = (segment * i) - halfSegment;
            if (rotations[i] < 0)
            {
                rotations[i] += 360;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<SpriteRenderer>().flipX = false;
        float rotation = transform.eulerAngles.y - transform.parent.eulerAngles.y;
        rotation -= 135;
        if (rotation < 0 )
        {
            rotation += 360;
        }
        //Debug.Log(rotation);
        if (rotation > 0 && rotation < 90)
        {
            //Debug.Log("face forward");
            animator.SetInteger("Direction", 0);
        }
        else if (rotation > 90 && rotation < 180)
        {
            //Debug.Log("face left");
            animator.SetInteger("Direction", 1);
        }
        else if (rotation > 180 && rotation < 270)
        {
            //Debug.Log("face back");
            animator.SetInteger("Direction", 2);
        }
        else if (rotation > 270 && rotation < 360)
        {
            //Debug.Log("face right");
            animator.SetInteger("Direction", 3);
            //GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
