using UnityEngine;
using System.Collections;
public class WallTileController : MonoBehaviour, IPlayerCanCollide
{
    public void OnPlayerCollision()
    {
        LevelController.instance.playerCharacterMover.StopMoving();
        if (LevelController.instance.playerCharacterMover.canFill == true)
            FloodFill.ChooseSideAndFill();
    }

}
