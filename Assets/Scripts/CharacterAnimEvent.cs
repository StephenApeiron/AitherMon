using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimEvent : MonoBehaviour
{
    PlayerController playerController;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.parent.parent.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackHit()
    {
        playerController.target.GetComponent<PlayerController>().BeingAttack(2550);
    }

    public void AttackEnd()
    {
        anim.CrossFade("idle", .1f);

        StartCoroutine(playerController.PlayAttackAnimationEnd());

    }


    public void AnimEnd()
    {
        anim.CrossFade("idle", .25f);
    }

    public void CreateVFX(GameObject vfx)
    {
        GameObject tempVfx = Instantiate(vfx, transform.position, transform.rotation);

        tempVfx.transform.SetParent(GameObject.Find("VFX Group").transform);
    }
}
