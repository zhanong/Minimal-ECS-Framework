using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZhTool
{

    public class GPool<T> where T : MonoBehaviour
    {
        Stack<T> m_Stack = new Stack<T>();
        UnityAction<T> m_ActionOnCreate;
        public UnityAction<T> ActionOnCreate { get => m_ActionOnCreate; set => m_ActionOnCreate = value; }
        UnityAction<T> m_ActionOnGet;
        public UnityAction<T> ActionOnGet { get => m_ActionOnGet; set => m_ActionOnGet = value; }
        UnityAction<T> m_ActionOnRelease;
        public UnityAction<T> ActionOnRelease { get => m_ActionOnRelease; set => m_ActionOnRelease = value; }
        GameObject m_Prefab;
        Transform m_Parent;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public GPool(GameObject prefab, Transform parent, int capacity)
        {
            m_Prefab = prefab;
            m_Parent = parent;
            m_Stack = new(capacity);
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = InstantiateElement();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
                // element.gameObject.SetActive(true);
            }

            m_ActionOnGet?.Invoke(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool");

            m_ActionOnRelease?.Invoke(element);

            if (element is IPoolObject poolObject)
                poolObject.Reset();

            if (element.transform.parent != m_Parent)
                element.transform.SetParent(m_Parent);
            element.gameObject.SetActive(false);
            m_Stack.Push(element);
        }

        public void Create(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T element = InstantiateElement();


                element.gameObject.SetActive(false);
                m_Stack.Push(element);
            }
            countAll += count;
        }

        T InstantiateElement()
        {
            T element = GameObject.Instantiate(m_Prefab, m_Parent).GetComponent<T>();
            element.gameObject.name = m_Prefab.name + "_" + countAll;

            if (element is IPoolObject poolObject)
                poolObject.Reset();

            ActionOnCreate?.Invoke(element);

            return element;
        }
    }
    public class GPool 
    {
        Stack<GameObject> m_Stack = new Stack<GameObject>();

        public UnityAction<GameObject> actionOnCreate;
        public UnityAction<GameObject> actionOnGet;
        public UnityAction<GameObject> actionOnRelease;

        GameObject m_Prefab;
        Transform m_Parent;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public GPool(GameObject prefab, Transform parent, int capacity)
        {
            m_Prefab = prefab;
            m_Parent = parent;
            m_Stack = new(capacity);
        }

        public GameObject Get()
        {
            GameObject element;
            if (m_Stack.Count == 0)
            {
                element = InstantiateElement();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
                // element.gameObject.SetActive(true);
            }

            actionOnGet?.Invoke(element);
            return element;
        }

        public void Release(GameObject element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool");

            actionOnRelease?.Invoke(element);

            if (element.transform.parent != m_Parent)
                element.transform.SetParent(m_Parent);
            element.gameObject.SetActive(false);
            m_Stack.Push(element);
        }

        public void Create(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject element = InstantiateElement();


                element.gameObject.SetActive(false);
                m_Stack.Push(element);
            }
            countAll += count;
        }

        GameObject InstantiateElement()
        {
            GameObject element = GameObject.Instantiate(m_Prefab, m_Parent);
            element.gameObject.name = m_Prefab.name + "_" + countAll;

            actionOnCreate?.Invoke(element);

            return element;
        }


    }

    public interface IPoolObject
    {
        public void Reset();
    }

}