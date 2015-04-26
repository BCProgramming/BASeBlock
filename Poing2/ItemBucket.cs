using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    public class ItemBucket<T>
    {
        private int _dispensed = 0;
        private Queue<T> _elements;
        private T[] _original;
        private readonly Random rgenerator = new Random();

        public ItemBucket(Random rg, params T[] source) : this(rg, (IEnumerable<T>) source)
        {
        }
        private IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
        {
            
            return source.OrderBy<T, int>((item) => rgenerator.Next());
        }
        public ItemBucket(params T[] source) : this((IEnumerable<T>) source)
        {
        }

        public ItemBucket(Random rg, IEnumerable<T> source) : this(source)
        {
            rgenerator = rg;
        }

        public ItemBucket(IEnumerable<T> source)
        {
            _original = source.ToArray();
            _elements = new Queue<T>(Shuffle(_original));
        }

        public static T[] RepeatArray(T[] array, int count)
        {
            var resultarray = new List<T>(array);
            for (int i = 0; i < count; i++)
            {
                resultarray.AddRange(array);
            }
            return resultarray.ToArray();
        }
        public ItemBucket<T> AddRange(IEnumerable<T> Elements )
        {
            foreach (var iterate in Elements)
            {
                _elements.Enqueue(iterate);
                var neworiginal = new List<T>(_original) { iterate };
                _original = neworiginal.ToArray();
            }
            _elements = new Queue<T>(Shuffle(_elements));
            return this;


        }
        public ItemBucket<T> Add(T element)
        {
            _elements.Enqueue(element);
            _elements = new Queue<T>(Shuffle(_elements));
            var neworiginal = new List<T>(_original) {element};
            _original = neworiginal.ToArray();
            return this;
        }

        private T DispenseSingle()
        {
            //dispense a single item.
            //get an index into the Elements array.
            T retrieved = _elements.Dequeue();
            
            if (!_elements.Any())
            {
                //no elements, so re-create from Original.
                Debug.Print("Bucket Empty!");
                _elements = new Queue<T>(Shuffle(_original));
            }

            return retrieved;
        }

        public T Dispense()
        {
            return DispenseSingle();
        }

        public IEnumerable<T> Dispense(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return DispenseSingle();
            }
        }
    }
}