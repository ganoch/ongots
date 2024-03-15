using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneOnPaper
{
    interface IAIOpponent
    {
        List<Plane> MyPlanes { get; }
    }
}
