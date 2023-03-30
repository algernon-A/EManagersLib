using System.Runtime.CompilerServices;
using UnityEngine;

namespace EManagersLib
{
    public static class EDistrictManager
    {
        public const int DEFAULTGRID_RESOLUTION = 512;


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void MoveParkProps(DistrictPark[] parks, int cellX, int cellZ, byte src, byte dest)
        {
            const float halfGrid = DEFAULTGRID_RESOLUTION / 2f;
            const int gridSize = DEFAULTGRID_RESOLUTION - 1;
            int startX = EMath.Max((int)((cellX - halfGrid) * (19.2f / 64f) + 135f), 0);
            int startZ = EMath.Max((int)((cellZ - halfGrid) * (19.2f / 64f) + 135f), 0);
            int endX = EMath.Min((int)((cellX - halfGrid + 1f) * (19.2f / 64f) + 135f), 269);
            int endZ = EMath.Min((int)((cellZ - halfGrid + 1f) * (19.2f / 64f) + 135f), 269);
            EPropInstance[] props = EPropManager.m_props.m_buffer;
            uint[] propGrid = EPropManager.m_propGrid;
            for (int i = startZ; i <= endZ; i++)
            {
                for (int j = startX; j <= endX; j++)
                {
                    uint propID = propGrid[i * 270 + j];
                    while (propID != 0)
                    {
                        if ((props[propID].m_flags & EPropInstance.BLOCKEDFLAG) == 0)
                        {
                            Vector3 position = props[propID].Position;
                            int x = EMath.Clamp((int)(position.x / 19.2f + halfGrid), 0, gridSize);
                            int y = EMath.Clamp((int)(position.z / 19.2f + halfGrid), 0, gridSize);
                            if (x == cellX && y == cellZ)
                            {
                                parks[src].m_propCount--;
                                parks[dest].m_propCount++;
                            }
                        }
                        propID = props[propID].m_nextGridProp;
                    }
                }
            }
        }
    }
}
