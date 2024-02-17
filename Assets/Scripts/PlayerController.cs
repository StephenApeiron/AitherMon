using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStatus))]
public class PlayerController : MonoBehaviour
{
    CharacterStatus characterStatus;
    Animator anim;
    public Transform canvasInfo;

    public bool isPlayer;

    public Transform target;

    Vector3 defaultPos;

    public bool isDie;
    public bool isStartMove;
    public bool isStartAttack;
    public bool isStartReturn;


    Vector3 v3Ref = Vector3.zero;


    [Header("Hurt Glow")]
    public float hurtGlowSpd = .5f;
    bool isStartHurtGlow = false;
    float currentHurtGlowTime = 0;
    public AnimationCurve hurtGlowCurve;
    List<Material> skinMats = new List<Material>();


    bool isFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        characterStatus = GetComponent<CharacterStatus>();
        anim = transform.Find("skin").GetChild(0).GetComponent<Animator>();
        defaultPos = transform.position;


        foreach (Transform child in transform.Find("skin").GetChild(0))
        {
            print(child.name);
            if (child.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
            {

                skinMats.Add(skinnedMeshRenderer.material);
            }
        }

        AutoSearchTarget();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown("0"))
        {
            if(isPlayer)
            {
                StartAttack();
                print("diu");

            }
        }*/


        CheckCharacterState();
        UpdateEnergy();

        if(isStartHurtGlow)
        {
            UpdateHurtGlow();
        }

        if(target.GetComponent<PlayerController>().isDie)
        {
            if(!isFinished)
            {
                anim.CrossFade("happy", .1f);
                isFinished = true;
            }
                
        }
    }

    void AutoSearchTarget()
    {
        GameObject[] tempTargets;
        if (isPlayer)
            tempTargets = GameObject.FindGameObjectsWithTag("Enemy");
        else
            tempTargets = GameObject.FindGameObjectsWithTag("Player");

        if (tempTargets.Length > 0)
        {
            target = tempTargets[0].transform;

            foreach(GameObject tempT in tempTargets)
            {
                float curTargetDis = Vector3.Distance(transform.position, target.position);
                float tempDis = Vector3.Distance(transform.position, tempT.transform.position);
                if (tempDis < curTargetDis)
                {
                    target = tempT.transform;
                }
            }
        }
    }


    void UpdateHurtGlow()
    {
        if (currentHurtGlowTime < 1)
        {
            currentHurtGlowTime += Time.deltaTime * (1f/hurtGlowSpd);
            foreach (Material m in skinMats)
            {
                m.SetFloat("_RimPower", hurtGlowCurve.Evaluate(currentHurtGlowTime));

            }
        }
        else
        {
            isStartHurtGlow = false;
        }
    }

    void UpdateEnergy()
    {
        if(!isDie)
        {
            if (characterStatus.curEnergy < characterStatus.maxEnergy)
            {
                characterStatus.curEnergy += characterStatus.spd * 1f / 100;
                canvasInfo.Find("energy bar/bar").localScale = new Vector3(characterStatus.curEnergy / characterStatus.maxEnergy, 1, 1);
            }
            else
            {
                characterStatus.curEnergy = characterStatus.maxEnergy;

                if (!target.GetComponent<PlayerController>().isDie) //<- dont attack died target
                {
                    if(anim.GetCurrentAnimatorStateInfo(0).IsName("idle")) //<- make sure finish hurt animation than attack
                    {
                        StartAttack();
                        print("diu");
                        characterStatus.curEnergy = 0;
                    }

                }
            }

            
        }
        
    }

    void CheckCharacterState()
    {
        if (isStartMove)
        {
            if (Vector3.Distance(transform.position, target.position) > 2)
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.position, ref v3Ref, .5f);
            }
            else
            {
                isStartMove = false;
                isStartAttack = true;

                StartCoroutine(PlayAttackAnimation());
            }
        }

        if (isStartReturn)
        {
            if (Vector3.Distance(transform.position, defaultPos) > .1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, defaultPos, ref v3Ref, .5f);
            }
            else
            {
                isStartReturn = false;
                print("end attack");
            }
        }
    }

    public void StartAttack()
    {
        isStartMove = true;
        isStartAttack = false;
        isStartReturn = false;

        anim.Play("fast run");
    }

    public IEnumerator PlayAttackAnimation()
    {
        print("start attack");
        int randomAtk = Random.Range(0, 3);
        switch(randomAtk)
        {
            case 0:
                anim.Play("attack1");
                break;

            case 1:
                anim.Play("attack2");
                break;

            case 2:
                anim.Play("attack3");
                break;

        }

        //target.GetComponent<PlayerController>().BeingAttack(1550);

        yield return new WaitForSeconds(0);
        //isStartAttack = false;
        //isStartReturn = true;
    }

    public IEnumerator PlayAttackAnimationEnd()
    {
        yield return new WaitForSeconds(0);
        isStartAttack = false;
        isStartReturn = true;
    }

    public void BeingAttack(int damage)
    {
        isStartReturn = false;

        //target.GetComponent<PlayerController>().BeingAttack(550);
        characterStatus.currentHP -= damage;
        float tempHpBarScale = characterStatus.currentHP * 1f / characterStatus.maxHp;

        canvasInfo.Find("hp bar bg/hp_bar").localScale = new Vector3(tempHpBarScale, 1, 1);

        //start glow
        isStartHurtGlow = true;
        currentHurtGlowTime = 0;

        if (characterStatus.currentHP > 0)
        {
            anim.CrossFade("hurt1",.01f);
        }
        else
        {
            isDie = true;
            anim.CrossFade("die1", .1f);
            canvasInfo.Find("hp bar bg/hp_bar").localScale = new Vector3(0, 1, 1);
        }
        


    }
}
