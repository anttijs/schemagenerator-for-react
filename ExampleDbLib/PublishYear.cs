using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace ExampleDbLib
{
    [NotMapped]
    public class PublishYear : IProps
    {
        public List<EnumDesc> GetPropEnums(object obj, ExampleDbContext context)
        {
            List<EnumDesc> retval = new List<EnumDesc>();
            var endYear = DateTime.Now.Year;
            var startYear = 1500;
            if (obj is MovieDTO movie)
                startYear = 1900;
            for (int i = endYear; i >= startYear; i--)
            {
                retval.Add(new EnumDesc { text = $"{i}", value = i });
            }
            return retval;
        }
    }
}
