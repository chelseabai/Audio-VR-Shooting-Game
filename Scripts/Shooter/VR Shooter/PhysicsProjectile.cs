using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectile : Projectile
{
    private float lifeTime = 5;
    private Rigidbody rigidBody;
    private GameObject[] _LiveProjectiles;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        _LiveProjectiles = GameObject.FindGameObjectsWithTag("projectile");
        foreach (GameObject _LiveProjectile in _LiveProjectiles) {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), _LiveProjectile.GetComponent<Collider>(), true);
        }
        // Debug.Log("h");
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        Destroy(gameObject, lifeTime);
    }

    public override void Launch()
    {
        base.Launch();
        rigidBody.AddRelativeForce(Vector3.forward * weapon.GetShootingForce(), ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy(gameObject);
        ITakeDamage[] damageTakers = other.GetComponentsInParent<ITakeDamage>();

        foreach (var taker in damageTakers)
        {
            taker.TakeDamage(weapon, this, transform.position);
        }
    }

    private void OnCollisionEnter(Collision _collision)
    {
        if ((_collision.gameObject.tag != "projectile") && (_collision.gameObject.tag != "gun") && (_collision.gameObject.tag != "target") && (_collision.gameObject.tag != "Player")) {
            // Debug.Log("hiiiii");
            Destroy(gameObject);
        }
        if (_collision.gameObject.tag == "target") {
            // Debug.Log("hit");
            // Destroy(_collision.gameObject);
        }
    }
}