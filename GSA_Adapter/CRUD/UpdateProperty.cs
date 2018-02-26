using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structural.Loads;
using BH.oM.Structural.Elements;
using BH.oM.Base;
using BH.Engine.GSA;
using BH.oM.DataManipulation.Queries;

namespace BH.Adapter.GSA
{
    public partial class GSAAdapter
    {
        public override int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {

            if (property == "Tags")
            {
                List<string> indecies = ids.Select(x => x.ToString()).ToList();
                if (indecies.Count < 1)
                    return 0;

                List<HashSet<string>> tags = (newValue as IEnumerable<HashSet<string>>).ToList();
                return UpdateDateTags(type, indecies, tags);
            }

            return 0;
        }

        private int UpdateDateTags(Type t, List<string> indecies, List<HashSet<string>> tags)
        {
            
            List<IBHoMObject> objects = Read(t, indecies.ToList()).ToList();

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Tags = tags[i];
            }

            if (Create(objects))
                return objects.Count;

            return 0;
        }
    }
}
