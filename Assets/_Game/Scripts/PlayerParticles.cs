using UnityEngine;


public class PlayerParticles : MonoBehaviour
{
    public float walkDustMinSpeed = 3.5f;
    public float landingParticleMinSpeed = 5f;

    public ParticleSystem walkDust;
    public ParticleSystem landDust;

    //protected Player m_player;

    /// <summary>
    /// Start playing a given particle.
    /// </summary>
    /// <param name="particle">The particle you want to play.</param>
     void Play(ParticleSystem particle)
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }

    /// <summary>
    /// Stop a given particle.
    /// </summary>
    /// <param name="particle">The particle you want to stop.</param>
    public void Stop(ParticleSystem particle)
    {
        if (particle.isPlaying)
        {
            particle.Stop();
        }
    }

    public void HandleWalkParticle()
    {
        Play(walkDust);
    }

    public void HandleLandParticle()
    {
        Play(landDust);
    }


}

