using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Generic Vector3 To Band")]
    public class GenericVector3ToBand : GenericBandExtension<Vector3>
    {
        protected override Vector3 Lerp(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
    }
}
