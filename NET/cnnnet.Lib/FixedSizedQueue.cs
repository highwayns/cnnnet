using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib
{
    public class FixedSizedQueue<T> : IEnumerable<T>
    {
        #region Fields
        
        private readonly ConcurrentQueue<T> _queue;

        #endregion

        #region Properties
        
        public int Size
        {
            get;
            private set;
        }

        #endregion

        #region Methods
        
        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);
            lock (this)
            {
                while (_queue.Count > Size)
                {
                    T outObj;
                    _queue.TryDequeue(out outObj);
                }
            }
        }

        public void Clear()
        {
            while (_queue.Count > 0)
            {
                T outObj;
                _queue.TryDequeue(out outObj);
            }
        }

        #region IEnumerable implementation
        
        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        #endregion

        #endregion

        #region Instance

        public FixedSizedQueue(int size)
        {
            Size = size;
            _queue = new ConcurrentQueue<T>();
        }

        #endregion
    }
}
