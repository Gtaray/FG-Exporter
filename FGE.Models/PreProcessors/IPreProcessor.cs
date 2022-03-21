using FGE.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Models.PreProcessors
{
    public interface IPreProcessor
    {
        public bool ShouldRun(Converter converter);
        public void Process(Converter converter);
    }
}
