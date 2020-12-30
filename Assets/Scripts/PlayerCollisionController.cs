using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        IPlayerCanCollide playerCollisionable = col.GetComponent<IPlayerCanCollide>();

        if (playerCollisionable != null)
            playerCollisionable.OnPlayerCollision();

    }

    private void OnEnable()
    {
        LevelController.GameOverEvent += GameOver;
    }
    private void OnDisable()
    {
        LevelController.GameOverEvent -= GameOver;
    }

    private void GameOver()
    {
        ParticleManager.instance.PlayPlayerParticle(transform.position);
        gameObject.SetActive(false);
    }
}
