using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private PostProcessProfile blurProfile;
    [SerializeField] private PostProcessProfile normalProfile;
    [SerializeField] private PostProcessVolume postProcessVolume;

    public void EnableCameraBlur(bool state)
    {
        if (postProcessVolume != null && blurProfile != null && normalProfile != null)
        {
            postProcessVolume.profile = (state) ? blurProfile : normalProfile;
        }
    }
}
