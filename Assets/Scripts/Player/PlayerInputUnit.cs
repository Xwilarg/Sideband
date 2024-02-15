using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmJam2024.Player
{
    public class PlayerInputUnit : MonoBehaviour
    {
        private HitArea _hitArea;

        public void Init(HitArea hitArea)
        {
            _hitArea = hitArea;
        }

        private void Start()
        {
            GameManager.Instance.RegisterPlayer(this);
        }

        private void OnHit(InputAction.CallbackContext value, int line)
        {
            if (_hitArea == null)
            {
                return;
            }

            if (value.phase == InputActionPhase.Started)
            {
                _hitArea.OnKeyDown(line);
                GameManager.Instance.HitNote(line, _hitArea.GetInstanceID());
            }
            else if (value.phase == InputActionPhase.Canceled)
            {
                _hitArea.OnKeyUp(line);
            }
        }

        public void OnHit1(InputAction.CallbackContext value) => OnHit(value, 0);
        public void OnHit2(InputAction.CallbackContext value) => OnHit(value, _hitArea != null && _hitArea.IsReversed ? 3 : 1);
        public void OnHit3(InputAction.CallbackContext value) => OnHit(value, 2);
        public void OnHit4(InputAction.CallbackContext value) => OnHit(value, _hitArea != null && _hitArea.IsReversed ? 1 : 3);
    }
}
