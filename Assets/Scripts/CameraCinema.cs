using UnityEngine;

/**
 * Helper class to control animations while taking videos
 * Not used in production, only for marketing purposes
 */
public class CameraCinema : MonoBehaviour
{
    public AssassinCinematic assassinCinematic;

    private Animator _animator;
    private bool _rotating;

    void Start() {
        _animator = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (_rotating) {
                _animator.SetBool("Rotate", false);
                _rotating = false;
            }
            else {
                _animator.SetBool("Rotate", true);
                _rotating = true;
            }
        }
    }

    public void ToggleADS() {
        assassinCinematic.ToggleAimAnim();
    }
}
