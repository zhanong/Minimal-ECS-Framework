using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZhTool
{

    public class SharedKey
    {
        public List<byte> keys;
        public int Capacity => keys.Count * 8;

        public SharedKey(int capacity)
        {
            keys = new List<byte>(capacity / 8 + 1);
        }

        public struct Key
        {
            public int id;
            public bool IsNull => id == 0;
        }

        public Key GetKey()
        {
            int index = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                if (IsFull(i))
                    index += 8;
                else
                {
                    var unit = keys[i];
                    for (int j = 0; j < 8; j++)
                    {
                        if (IsBitAvailable(unit, j))
                        {
                            unit = SetBit(unit, j, true);
                            keys[i] = unit;
                            return new Key { id = GetID(index + j) };
                        }
                    }
                }
            }
            keys.Add(0b00000001);
            return new Key { id = GetID(index) };
        }

        public void ReleaseKey(Key key)
        {
            var index = GetIndex(key.id);
            var unitIndex = index / 8;
            var bitIndex = index % 8;
            var unit = SetBit(keys[unitIndex], bitIndex, false);
            keys[unitIndex] = unit;
        }

        public bool IsAllAvailable()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] != 0)
                    return false;
            }
            return true;
        }

        public void Clear()
        {
            keys.Clear();
        }

        bool IsFull(int unitIndex)
        {
            return keys[unitIndex] == 0b11111111;
        }

        bool IsBitAvailable(byte unit, int bitIndex)
        {
            return (unit & (0b1 << bitIndex)) == 0;
        }

        byte SetBit(byte unit, int bitIndex, bool value)
        {
            if (value)
                unit |= (byte)(0b1 << bitIndex);
            else
                unit &= (byte)~(0b1 << bitIndex);
            return unit;
        }

        public int GetID(int index)
        {
            return index + 1;
        }

        public int GetIndex(int id)
        {
            return id - 1;
        }

    }

    public class ListPool<T>
    {
        List<List<T>> pool;
        List<bool> unavailable;
        int listCapacity;

        public ListPool(int poolCapacity, int listCapacity)
        {
            pool = new List<List<T>>(poolCapacity);
            unavailable = new List<bool>(poolCapacity);

            this.listCapacity = listCapacity;
        }

        public ListPoolItem Get(int capacity = -1)
        {
            if (capacity == -1)
                capacity = listCapacity;

            for (int i = 0; i < pool.Count; i++)
            {
                if (!unavailable[i])
                {
                    unavailable[i] = true;
                    var list = pool[i];
                    if (list.Capacity < capacity)
                        list.Capacity = capacity;
                    return new() { id = GetID(i), list = pool[i] };
                }
            }

            pool.Add(new List<T>(listCapacity));
            unavailable.Add(true);
            return new() { id = GetID(pool.Count - 1), list = pool[^1] };
        }

        public void Release(int id)
        {
            var index = GetIndex(id);
            var list = pool[index];
            list.Clear();
            if (list.Capacity > listCapacity * 2)
                list.Capacity = listCapacity;
            unavailable[index] = false;
        }

        public List<T> GetList(int id)
        {
            return pool[GetIndex(id)];
        }

        public int GetID(int index)
        {
            return index + 1;
        }

        public int GetIndex(int id)
        {
            return id - 1;
        }

        public struct ListPoolItem
        {
            public List<T> list;
            public int id;
        }
    }

    public class KeyPool
    {
        GameObject prefab;
        Transform parent;
        List<PoolItem> pool;

        public UnityAction<GameObject> onCreate, onGet, onRelease;

        public KeyPool(GameObject prefab, Transform parent, int Capacity)
        {
            this.prefab = prefab;
            this.parent = parent;
            pool = new(Capacity);
        }

        public GameObject Get(out Key key)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                var item = pool[i];
                if (!item.isBorrowed)
                {
                    item.isBorrowed = true;
                    pool[i] = item;

                    key = Key.FromIndex(i);
                    var obj = pool[i].obj;
                    onGet?.Invoke(obj);
                    return pool[i].obj;
                }
            }

            var newObj = GameObject.Instantiate(prefab, parent);
            onCreate?.Invoke(newObj);
            onGet?.Invoke(newObj);
            pool.Add(new PoolItem { obj = newObj, isBorrowed = true });
            key = Key.FromIndex(pool.Count - 1);
            return newObj;

        }

        public PoolItem Query(Key key)
        {
            return pool[key.ToIndex()];
        }

        public void Release(Key key)
        {
            var index = key.ToIndex();
            var item = pool[index];
            item.isBorrowed = false;

            onRelease?.Invoke(item.obj);
            item.obj.SetActive(false);

            pool[index] = item;
        }

        public struct Key
        {
            public int id;

            public int ToIndex()
            {
                return id - 1;
            }

            public static Key FromIndex(int index)
            {
                return new() { id = index + 1 };
            }
        }

        public struct PoolItem
        {
            public GameObject obj;
            public bool isBorrowed;
        }
    }

}