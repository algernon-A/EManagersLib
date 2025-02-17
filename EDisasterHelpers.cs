﻿using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;
using static EManagersLib.EPropManager;

namespace EManagersLib {
    public static class EDisasterHelpers {
        public static void DestroyProps(Vector3 position, float totalRadius, float removeRadius) {
            const ushort Created = (ushort)(PropInstance.Flags.Created);
            const ushort CreatedOrDeleted = (ushort)(PropInstance.Flags.Created | PropInstance.Flags.Deleted);
            float radius = EMath.Min(totalRadius, removeRadius);
            int startX = EMath.Max((int)((position.x - radius) / PROPGRID_CELL_SIZE + 135f), 0);
            int startZ = EMath.Max((int)((position.z - radius) / PROPGRID_CELL_SIZE + 135f), 0);
            int endX = EMath.Min((int)((position.x + radius) / PROPGRID_CELL_SIZE + 135f), PROPGRID_RESOLUTION - 1);
            int endZ = EMath.Min((int)((position.z + radius) / PROPGRID_CELL_SIZE + 135f), PROPGRID_RESOLUTION - 1);
            EPropInstance[] props = m_props.m_buffer;
            uint[] propGrid = m_propGrid;
            for (int i = startZ; i <= endZ; i++) {
                for (int j = startX; j <= endX; j++) {
                    uint propID = propGrid[i * PROPGRID_RESOLUTION + j];
                    while (propID != 0) {
                        if ((props[propID].m_flags & CreatedOrDeleted) == Created) {
                            if (VectorUtils.LengthXZ(props[propID].Position - position) < radius) {
                                Singleton<PropManager>.instance.ReleaseProp(propID);
                            }
                        }
                        propID = props[propID].m_nextGridProp;
                    }
                }
            }
        }

    }
}
