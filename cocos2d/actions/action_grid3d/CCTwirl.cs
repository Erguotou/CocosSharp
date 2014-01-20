using System;

namespace CocosSharp
{
    public class CCTwirl : CCGrid3DAction
    {
        protected float m_fAmplitude;
        protected float m_fAmplitudeRate;
        protected int m_nTwirls;
        protected CCPoint m_position;
        protected CCPoint m_positionInPixels;

        public CCPoint Position
        {
            get { return m_position; }
            set
            {
                m_position = value;
                m_positionInPixels.X = value.X * CCDirector.SharedDirector.ContentScaleFactor;
                m_positionInPixels.Y = value.Y * CCDirector.SharedDirector.ContentScaleFactor;
            }
        }

        public float Amplitude
        {
            get { return m_fAmplitude; }
            set { m_fAmplitude = value; }
        }

        public override float AmplitudeRate
        {
            get { return m_fAmplitudeRate; }
            set { m_fAmplitudeRate = value; }
        }


        #region Constructors

        protected CCTwirl()
        {
        }

        public CCTwirl(float duration, CCGridSize gridSize, CCPoint position, int twirls, float amplitude) : base(duration, gridSize)
        {
            InitCCTwirl(position, twirls, amplitude);
        }

        public CCTwirl(CCTwirl twirl) : base(twirl)
        {
            InitCCTwirl(twirl.Position, twirl.m_nTwirls, twirl.m_fAmplitude);
        }

        private void InitCCTwirl(CCPoint position, int twirls, float amplitude)
        {  
            m_positionInPixels = new CCPoint();
            Position = position;
            m_nTwirls = twirls;
            m_fAmplitude = amplitude;
            m_fAmplitudeRate = 1.0f;
        }

        #endregion Constructors


        public override object Copy(ICCCopyable pZone)
        {
            return new CCTwirl(this);
        }

        public override void Update(float time)
        {
            int i, j;
            CCPoint c = m_positionInPixels;

            for (i = 0; i < (m_sGridSize.X + 1); ++i)
            {
                for (j = 0; j < (m_sGridSize.Y + 1); ++j)
                {
                    CCVertex3F v = OriginalVertex(new CCGridSize(i, j));

                    var avg = new CCPoint(i - (m_sGridSize.X / 2.0f), j - (m_sGridSize.Y / 2.0f));
                    var r = (float) Math.Sqrt((avg.X * avg.X + avg.Y * avg.Y));

                    float amp = 0.1f * m_fAmplitude * m_fAmplitudeRate;
                    float a = r * (float) Math.Cos((float) Math.PI / 2.0f + time * (float) Math.PI * m_nTwirls * 2) *
                              amp;

                    float dx = (float) Math.Sin(a) * (v.Y - c.Y) + (float) Math.Cos(a) * (v.X - c.X);
                    float dy = (float) Math.Cos(a) * (v.Y - c.Y) - (float) Math.Sin(a) * (v.X - c.X);

                    v.X = c.X + dx;
                    v.Y = c.Y + dy;

                    SetVertex(new CCGridSize(i, j), ref v);
                }
            }
        }
    }
}