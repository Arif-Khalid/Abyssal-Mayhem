using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : Bullet
{
    //Inheriting from the Bullet class
    //Pistol bullet is a simple bullet that travels straight forward
    //Damages enemies on hit
    //Is destroyed on hit
    //Expires(destroyed) after a certain time of no collision
    public override void HitSomething(Collider other)
    {
        base.HitSomething(other);
    }
}
