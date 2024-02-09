using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// Base class for all RhythmEngine extensions.
    /// </summary>
    public abstract class RhythmEngineExtension : MonoBehaviour
    {
        /// <summary>
        /// The RhythmEngine instance that this extension is attached to
        /// </summary>
        [Tooltip("The RhythmEngine instance that this extension is attached to.")]
        public RhythmEngineCore RhythmEngine;
    }
}
