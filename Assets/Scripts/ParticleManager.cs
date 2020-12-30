using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }

    [SerializeField]
    private GameObject EnemyParticlePrefab, PlayerParticlePrefab;

    private List<ParticleSystem> particles = new List<ParticleSystem>();
    private GameObject particleParent;
    private ParticleSystem playerParticle;
    private int enemyParticleIndex = 0;
    private readonly int enemyParticleAmount = 100; // max height or width is 99! ... so 100 Will enough for objectPooling Lenght! 

    void Start()
    {
        particleParent = new GameObject("Particle Parent");
        for (int i = 0; i < enemyParticleAmount; i++)
        {
            ParticleSystem ps = Instantiate(EnemyParticlePrefab, particleParent.transform).GetComponent<ParticleSystem>();
            particles.Add(ps);
        }
        playerParticle = Instantiate(PlayerParticlePrefab, particleParent.transform).GetComponent<ParticleSystem>();
    }
    public void PlayEnemyParticle(Vector3 pos)
    {
        particles[enemyParticleIndex].transform.position = pos;
        particles[enemyParticleIndex].Play();

        enemyParticleIndex++;
        if (enemyParticleIndex >= enemyParticleAmount) enemyParticleIndex = 0;
    }
    public void PlayPlayerParticle(Vector3 pos)
    {
        playerParticle.transform.position = pos;
        playerParticle.Play();
    }
}
