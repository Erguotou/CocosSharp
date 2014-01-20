using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CocosSharp
{
    public class CCCardinalSplineTo : CCActionInterval
    {
        protected float m_fDeltaT;
        protected float m_fTension;
        protected List<CCPoint> m_pPoints;
        protected CCPoint m_previousPosition;
        protected CCPoint m_accumulatedDiff;

        public List<CCPoint> Points
        {
            get { return m_pPoints; }
            set { m_pPoints = value; }
        }


        #region Constructors

        public CCCardinalSplineTo(float duration, List<CCPoint> points, float tension) : base(duration)
        {
            Init(points, tension);
        }

        // Perform a deep copy of CCACtionInterval
        protected CCCardinalSplineTo(CCCardinalSplineTo cardinalSplineTo) : base(cardinalSplineTo)
        {
            Init(cardinalSplineTo.m_pPoints, cardinalSplineTo.m_fTension);
        }

        private void Init(List<CCPoint> points, float tension)
        {
            Debug.Assert(points.Count > 0, "Invalid configuration. It must at least have one control point");

            Points = points;
            m_fTension = tension;
        }

        #endregion Constructors


        // This should be changed to DeepCopy()
        public override object Copy(ICCCopyable pZone)
        {
            return new CCCardinalSplineTo(this);
        }

        protected internal override void StartWithTarget(CCNode target)
        {
            base.StartWithTarget(target);

            m_fDeltaT = 1f / m_pPoints.Count;

            m_previousPosition = target.Position;
            m_accumulatedDiff = CCPoint.Zero;
        }

        public override void Update(float time)
        {
            int p;
            float lt;

            // eg.
            // p..p..p..p..p..p..p
            // 1..2..3..4..5..6..7
            // want p to be 1, 2, 3, 4, 5, 6
            if (time == 1)
            {
                p = m_pPoints.Count - 1;
                lt = 1;
            }
            else
            {
                p = (int) (time / m_fDeltaT);
                lt = (time - m_fDeltaT * p) / m_fDeltaT;
            }

            // Interpolate    
            var c = m_pPoints.Count - 1;
            CCPoint pp0 = m_pPoints[Math.Min(c, Math.Max(p - 1, 0))];
            CCPoint pp1 = m_pPoints[Math.Min(c, Math.Max(p + 0, 0))];
            CCPoint pp2 = m_pPoints[Math.Min(c, Math.Max(p + 1, 0))];
            CCPoint pp3 = m_pPoints[Math.Min(c, Math.Max(p + 2, 0))];

            CCPoint newPos = CCSplineMath.CCCardinalSplineAt(pp0, pp1, pp2, pp3, m_fTension, lt);

            // Support for stacked actions
            CCNode node = m_pTarget;
            CCPoint diff = node.Position - m_previousPosition;
            if (diff.X != 0 || diff.Y != 0)
            {
                m_accumulatedDiff = m_accumulatedDiff + diff;
                newPos = newPos + m_accumulatedDiff;
            }

            UpdatePosition(newPos);
        }

        public override CCFiniteTimeAction Reverse()
        {
            List<CCPoint> pReverse = m_pPoints.ToList();
            pReverse.Reverse();

            return new CCCardinalSplineTo(m_fDuration, pReverse, m_fTension);
        }

        public virtual void UpdatePosition(CCPoint newPos)
        {
            m_pTarget.Position = newPos;
            m_previousPosition = newPos;
        }
    }

    public class CCCardinalSplineBy : CCCardinalSplineTo
    {
        protected CCPoint m_startPosition;


        #region Constructors

        public CCCardinalSplineBy(float duration, List<CCPoint> points, float tension) : base(duration, points, tension)
        {
        }

        #endregion Constructors


        protected internal override void StartWithTarget(CCNode target)
        {
            base.StartWithTarget(target);
            m_startPosition = target.Position;
        }

        public override CCFiniteTimeAction Reverse()
        {
            List<CCPoint> copyConfig = m_pPoints.ToList();

            //
            // convert "absolutes" to "diffs"
            //
            CCPoint p = copyConfig[0];

            for (int i = 1; i < copyConfig.Count; ++i)
            {
                CCPoint current = copyConfig[i];
                CCPoint diff = (current - p);
                copyConfig[i] = diff;

                p = current;
            }

            // convert to "diffs" to "reverse absolute"
            copyConfig.Reverse();

            // 1st element (which should be 0,0) should be here too

            p = copyConfig[copyConfig.Count - 1];
            copyConfig.RemoveAt(copyConfig.Count - 1);

            p = -p;
            copyConfig.Insert(0, p);

            for (int i = 1; i < copyConfig.Count; ++i)
            {
                CCPoint current = copyConfig[i];
                current = -current;
                CCPoint abs = current + p;
                copyConfig[i] = abs;

                p = abs;
            }

            return new CCCardinalSplineBy(m_fDuration, copyConfig, m_fTension);
        }

        public override void UpdatePosition(CCPoint newPos)
        {
            m_pTarget.Position = newPos + m_startPosition;
            m_previousPosition = m_pTarget.Position;
        }
    }

    public class CCCatmullRomTo : CCCardinalSplineTo
    {
        public CCCatmullRomTo(float dt, List<CCPoint> points) : base(dt, points, 0.5f)
        {
        }
    }

    public class CCCatmullRomBy : CCCardinalSplineBy
    {
        public CCCatmullRomBy(float dt, List<CCPoint> points) : base(dt, points, 0.5f)
        {
        }
    }
}