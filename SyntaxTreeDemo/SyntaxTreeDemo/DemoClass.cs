using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxTreeDemo
{
    public class DemoClass : ICollection<string>
    {
        private bool Dummy { get; set; }

        private void DummyMethod()
        {
            
        }

        #region Dummy ICollection implementation
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(string item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
