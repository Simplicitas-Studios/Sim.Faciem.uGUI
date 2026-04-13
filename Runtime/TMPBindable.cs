using R3;
using TMPro;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPBindable : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _textControl;

        [SerializeField]
        private BindableProperty<string> _text;

        private void Awake()
        {
            _text.CreateBinding();
            _text.ObserveChanges()
                .Subscribe(text =>
                {
                    _textControl.SetText(text);
                }).AddTo(this);

            _text.AddTo(this);
        }

#if UNITY_EDITOR
        
        private void Reset()
        {
            _textControl = GetComponent<TMP_Text>();
        }
        
        #endif
    }
}