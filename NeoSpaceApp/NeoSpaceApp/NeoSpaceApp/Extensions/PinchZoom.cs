using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSpaceApp.Extensions
{
    class PinchZoom
    {
        /// <summary>
        /// Calculates the scaling factor you should apply to the object being manipulated. A scaling factor of 2 means you
        /// should multiply your object's scale by 2. This assumes that the center of scaling is at the center of the object
        /// when you draw it.
        /// </summary>
        /// <param name="position1">The position of the first finger in the pinch gesture, in screen-space.</param>
        /// <param name="position2">The position of the second finger in the pinch gesture, in screen-space.</param>
        /// <param name="delta1">The delta of the first finger in the pinch gesture.</param>
        /// <param name="delta2">The delta of the second finger in the pinch gesture.</param>
        /// <returns>The scaling factor to apply to your object.</returns>
        public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 delta1, Vector2 delta2)
        {
            Vector2 oldPosition1 = position1 - delta1;
            Vector2 oldPosition2 = position2 - delta2;

            float distance = Vector2.Distance(position1, position2);
            float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

            if (oldDistance == 0 || distance == 0)
            {
                return 1.0f;
            }

            return distance / oldDistance;
        }

        /// <summary>
        /// Calculates the amount you should translate your object by during a pinch gesture.
        /// </summary>
        /// <param name="position1">The position of the first finger in the pinch gesture, in screen-space.</param>
        /// <param name="position2">The position of the second finger in the pinch gesture, in screen-space.</param>
        /// <param name="delta1">The delta of the first finger in the pinch gesture.</param>
        /// <param name="delta2">The delta of the second finger in the pinch gesture.</param>
        /// <param name="objectPos">The current position of your object, in screen-space.</param>
        /// <param name="scaleFactor">The current scale factor of your object. Call GetScaleFactor to retreive this.</param>
        /// <returns></returns>
        public static Vector2 GetTranslationDelta(Vector2 position1, Vector2 position2, Vector2 delta1, Vector2 delta2,
            Vector2 objectPos, float scaleFactor)
        {
            Vector2 oldPosition1 = position1 - delta1;
            Vector2 oldPosition2 = position2 - delta2;

            Vector2 newPos1 = position1 + (objectPos - oldPosition1) * scaleFactor;
            Vector2 newPos2 = position2 + (objectPos - oldPosition2) * scaleFactor;
            Vector2 newPos = (newPos1 + newPos2) / 2;

            return newPos - objectPos;
        }
    }
}
