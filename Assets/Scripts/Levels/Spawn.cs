using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn
{
    public string enemy; // do we make the enemy instance here or in enemyspawner? im leaning towards the latter
    // need to implement way to interpret RPN
    public string count;
    public string hp;
    public string delay;
    public string damage;

    public List<int> sequence;
    public string location;

}