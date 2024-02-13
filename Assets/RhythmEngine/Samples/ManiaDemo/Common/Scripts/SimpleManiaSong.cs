using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine.Examples
{
    /// <summary>
    /// This version of a Song contains all the data needed to play the mania demo.
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleManiaSong", menuName = "RhythmEngine/Songs/Examples/SimpleManiaSong")]
    // Feel free to uncomment the above line if you want to create more mania song assets.
    public class SimpleManiaSong : ScriptableObject
    {
        [Header("Mania Notes")]
        public List<SimpleManiaNote> Notes = new();
    }
}
