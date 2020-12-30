using UnityEngine;
using System.Collections;
public class FilledTileController : MonoBehaviour, ICollisionable
{
    private bool isAnimationStarted = false;
    public void OnPlayerCollision()
    {
        if (LevelController.instance.playerCharacterMover.canFill == true)
            FloodFill.ChooseSideAndFill();
    }
    public bool OnEnemyCollision()
    {
        return true;  // true means: enemyFragment can destroy itself
    }

    private void OnEnable()
    {
        FloodFill.FloodFinished += StartAnimation;
        LevelController.LevelPassedEvent += StartAnimation;
    }
    private void OnDisable()
    {
        FloodFill.FloodFinished -= StartAnimation;
        LevelController.LevelPassedEvent -= StartAnimation;
    }
    private void StartAnimation()
    {
        if (!isAnimationStarted)
        {
            isAnimationStarted = true;
            StartCoroutine(MoveUpAnimation());
        }
    }
    private IEnumerator MoveUpAnimation()
    {
        while (transform.position.y < 0.47f)
        {
            transform.position += new Vector3(0, 1, 0) * Time.deltaTime;
            ColorManager.instance.ChangeColor(transform.position.y.Remap(0, 0.47f, 0, 1));
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, 0.47f, transform.position.z);
        ColorManager.instance.ChangeColor(1);
        GetComponent<Renderer>().sharedMaterial = ColorManager.instance.FilledMat;
    }
}
