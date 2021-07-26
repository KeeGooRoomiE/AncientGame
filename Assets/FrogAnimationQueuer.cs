using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class FrogAnimationQueuer : MonoBehaviour
{
   [SerializeField] private AnimationStateData stateData;
   [SerializeField] private AnimationState state;

    // Start is called before the first frame update
    void Start()
    {
        //TrackEntry entry = addAnimation(1, "idle", true, 0.2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
