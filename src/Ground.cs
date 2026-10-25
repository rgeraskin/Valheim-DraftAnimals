using UnityEngine;

namespace DraftAnimals
{
    /// <summary>
    /// Пол под точкой — для зверей, которых мод ставит в мир. Ванильный ZoneSystem.GetSolidHeight бьёт лучом с высоты
    /// 1000 м и находит самую верхнюю поверхность — в доме это крыша; здесь луч идёт вниз от заданной высоты над точкой.
    /// </summary>
    internal static class Ground
    {
        private static int s_solidMask;

        private static int SolidMask => s_solidMask != 0
            ? s_solidMask
            : s_solidMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "terrain", "vehicle");

        /// <summary>
        /// Высота пола под point: первая твёрдая поверхность (не предмет и не персонаж) на луче вниз от point + above
        /// длиной above + depth. Не нашли — высота земли, но не ниже point.
        /// </summary>
        public static float FloorHeight(Vector3 point, float above, float depth)
        {
            Vector3 origin = point + Vector3.up * above;
            float best = float.MaxValue;
            float y = point.y;
            foreach (RaycastHit hit in Physics.RaycastAll(origin, Vector3.down, above + depth, SolidMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.attachedRigidbody == null && hit.distance < best)
                {
                    best = hit.distance;
                    y = hit.point.y;
                }
            }
            if (best < float.MaxValue)
            {
                return y;
            }
            return ZoneSystem.instance != null ? Mathf.Max(point.y, ZoneSystem.instance.GetGroundHeight(point)) : point.y;
        }

        /// <summary>Сколько можно пройти от from в сторону dir (не дальше distance), не упираясь в стену; margin — зазор.</summary>
        public static float FreeDistance(Vector3 from, Vector3 dir, float distance, float margin)
        {
            float free = distance;
            foreach (RaycastHit hit in Physics.RaycastAll(from, dir, distance, SolidMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.attachedRigidbody == null)
                {
                    free = Mathf.Min(free, hit.distance - margin);
                }
            }
            return Mathf.Max(0f, free);
        }
    }
}
