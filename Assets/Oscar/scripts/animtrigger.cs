using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animtrigger : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        AnimationClip death = null;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == "Armature|death")
            {
                death = clip;
            }

        AnimationEvent ev = new()
        {
            time = 3.3f,
            functionName = "Despawn"
        };
        death.AddEvent(ev);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("attacktrigger");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("trigger");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("deathtrigger");
        }
    }

    public void Despawn()
    {
        GetComponentInParent<Enemy>().Despawn();
    }
}
