using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Generic Color To Band")]
    public class GenericColorToBand : GenericBandExtension<Color>
    {
        protected override Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, t);
    }
}
