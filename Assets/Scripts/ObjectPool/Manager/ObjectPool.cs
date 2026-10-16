using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Rts2D.ObjectPool
{
    public class ObjectPool<T> where T : Component
    {
        private readonly IInstantiator _instantiator;
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _inactive = new Stack<T>();

        public ObjectPool(IInstantiator instantiator, T prefab, Transform container, int prewarm = 0)
        {
            _instantiator = instantiator;
            _prefab = prefab;
            _container = container;

            for (int i = 0; i < prewarm; i++)
                Despawn(CreateNew());
        }

        public T Spawn(Transform parent = null)
        {
            T item = _inactive.Count > 0 ? _inactive.Pop() : CreateNew();

            item.transform.SetParent(parent != null ? parent : _container, false);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Despawn(T item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_container, false);
            _inactive.Push(item);
        }

        private T CreateNew()
        {
            return _instantiator.InstantiatePrefabForComponent<T>(_prefab.gameObject, _container);
        }
    }
}
