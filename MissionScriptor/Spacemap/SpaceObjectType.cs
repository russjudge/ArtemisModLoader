using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionStudio.Spacemap
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum SpaceObjectType
    {

        Station = 1,
        Player,
        Enemy,
        Neutral,
        Anomaly,
        BlackHole,
        Monster,
        GenericMesh,
        Whale,
        Nebulas,
        Asteroids,
        Mines,
        Destroyer,
        Other
    }
}
