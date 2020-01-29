using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 2.0f;
    public List<IHealthModifier> attackEffect = new List<IHealthModifier>();

    private float _destroyAfterTime = 3.0f;
    private float _elapsedTime = 0.0f;

    public void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _destroyAfterTime)
        {            
            PoolSystem.Instance.AddBackToPool(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ennemy")
        {
            HealthComponent targetHealthComponent = other.gameObject.GetComponent<HealthComponent>();
            if (!targetHealthComponent)
            {
                Debug.Log("Trying to attack a target without a healthComponent.");
                return;
            }

            // Apply effect
            foreach (var effect in attackEffect)
            {                
                targetHealthComponent.AddModifier(effect);
            }

            PoolSystem.Instance.AddBackToPool(gameObject);
        }
    }
}
