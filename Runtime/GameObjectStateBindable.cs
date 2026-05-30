using R3;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public class GameObjectStateBindable : MonoBehaviour
    {
        [SerializeField]
        private BindableProperty<bool> _isActive;

        private void Awake()
        {
            _isActive.CreateBinding();
            _isActive.ObserveChanges()
                .Subscribe(isActive =>
                {
                    gameObject.SetActive(isActive);
                }).AddTo(this);

            _isActive.AddTo(this);
        }
    }
}
