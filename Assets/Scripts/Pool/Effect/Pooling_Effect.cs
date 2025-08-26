using UnityEngine;

public class Pooling_Effect : PoolingItem
{
    [SerializeField] private ParticleSystem effect;

    public void Play() => effect.Play();

    private void OnParticleSystemStopped()
    {
        Release();
    }
}
