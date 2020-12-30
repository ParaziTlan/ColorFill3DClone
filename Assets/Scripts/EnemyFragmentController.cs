using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFragmentController : MonoBehaviour, IPlayerCanCollide
{
    private void OnTriggerEnter(Collider col)
    {
        IEnemyCanCollide enemyCollisionable = col.GetComponent<IEnemyCanCollide>();

        Debug.Log(col.name);

        if (enemyCollisionable != null)
        {
            if (enemyCollisionable.OnEnemyCollision() == true)
            {
                ParticleManager.instance.PlayEnemyParticle(transform.position);
                gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayerCollision()
    {
        if (LevelController.instance.playerCharacterMover.canFill == true)
        {
            Debug.Log("Player Enemy e çarptı");
            LevelController.instance.DisablePlayerTiles();
        }
    }


}
