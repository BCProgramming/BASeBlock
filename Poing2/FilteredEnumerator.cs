using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    public interface IFilteredEnumerator<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> source); 


    }

    public class FilteredEnumerator<T>:IFilteredEnumerator<T>
    {
        Func<T, bool> testRoutine = null; 
        public FilteredEnumerator(Func<T, bool> ptestroutine)
        {
            testRoutine = ptestroutine;

        }

        public IEnumerable<T> Filter(IEnumerable<T> source )
        {
            return source.Where(iterate => testRoutine == null || testRoutine(iterate));
        }
    }
}
