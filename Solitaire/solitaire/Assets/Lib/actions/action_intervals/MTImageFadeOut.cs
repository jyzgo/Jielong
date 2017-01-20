using UnityEngine;
using UnityEngine.UI;

namespace MTUnity.Actions
{
    public class MTImageFadeOut : MTFiniteTimeAction
    {
        #region Constructors

        public float Alpha;
        public MTImageFadeOut (float durtaion) : base (durtaion)
        {
        
        }

        #endregion Constructors

        protected internal override MTActionState StartAction(GameObject target)
        {
            return new MTImageFadeOutState (this, target);

        }

        public override MTFiniteTimeAction Reverse ()
        {
            return new MTFadeIn (Duration);
        }
    }

    public class MTImageFadeOutState : MTFiniteTimeActionState
    {
        Image img;
        float startTime;
        float endTime;
        float cost = 0f;
        public MTImageFadeOutState (MTImageFadeOut action, GameObject target)
            : base (action, target)
        {
            img = target.GetComponent<Image>();
            startTime = Time.time;
            endTime = Time.time + Duration;
            
        }

        public override void Update (float time)
        {
           
            if (img != null)
            {
                cost += Time.deltaTime;
                var curColor = img.color;
                img.color = new Color(curColor.r, curColor.g, curColor.b, 1-cost/Duration);
            }
        }

    }

}