using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using MTUnity.Actions;

namespace MTUnity.Utils
{
	static class ThreadSafeRandom
	{
		[ThreadStatic]
		private static System.Random Local;
	
		public static System.Random ThisThreadsRandom {
			get { return Local ?? (Local = new System.Random (unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}
	}

	public static class MTExtension
	{
		public static void RandomShuffle<T> (this IList<T> list)
		{
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = ThreadSafeRandom.ThisThreadsRandom.Next (n + 1);
				T value = list [k];
				list [k] = list [n];
				list [n] = value;
			}
		}

		static int _flag = 0;
		public static int GetRandomInt()
		{
			_flag++;
			if(_flag == int.MaxValue)
			{
				_flag = int.MinValue;
			}

			return ThreadSafeRandom.ThisThreadsRandom.Next (_flag);
		}





		public static void PlayParticle (this GameObject target)
		{
			if(target == null)
			{
				return;
			}
			var anim = target.GetComponent<ParticleSystem> ();
			if (anim != null) {
//				anim.Stop();
//				anim.Clear();
//				anim.Simulate(0.02f);
				anim.Play();
				var em = anim.emission;
				em.enabled = true;
			} 

			int childCount = target.transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				target.transform.GetChild (i).gameObject.PlayParticle();
			}

		}

		public static void StopParticle(this GameObject target)
		{
			if(target == null)
			{
				return;
			}
			var anim = target.GetComponent<ParticleSystem> ();
			if (anim != null) {
				var em = anim.emission;
				em.enabled = false;
				anim.Stop();
			} 
			
			int childCount = target.transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				target.transform.GetChild (i).gameObject.StopParticle();
			}
		}



		

	}
}