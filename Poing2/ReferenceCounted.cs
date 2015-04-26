using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{


    

    /// <summary>
    /// Implements a reference counted list of T, allowing for implicit conversion to type T.
    /// A "Reference counted list" keeps track of both the items in the list as well as a reference count for each.
    /// When an item is added, it's reference count is incremented. If the item already exists, the existing item has it's reference count incremented.
    /// Equality is determined by default using the Equals Method, but a predicate can be assigned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ReferenceCounted<T>
    {



        private T lastitem = default(T); 
        private Dictionary<T, int> ReferenceDictionary= new Dictionary<T, int>();
        /// <summary>
        /// Returns the Dictionary holding the Elements.
        /// </summary>
        /// <returns></returns>
        public Dictionary<T, int> getReferenceDictionary() { return ReferenceDictionary; }
        /// <summary>
        /// creates a new ReferenceCounted List Object.
        /// </summary>
        public ReferenceCounted()
        {



        }
        /// <summary>
        /// creates a new ReferenceCounted List Object, with the given IEqualityComparer as the comparison predicate.
        /// </summary>
        /// <param name="compareobject"></param>
        public ReferenceCounted(IEqualityComparer<T> compareobject)
        {
            //initialize Dictionary with given IEqualityComparer interface.
            ReferenceDictionary = new Dictionary<T, int>(compareobject);
            

        }
        /// <summary>
        /// adds the given element to this list; which will either add an nonexistent item to the list and set it's reference count to 1 or
        /// increment the value of the existing element for the given value, as determined by any IEqualityComparer predicate given in the constructor.
        /// </summary>
        /// <param name="value">Value to add or increment in the list.</param>
        public void AddElement(T value)
        {
            //adds an element to this ReferenceCounted object.
            //remove the previous item... or dereference it, rather.

            if (!ReferenceDictionary.Comparer.Equals(lastitem, default(T)))
            {
                RemoveElement(lastitem);
                lastitem = default(T);
            }
            //first, does the given item exist in our dictionary already?
            if (!ReferenceDictionary.ContainsKey(value))
            {
                //if not, we add it. Add it with a value 0, since it will be incremented after this if.
                ReferenceDictionary.Add(value, 0);

                
                
            }
            //due to the above condition we know the element exists; increment it's reference count.
            ReferenceDictionary[value]++;
            _Dirty = true;

        }
        /// <summary>
        /// "removes" the given element from this list. This is accomplished by decrementing the appropriate keyed item in the dictionary.
        /// If the element is not in this dictionary, this method has no effect.
        /// </summary>
        /// <param name="value"></param>
        public void RemoveElement(T value)
        {
            if (!ReferenceDictionary.ContainsKey(value))
                return; //nothing to do if no item.

            ReferenceDictionary[value]--;
            //if reference count is zero: remove it.
            if (ReferenceDictionary[value] == 0) ReferenceDictionary.Remove(value);
            _Dirty = true;

        }
        T _CurrentMaximum=default(T);
        bool _Dirty = true; //whether the max is out of date.
        /// <summary>
        /// retrieves the item in this list with the maximum reference count.
        /// If multiple items have the maximum, only the first one encountered will be returned.
        /// This method also caches the result; calls only calculate a new maximum if necessary. (if the list was changed since the last one was cached).
        /// 
        /// </summary>
        /// <returns></returns>
        public T getMaxReferenced()
        {
            //find the item with the maximum value and return it's key.

            if (!_Dirty) return _CurrentMaximum;


            //otherwise, we're "dirty" and need to re-find the maximum again.
            int maxfound_int = int.MinValue;
            T maxfound_T = default(T);
            foreach (var iterate in ReferenceDictionary)
            {
                if (iterate.Value > maxfound_int)
                {
                    maxfound_int = iterate.Value;
                    maxfound_T = iterate.Key;

                }




            }
            _CurrentMaximum = maxfound_T;
            _Dirty = false;
            return _CurrentMaximum;


        }
        //Operator+; designed to be used like events- rather than code using = to "assign" a ReferenceCounted<T> value,
        //it will use +=
    public static ReferenceCounted<T> operator+(ReferenceCounted<T> firstvalue, T secondvalue)
    {
        firstvalue.AddElement(secondvalue);
        
        return firstvalue; //return the ReferenceCounted<T> object.
        
    }
        /// <summary>
        /// Operator that calls RemoveElement. The result will be the ReferenceCounted object itself.
        /// </summary>
        /// <param name="firstvalue"></param>
        /// <param name="secondvalue"></param>
        /// <returns></returns>
    public static ReferenceCounted<T> operator -(ReferenceCounted<T> firstvalue, T secondvalue)
    {
        firstvalue.RemoveElement(secondvalue);
        
        return firstvalue;

    }
        /// <summary>
        /// implicitly converts this list to the type of it's component. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
    public static implicit operator T(ReferenceCounted<T> value)
    {

        return value.getMaxReferenced();


    }


      




    }
}
