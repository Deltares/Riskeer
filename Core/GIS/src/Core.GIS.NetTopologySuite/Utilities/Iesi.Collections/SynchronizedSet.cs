/* Copyright � 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */ 
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Iesi_NTS.Collections
{
	/// <summary>
	/// <p>Implements a thread-safe <c>Set</c> wrapper.  The implementation is extremely conservative, 
	/// serializing critical sections to prevent possible deadlocks, and locking on everything.
	/// The one exception is for enumeration, which is inherently not thread-safe.  For this, you
	/// have to <c>lock</c> the <c>SyncRoot</c> object for the duration of the enumeration.</p>
	/// </summary>
	[Serializable]
	public sealed class SynchronizedSet : Set
	{
		private ISet   mBasisSet;
		private object mSyncRoot;

		/// <summary>
		/// Constructs a thread-safe <c>Set</c> wrapper.
		/// </summary>
		/// <param name="basisSet">The <c>Set</c> object that this object will wrap.</param>
		public SynchronizedSet(ISet basisSet)
		{
			mBasisSet = basisSet;
			mSyncRoot = basisSet.SyncRoot;
			if(mSyncRoot == null)
				throw new NullReferenceException("The Set you specified returned a null SyncRoot.");
		}

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="o">The object to add to the set.</param>
		/// <returns><c>true</c> is the object was added, <c>false</c> if it was already present.</returns>
		public sealed override bool Add(object o)
		{
			lock(mSyncRoot)
			{
				return mBasisSet.Add(o);
			}
		}

		/// <summary>
		/// Adds all the elements in the specified collection to the set if they are not already present.
		/// </summary>
		/// <param name="c">A collection of objects to add to the set.</param>
		/// <returns><c>true</c> is the set changed as a result of this operation, <c>false</c> if not.</returns>
		public sealed override bool AddAll(ICollection c)
		{
			Set temp;
			lock(c.SyncRoot)
			{
				temp = new HybridSet(c);
			}

			lock(mSyncRoot)
			{
				return mBasisSet.AddAll(temp);
			}
		}

		/// <summary>
		/// Removes all objects from the set.
		/// </summary>
		public sealed override void Clear()
		{
			lock(mSyncRoot)
			{
				mBasisSet.Clear();
			}
		}

		/// <summary>
		/// Returns <c>true</c> if this set contains the specified element.
		/// </summary>
		/// <param name="o">The element to look for.</param>
		/// <returns><c>true</c> if this set contains the specified element, <c>false</c> otherwise.</returns>
		public sealed override bool Contains(object o)
		{
			lock(mSyncRoot)
			{
				return mBasisSet.Contains(o);
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the set contains all the elements in the specified collection.
		/// </summary>
		/// <param name="c">A collection of objects.</param>
		/// <returns><c>true</c> if the set contains all the elements in the specified collection, <c>false</c> otherwise.</returns>
		public sealed override bool ContainsAll(ICollection c)
		{
			Set temp;
			lock(c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock(mSyncRoot)
			{
				return mBasisSet.ContainsAll(temp);
			}
		}

		/// <summary>
		/// Returns <c>true</c> if this set contains no elements.
		/// </summary>
		public sealed override bool IsEmpty
		{
			get
			{
				lock(mSyncRoot)
				{
					return mBasisSet.IsEmpty;
				}
			}
		}


		/// <summary>
		/// Removes the specified element from the set.
		/// </summary>
		/// <param name="o">The element to be removed.</param>
		/// <returns><c>true</c> if the set contained the specified element, <c>false</c> otherwise.</returns>
		public sealed override bool Remove(object o)
		{
			lock(mSyncRoot)
			{
				return mBasisSet.Remove(o);
			}
		}

		/// <summary>
		/// Remove all the specified elements from this set, if they exist in this set.
		/// </summary>
		/// <param name="c">A collection of elements to remove.</param>
		/// <returns><c>true</c> if the set was modified as a result of this operation.</returns>
		public sealed override bool RemoveAll(ICollection c)
		{
			Set temp;
			lock(c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock(mSyncRoot)
			{
				return mBasisSet.RemoveAll(temp);
			}
		}

		/// <summary>
		/// Retains only the elements in this set that are contained in the specified collection.
		/// </summary>
		/// <param name="c">Collection that defines the set of elements to be retained.</param>
		/// <returns><c>true</c> if this set changed as a result of this operation.</returns>
		public sealed override bool RetainAll(ICollection c)
		{
			Set temp;
			lock(c.SyncRoot)
			{
				temp = new HybridSet(c);
			}
			lock(mSyncRoot)
			{
				return mBasisSet.RetainAll(temp);
			}
		}

		/// <summary>
		/// Copies the elements in the <c>Set</c> to an array.  The type of array needs
		/// to be compatible with the objects in the <c>Set</c>, obviously.
		/// </summary>
		/// <param name="array">An array that will be the target of the copy operation.</param>
		/// <param name="index">The zero-based index where copying will start.</param>
		public sealed override void CopyTo(Array array, int index)
		{
			lock(mSyncRoot)
			{
				mBasisSet.CopyTo(array, index);
			}
		}

		/// <summary>
		/// The number of elements contained in this collection.
		/// </summary>
		public sealed override int Count
		{
			get
			{
				lock(mSyncRoot)
				{
					return mBasisSet.Count;
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c>, indicating that this object is thread-safe.  The exception to this
		/// is enumeration, which is inherently not thread-safe.  Use the <c>SyncRoot</c> object to
		/// lock this object for the entire duration of the enumeration.
		/// </summary>
		public sealed override bool IsSynchronized
		{
			get{return true;}
		}

		/// <summary>
		/// Returns an object that can be used to synchronize the <c>Set</c> between threads.
		/// </summary>
		public sealed override object SyncRoot
		{
			get{return mSyncRoot;}
		}

		/// <summary>
		/// Enumeration is, by definition, not thread-safe.  Use a <c>lock</c> on the <c>SyncRoot</c> 
		/// to synchronize the entire enumeration process.
		/// </summary>
		/// <returns></returns>
		public sealed override IEnumerator GetEnumerator()
		{
			return mBasisSet.GetEnumerator();
		}

		/// <summary>
		/// Returns a clone of the <c>Set</c> instance.  
		/// </summary>
		/// <returns>A clone of this object.</returns>
		public override object Clone()
		{
			return new SynchronizedSet((ISet)mBasisSet.Clone());
		}

	}
}
