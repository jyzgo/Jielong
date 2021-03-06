﻿using UnityEngine;
using System.Collections;

namespace MTUnity.Actions {
	public static class MTActionExtension  {
		
		
		#region Actions
		
		public static bool IsRunning(this GameObject target)
		{
			bool isActive = false;
			if (target && target.gameObject && target.gameObject.activeInHierarchy) {
				isActive = true;
			}
			return isActive;
		}
		
		public static void AddAction(this GameObject target,MTAction action, bool paused = false)
		{
			if (MTActionManager.instance != null)
				MTActionManager.instance.AddAction(action, target, paused);
			
		}
		
		public static void AddActions(this GameObject target,bool paused, params MTFiniteTimeAction[] actions)
		{
			if (MTActionManager.instance != null)
				MTActionManager.instance.AddAction(new MTSequence(actions), target, paused);
			
		}
		
		public static MTActionState Repeat(this GameObject target, uint times, params MTFiniteTimeAction[] actions)
		{
			return target.RunAction (new MTRepeat (new MTSequence(actions), times));
		}
		
		public static MTActionState Repeat (this GameObject target, uint times, MTFiniteTimeAction action)
		{
			return  target.RunAction (new MTRepeat (action, times));
		}
		
		public static MTActionState RepeatForever(this GameObject target, params MTFiniteTimeAction[] actions)
		{
			return target.RunAction(new MTRepeatForever (actions));
		}
		
		public static MTActionState RepeatForever(this GameObject target, MTFiniteTimeAction action)
		{
			return target.RunAction(new MTRepeatForever (action) { Tag = action.Tag });
		}
		
		public static MTActionState RunAction(this MonoBehaviour target, MTAction action)
		{
			Debug.Assert(action != null, "Argument must be non-nil");
			
			GameObject curObj = target.gameObject;
			return  curObj.RunAction (action);
		}
		
		public static MTActionState RunAction(this GameObject target, MTAction action)
		{
			Debug.Assert(action != null, "Argument must be non-nil");
			
			
			return  MTActionManager.instance.AddAction(action, target, !target.IsRunning());
		}
		
		public static MTActionState RunActions(this MonoBehaviour beh, params MTFiniteTimeAction[] actions)
		{
			GameObject curObj = beh.gameObject;
			return curObj.RunActions (actions);
		}
		
		public static MTActionState RunActions(this GameObject target, params MTFiniteTimeAction[] actions)
		{
			Debug.Assert(actions != null, "Argument must be non-nil");
			Debug.Assert(actions.Length > 0, "Paremeter: actions has length of zero. At least one action must be set to run.");
			
			
			var action = actions.Length > 1 ? new MTSequence(actions) : actions[0];
			
			return MTActionManager.instance.AddAction (action, target, !target.IsRunning());
		}

		public static void SetParticleScale(this GameObject target,float scale)
		{
			var particleSys = target.GetComponent<ParticleSystem>();
			if(particleSys != null)
			{
				particleSys.startSize *= scale;
			}
			int childCount = target.transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				target.transform.GetChild(i).gameObject.SetParticleScale(scale);
			}
		}
		
