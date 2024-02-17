using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int currentHP = 10000;
    public int maxHp = 10000;
    public int atk = 100;
    public int def = 50;
    public int res = 50;
    public int spd = 10;

    [Header("Battle Status")]
    public float curEnergy = 0;
    public int maxEnergy = 100;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
