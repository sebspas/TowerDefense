using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackComponent : AttackComponent
{
    public GameObject spawnPoint;
    public GameObject bullet;
    public GameObject head;

    public override void AttackTarget()
    {
        var clone = PoolSystem.Instance.RequestGameObject(bullet.tag);

        clone.transform.position = spawnPoint.transform.position;
        clone.transform.rotation = spawnPoint.transform.rotation;

        var rigidbody = clone.GetComponent<Rigidbody>();
        rigidbody.velocity = (_target.transform.position - spawnPoint.transform.position).normalized
                             * bullet.GetComponent<Bullet>().speed;
        
        clone.SetActive(true);
    }

    public override void RotateTowardTarget(GameObject target)
    {
        // Determine which direction to rotate towards
        var targetDirection = target.transform.position - head.transform.position;

        // The step size is equal to speed times frame time.
        var singleStep = rotateSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        var newDirection = Vector3.RotateTowards(head.transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(head.transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        head.transform.rotation = Quaternion.LookRotation(new Vector3(newDirection.x, 0.0f, newDirection.z));
    }
}
