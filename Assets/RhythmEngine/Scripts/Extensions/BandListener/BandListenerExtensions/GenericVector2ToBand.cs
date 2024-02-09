using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Generic Vector2 To Band")]
    public class GenericVector2ToBand : GenericBandExtension<Vector2>
    {
        protected override Vector2 Lerp(Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);
    }
}
