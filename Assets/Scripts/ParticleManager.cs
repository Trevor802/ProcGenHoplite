using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour{
    public static ParticleManager Instance {get; private set;} = null;
    public AutoDestroy AttackP;
    public AutoDestroy ThrowP;
    private void Awake() {
        Instance = this;
    }
    public void PlayParticle(Vector3 position, Vector3 direction, AutoDestroy particle){
        Instantiate<GameObject>(particle.gameObject, position, Quaternion.LookRotation(direction));
    }
}