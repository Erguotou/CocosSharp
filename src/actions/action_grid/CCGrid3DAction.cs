using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace CocosSharp
{
    public class CCGrid3DAction : CCGridAction
    {

        #region Constructors

        protected CCGrid3DAction (float duration) : base (duration)
        {
        }

        protected CCGrid3DAction (float duration, CCGridSize gridSize) : this (duration, gridSize, 0)
        {
        }

        protected CCGrid3DAction (float duration, CCGridSize gridSize, float amplitude)
            : base (duration, gridSize, amplitude)
        {
        }

        #endregion Constructors


        protected internal override CCActionState StartAction(CCNode target)
        {
            return new CCGrid3DActionState (this, target);
        }
    }


    #region Action state

    public class CCGrid3DActionState : CCGridActionState
    {
        CCRenderTexture gridRenderTexture;
        CCTexture2D gridTexture;
        CCGrid3D grid3D;
        CCSize gridTextureSizeInPoints;
        CCSize gridTextureSizeInPixels;

        public override CCGridBase Grid 
        {
            get 
            {
                CCSprite spriteTarget = Target as CCSprite;

                if (spriteTarget != null && !spriteTarget.ContentSize.Equals (CCSize.Zero))
                {
                    gridTextureSizeInPixels = spriteTarget.TextureRectInPixels.Size;
                    gridTextureSizeInPoints = spriteTarget.ContentSize;

                    gridTexture = new CCTexture2D(
                        (int)gridTextureSizeInPixels.Width, (int)gridTextureSizeInPixels.Height, CCSurfaceFormat.Color, true, false);
                } 
                else 
                {
                    gridTextureSizeInPixels = Target.Scene.Viewport.ViewportInPixels.Size;
                    gridTextureSizeInPoints = Target.Layer.VisibleBoundsWorldspace.Size;
                }

                gridRenderTexture = new CCRenderTexture(gridTextureSizeInPoints, gridTextureSizeInPixels);
                gridTexture = gridRenderTexture.Texture;
                grid3D = new CCGrid3D (GridSize, gridTexture);
  
                grid3D.Scene = Scene;

                return grid3D;
            }

            protected set 
            {
                Debug.Assert (value is CCGrid3D);
                grid3D = (CCGrid3D)value;
            }
        }

        public CCGrid3DActionState (CCGrid3DAction action, CCNode target) : base (action, target)
        {
        }


        #region Grid Vertex manipulation

        /// <summary>
        /// returns the vertex at a given position
        /// </summary>
        public CCVertex3F Vertex (CCGridSize pos)
        {
            return grid3D [pos];
        }

        /// <summary>
        /// returns the vertex at a given position
        /// </summary>
        public CCVertex3F Vertex (int x, int y)
        {
            return grid3D [x, y];
        }

        /// <summary>
        /// returns the original (non-transformed) vertex at a given position
        /// </summary>
        public CCVertex3F OriginalVertex (CCGridSize pos)
        {
            return grid3D.OriginalVertex (pos);
        }

        /// <summary>
        /// returns the original (non-transformed) vertex at a given position
        /// </summary>
        public CCVertex3F OriginalVertex (int x, int y)
        {
            return grid3D.OriginalVertex (x, y);
        }

        /// <summary>
        /// sets a new vertex at a given position
        /// </summary>
        public void SetVertex (CCGridSize pos, ref CCVertex3F vertex)
        {
            grid3D [pos] = vertex;
        }

        /// <summary>
        /// sets a new vertex at a given position
        /// </summary>
        public void SetVertex (int x, int y, ref CCVertex3F vertex)
        {
            grid3D [x, y] = vertex;
        }

        #endregion Grid Vertex manipulation
    }

    #endregion Action state
}