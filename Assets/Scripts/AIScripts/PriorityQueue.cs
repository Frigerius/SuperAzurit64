using System.Collections;
using System.Collections.Generic;

//Quelle: http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
public class PriorityQueue<T>
{
    private List<PriorityItem<T>> data;

    public PriorityQueue()
    {
        data = new List<PriorityItem<T>>();
    }

    public bool Empty()
    {
        return data.Count == 0;
    }

    public void Enqueue(T item, double f)
    {
        data.Add(new PriorityItem<T>(item, f));
        int ci = data.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (data[ci].f >= data[pi].f) break; // child item is larger than (or equal) parent so we're done
            PriorityItem<T> tmp = data[ci];
            data[ci] = data[pi];
            data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        // assumes pq is not empty; up to calling code
        int li = data.Count - 1; // last index (before removal)
        PriorityItem<T> frontItem = data[0];   // fetch the front
        data[0] = data[li];
        data.RemoveAt(li);

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        while (true)
        {
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && data[rc].f < data[ci].f) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                ci = rc;
            if (data[pi].f <= data[ci].f) break; // parent is smaller than (or equal to) smallest child so done
            PriorityItem<T> tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
            pi = ci;
        }
        return frontItem.Value;
    }

    public T Peek()
    {
        T frontItem = data[0].Value;
        return frontItem;
    }

    public void DecreaseKey(T item, double f)
    {
        int ci = data.IndexOf(new PriorityItem<T>(item, -1));
        data[ci].f = f;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (data[ci].f >= data[pi].f) break; // child item is larger than (or equal) parent so we're done
            PriorityItem<T> tmp = data[ci];
            data[ci] = data[pi];
            data[pi] = tmp;
            ci = pi;
        }
    }

    public int Count()
    {
        return data.Count;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "count = " + data.Count;
        return s;
    }

    public bool Contains(T elem)
    {
        return data.Contains(new PriorityItem<T>(elem, -1));
    }

    public bool IsConsistent()
    {
        // is the heap property true for all data?
        if (data.Count == 0) return true;
        int li = data.Count - 1; // last index
        for (int pi = 0; pi < data.Count; ++pi) // each parent index
        {
            int lci = 2 * pi + 1; // left child index
            int rci = 2 * pi + 2; // right child index

            if (lci <= li && data[pi].f > data[lci].f) return false; // if lc exists and it's greater than parent then bad.
            if (rci <= li && data[pi].f > data[rci].f) return false; // check the right child too.
        }
        return true; // passed all checks
    } // IsConsistent


    private class PriorityItem<N>
    {
        N value;
        public double f;

        public PriorityItem(N value, double f)
        {
            this.value = value;
            this.f = f;
        }

        public N Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public override bool Equals(object obj)
        {

            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            PriorityItem<N> otherT = obj as PriorityItem<N>;
            N other = otherT.Value;

            if (other == null)
            {
                return false;
            }

            // Return true if the fields match:
            return value.Equals(other);
        }

        public override int GetHashCode()
        {
            return -1584136870 + EqualityComparer<N>.Default.GetHashCode(value);
        }
    }
} // PriorityQueue