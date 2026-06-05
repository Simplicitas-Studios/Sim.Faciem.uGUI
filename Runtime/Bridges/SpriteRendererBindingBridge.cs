using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Sim.Faciem.uGUI.Bridges
{
    public class SpriteRendererBindingBridge : BindingBridge<SpriteRenderer>
    {
        [SerializeField]
        private BindableProperty<Sprite> _sprite = new();

        [SerializeField]
        private BindableProperty<Color> _color = new();

        [SerializeField]
        private BindableProperty<bool> _flipX = new();

        [SerializeField]
        private BindableProperty<bool> _flipY = new();

        [SerializeField]
        private BindableProperty<SpriteDrawMode> _drawMode = new();

        [SerializeField]
        private BindableProperty<SpriteMaskInteraction> _spriteMaskInteraction = new();

        [SerializeField]
        private BindableProperty<SpriteSortPoint> _spriteSortPoint = new();

        [SerializeField]
        private BindableProperty<Material> _material = new();

        [SerializeField]
        private BindableProperty<SortingLayer> _sortingLayer = new();

        [SerializeField]
        private BindableProperty<int> _orderInLayer = new();

        [SerializeField]
        private BindableProperty<RenderingLayerMask> _renderingLayerMask = new();

        private void Awake()
        {
            if (!_sprite.BindingInfo.IsDefault)
            {
                _sprite.CreateBinding();
                _sprite.ObserveChanges()
                    .Subscribe(next => Component.sprite = next)
                    .AddTo(this);
            }

            if (!_color.BindingInfo.IsDefault)
            {
                _color.CreateBinding();
                _color.ObserveChanges()
                    .Subscribe(next => Component.color = next)
                    .AddTo(this);
            }

            if (!_flipX.BindingInfo.IsDefault)
            {
                _flipX.CreateBinding();
                _flipX.ObserveChanges()
                    .Subscribe(next => Component.flipX = next)
                    .AddTo(this);
            }

            if (!_flipY.BindingInfo.IsDefault)
            {
                _flipY.CreateBinding();
                _flipY.ObserveChanges()
                    .Subscribe(next => Component.flipY = next)
                    .AddTo(this);
            }

            if (!_drawMode.BindingInfo.IsDefault)
            {
                _drawMode.CreateBinding();
                _drawMode.ObserveChanges()
                    .Subscribe(next => Component.drawMode = next)
                    .AddTo(this);
            }

            if (!_spriteMaskInteraction.BindingInfo.IsDefault)
            {
                _spriteMaskInteraction.CreateBinding();
                _spriteMaskInteraction.ObserveChanges()
                    .Subscribe(next => Component.maskInteraction = next)
                    .AddTo(this);
            }

            if (!_spriteSortPoint.BindingInfo.IsDefault)
            {
                _spriteSortPoint.CreateBinding();
                _spriteSortPoint.ObserveChanges()
                    .Subscribe(next => Component.spriteSortPoint = next)
                    .AddTo(this);
            }

            if (!_material.BindingInfo.IsDefault)
            {
                _material.CreateBinding();
                _material.ObserveChanges()
                    .Subscribe(next => Component.material = next)
                    .AddTo(this);
            }

            if (!_sortingLayer.BindingInfo.IsDefault)
            {
                _sortingLayer.CreateBinding();
                _sortingLayer.ObserveChanges()
                    .Subscribe(next => Component.sortingLayerID = next.id)
                    .AddTo(this);
            }

            if (!_orderInLayer.BindingInfo.IsDefault)
            {
                _orderInLayer.CreateBinding();
                _orderInLayer.ObserveChanges()
                    .Subscribe(next => Component.sortingOrder = next)
                    .AddTo(this);
            }

            if (!_renderingLayerMask.BindingInfo.IsDefault)
            {
                _renderingLayerMask.CreateBinding();
                _renderingLayerMask.ObserveChanges()
                    .Subscribe(next => Component.renderingLayerMask = next)
                    .AddTo(this);
            }
        }

        public override bool TryGetBindableProperty(string componentPropertyName,
            out IBindableProperty bindableProperty)
        {
            bindableProperty = componentPropertyName switch
            {
                "m_Sprite" => _sprite,
                "m_Color" => _color,
                "m_FlipX" => _flipX,
                "m_FlipY" => _flipY,
                "m_DrawMode" => _drawMode,
                "m_MaskInteraction" => _spriteMaskInteraction,
                "m_SpriteSortPoint" => _spriteSortPoint,
                "m_Materials.Array.data[0]" => _material,
                "m_SortingLayerID" => _sortingLayer,
                "m_SortingOrder" => _orderInLayer,
                _ => null
            };

            return bindableProperty != null;
        }

        public override IEnumerable<IBindableProperty> CollectBindableProperties()
        {
            yield return _sprite;
            yield return _color;
            yield return _flipX;
            yield return _flipY;
            yield return _drawMode;
            yield return _spriteMaskInteraction;
            yield return _spriteSortPoint;
            yield return _material;
            yield return _sortingLayer;
            yield return _orderInLayer;
            yield return _renderingLayerMask;
        }
    }
}
