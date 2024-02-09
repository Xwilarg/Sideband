using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Generic Value To Band")]
    public class GenericValueToBand : GenericBandExtension<float>
    {
        protected override float Lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
    }
}
