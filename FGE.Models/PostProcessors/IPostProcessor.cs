using FGE.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Models.PostProcessors
{
    public interface IPostProcessor
    {
        public bool ShouldRun(XDocument doc, ExportConfig config);
        public void Process(XDocument doc, ExportConfig config);
    }
}
