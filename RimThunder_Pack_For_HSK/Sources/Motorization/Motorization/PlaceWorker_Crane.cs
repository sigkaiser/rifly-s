﻿using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace Motorization
{
    internal class PlaceWorker_Crane : PlaceWorker
    {
        private static IEnumerable<IntVec3> GetAdjacentCorners(CellRect rect)
        {
            yield return new IntVec3(rect.minX, 0, rect.minZ);
            yield return new IntVec3(rect.minX, 0, rect.maxZ);
            yield return new IntVec3(rect.maxX, 0, rect.maxZ);
            yield return new IntVec3(rect.maxX, 0, rect.minZ);
        }

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            ModExtension_CranePlaceWorker ext = checkingDef.GetModExtension<ModExtension_CranePlaceWorker>();
            if (ext == null) { Log.ErrorOnce(checkingDef.defName + " has PlaceWorker_Crane without ModExtension_CranePlaceWorker", 1919810); return true; }
            if (ext.pillarDefs == null) { Log.ErrorOnce(checkingDef.defName + " has ModExtension_CranePlaceWorker without pillarDef", 114514); return true; }
            List<IntVec3> cells = GetAdjacentCorners(GenAdj.OccupiedRect(loc, rot, checkingDef.Size)).ToList();

            bool placeable = true;
            foreach (IntVec3 cell in cells)
            {
                if (!(ext.pillarDefs.Contains(cell.GetEdifice(map)?.def)))
                {
                    GhostDrawer.DrawGhostThing(cell, rot, ext.pillarDefs.First(), ext.pillarDefs.First().graphic, Color.red, AltitudeLayer.MetaOverlays);
                    placeable = new AcceptanceReport(reasonText: "RTC_AllCornerMustBeSupportedByWall");
                }
                else
                {
                    GhostDrawer.DrawGhostThing(cell, rot, ext.pillarDefs.First(), ext.pillarDefs.First().graphic, Color.green, AltitudeLayer.MetaOverlays);
                }
            }
            return placeable;
        }
    }

    internal class ModExtension_CranePlaceWorker : DefModExtension
    {
        public List<ThingDef> pillarDefs = null;
    }
}
