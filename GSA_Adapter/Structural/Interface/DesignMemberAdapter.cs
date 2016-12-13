using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Structural.Elements;
using BHoMBR = BHoM.Base.Results;
using BHE = BHoM.Structural.Elements;

namespace GSA_Adapter.Structural.Interface
{
    public partial class GSAAdapter : BHoM.Structural.Interface.IDesignMemberAdapter
    {
        public bool SetBarDesignElement(List<Bar> bars, out List<string> ids)
        {
            return Structural.DesignMembers.BarMemberIO.SetBarMember(gsa, bars, out ids);

        }

        public List<string> GetBarDesignElement(out List<Bar> bars, List<string> ids = null)
        {
            Structural.DesignMembers.BarMemberIO.GetBarMembers(gsa, out bars, ids);
            return bars.Select(x => x[Utility.Utils.ID].ToString()).ToList();
        }

    }
}