		public static void Hide(this GameObject target)
		{
			
			float curA = 0f;
			
			var render = target.GetComponent<Renderer>();
			if (render != null)// && render.material.HasProperty("_Color")) 
			{
				if(render is SpriteRenderer)
				{
					SpriteRenderer curRender = render as SpriteRenderer;
					
					Color originColor = curRender.color;
					var newColor = new Color (originColor.r,originColor.g,originColor.b,curA);
					curRender.color = newColor;
					
					
				}
				else if (render.material != null && render.material.HasProperty("_Color"))
				{
					var originColor = render.material.color;
					var newColor = new Color (originColor.r,originColor.g,originColor.b,curA);
					render.material.color = newColor;
					
				}
				
				
			}
			
			int childCount = target.transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				target.transform.GetChild(i).gameObject.Hide();
			}
			
		}
		
		
		
		public static void StopAllActions(this GameObject target)
		{
			if(MTActionManager.instance != null)
				MTActionManager.instance.RemoveAllActionsFromTarget(target);
		}
		
		public static void StopAllActions(this MonoBehaviour target)
		{
			target.gameObject.StopAllActions ();
		}
		
		public static void StopAction(this GameObject target, MTActionState actionState)
		{
			if(MTActionManager.instance != null)
				MTActionManager.instance.RemoveAction(actionState);
		}
		
		public static void StopAction(this MonoBehaviour target,MTActionState actionState)
		{
			target.gameObject.StopAction (actionState);
		}
		
		public static void StopAction(this GameObject target, int tag)
		{
			Debug.Assert(tag != -1, "Invalid tag");
			MTActionManager.instance.RemoveAction(tag, target);
		}
		public static void StopAction(this MonoBehaviour target,int tag)
		{
			target.gameObject.StopAction (tag);
		}
		
		public static MTAction GetAction(this GameObject target, int tag)
		{
			Debug.Assert(tag != -1, "Invalid tag");
			return MTActionManager.instance.GetAction(tag, target);
		}
		
		public static MTActionState GetActionState(this GameObject target, int tag)
		{
			Debug.Assert(tag != -1, "Invalid tag");
			return MTActionManager.instance.GetActionState(tag, target);
		}
		
		#endregion Actions
		
		public static bool getVisible(this GameObject target)
		{
			if (target) 
			{
				var render = target.GetComponent<Renderer> ();
				if (render && render.enabled == true) {
					return true;
				} 
				
			}
			
			return false;
		}
		
		public static void setVisible(this GameObject target ,bool curVis)
		{
			if (target) 
			{
				var render = target.GetComponent<Renderer> ();
				if (render) {
					render.enabled = curVis;
				}
				
			}
		}
		
		public static float getOpacity(this GameObject target)
		{
			if (target) 
			{
				var convasGroup = target.GetComponent<CanvasGroup> ();
				if (convasGroup != null) {
					
					return convasGroup.alpha ;
				}
				
				
				var render = target.GetComponent<Renderer>();
				if (render != null)// && render.material.HasProperty("_Color")) 
				{
					if(render is SpriteRenderer)
					{
						SpriteRenderer curRender = render as SpriteRenderer;
						
						return curRender.color.a;
						
						
					}
					else if (render.material != null && render.material.HasProperty("_Color"))
					{
						var originColor = render.material.color;
						return originColor.a;
						
					}
					
					
				}
				
			}
			return 0f;
		}
		
		public static void setOpacity(this Transform curTransform,float curA)
		{
			var target = curTransform.gameObject;
			target.setOpacity (curA);
		}
		
		public static float getOpacity(this Transform curTransform)
		{
			GameObject target = curTransform.gameObject;
			return target.getOpacity ();
		}
		
		public static void setOpacity(this GameObject target,float curA)
		{
			if (target) 
			{
				
				var convasGroup = target.GetComponent<CanvasGroup> ();
				if (convasGroup != null) {
					convasGroup.alpha = curA;
					return;
				}
				
				
				var render = target.GetComponent<Renderer>();
				if (render != null)// && render.material.HasProperty("_Color")) 
				{
					if(render is SpriteRenderer)
					{
						SpriteRenderer curRender = render as SpriteRenderer;
						
						Color originColor = curRender.color;
						var newColor = new Color (originColor.r,originColor.g,originColor.b,curA);
						curRender.color = newColor;
						
						
					}
					else if (render.material != null && render.material.HasProperty("_Color"))
					{
						var originColor = render.material.color;
						var newColor = new Color (originColor.r,originColor.g,originColor.b,curA);
						render.material.color = newColor;
						
					}
					
					
				}
				
				int childCount = target.transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					target.transform.GetChild(i).gameObject.setOpacity(curA);
				}
				
			}
			
			
		}
		
		public static Color getColor(this GameObject target)
		{
			
			if (target) 
			{
				var render = target.GetComponent<Renderer>();
				if (render && render.material) 
				{
					var originColor = render.material.color;
					return originColor;
				}
				
			}
			
			return new Color();
		}
		
		public static void setColor(this GameObject target, Color curColor)
		{
			if (target) 
			{
				var render = target.GetComponent<Renderer>();
				if (render && render.material) 
				{
					render.material.color = curColor;
				}
				
			}
		}
		
	}
	
}