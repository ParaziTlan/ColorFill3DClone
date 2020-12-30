using UnityEngine;

public class PlayerTileController : MonoBehaviour, ICollisionable
{
    bool isFirstHit = true;
    public void OnPlayerCollision()
    {
        if (isFirstHit) // ilk hit. spawn edildiği anda oluyor ! etkileşim olmasın diye return
        {
            isFirstHit = false;
            return;
        }

        Debug.LogWarning("Player kendi kuyruğuna çarptı");
        LevelController.instance.StartTileAnimation();
    }
    public bool OnEnemyCollision()
    {
        LevelController.instance.GameOver();
        Debug.Log("Enemy Player kuyruğuna çarptı");
        return false;
    }
}
