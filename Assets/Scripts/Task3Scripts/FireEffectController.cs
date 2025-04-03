using UnityEngine;
using UnityEngine.UI;

public class FireEffectController : MonoBehaviour
{
    [SerializeField] private Animator fireAnimator;
    [SerializeField] private Button toggleButton;
    
    private bool _isFireOn = true;
    private static readonly int IsFireOn = Animator.StringToHash("IsFireOn");

    private void Start()
    {
        toggleButton.onClick.AddListener(ToggleFire);
    }

    private void ToggleFire()
    {
        _isFireOn = !_isFireOn;
        fireAnimator.SetBool(IsFireOn, _isFireOn);
    }
}
