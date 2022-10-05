using System.Collections;
using System.Collections.Generic;

public class LinkedList<T> : IEnumerable<T>
{
    Node<T> first;
    Node<T> last;
    int _count;

    public int Count
    {
        get { return _count; }
    }

    public T this[int index]
    {
        get
        {
            Node<T> current = first;

            for (int i = 0; i < index; i++)
                current = current.next;

            return current.myObj;
        }
        set
        {
            Node<T> current = first;

            for (int i = 0; i < index; i++)
                current = current.next;

            current.myObj = value;
        }
    }

    public void Add(T obj)
    {
        Node<T> newNode = new Node<T>();
        newNode.myObj = obj;

        if(last != null)
        {
            last.next = newNode;
            last = newNode;
        }else
        {
            first = newNode;
            last = newNode;
        }

        _count++;
    }

    public void Remove (T obj) 
    {
        Node<T> current = first;

        if(EqualityComparer<T>.Default.Equals(current.myObj, obj))
        {
            first = current.next;
            _count--;
            return;
        }

        for (int i = 0; i < _count -1; i++)
        {
            if(EqualityComparer<T>.Default.Equals(current.next.myObj, obj))
            {
                Node<T> nodeRemove = current.next;

                current.next = nodeRemove.next;
                _count--;
                break;
            }
            current = current.next;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = first;
        while (current != null)
        {
            yield return current.myObj;
            current = current.next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
