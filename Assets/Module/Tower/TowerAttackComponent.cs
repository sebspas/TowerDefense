using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackComponent : AttackComponent
{
    public GameObject spawnPoint;
    public GameObject bullet;

    public override void AttackTarget()
    {
        var clone = PoolSystem.Instance.RequestGameObject(bullet.tag);

        clone.transform.position = spawnPoint.transform.position;
        clone.transform.rotation = spawnPoint.transform.rotation;

        var rigidbody = clone.GetComponent<Rigidbody>();
        rigidbody.velocity = (_target.transform.position - spawnPoint.transform.position).normalized
                             * bullet.GetComponent<Bullet>().speed;         
    }
}
