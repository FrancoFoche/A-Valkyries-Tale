using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHashTable<K, V>
{
    private int size;
    private LinkedList<KeyValuePair<K, V>>[] items;

    public CustomHashTable(int size)
    {
        this.size = size;
        items = new LinkedList<KeyValuePair<K, V>>[size];
    }

    protected int GetArrayPosition(K key)
    {
        int position = key.GetHashCode() % size;
        return Mathf.Abs(position);
    }

    public V Find(K key)
    {
        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<K, V>> linkedList = GetLinkedList(position);
        foreach (KeyValuePair<K, V> item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                return item.Value;
            }
        }

        throw new System.Exception("Not found");
    }

    public void Add(K key, V value)
    {
        int position = GetArrayPosition(key);
        Debug.Log(key + "--" + position);
        LinkedList<KeyValuePair<K, V>> linkedList = GetLinkedList(position);
        KeyValuePair<K, V> item = new KeyValuePair<K, V>(key, value);

        linkedList.Add(item);
    }

    public void Remove(K key)
    {
        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<K, V>> linkedList = GetLinkedList(position);

        foreach (KeyValuePair<K, V> item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                linkedList.Remove(item);

                break;
            }
        }
    }

    protected LinkedList<KeyValuePair<K, V>> GetLinkedList(int position)
    {
        LinkedList<KeyValuePair<K, V>> linkedList = items[position];
        if (linkedList == null)
        {
            linkedList = new LinkedList<KeyValuePair<K, V>>();
            items[position] = linkedList;
        }

        return linkedList;
    }
}