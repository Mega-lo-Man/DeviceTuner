using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.SharedDataModel
{
    public class ConstructionSite
    {
        private List<Cabinet> _constructionSite = new List<Cabinet>();

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _directoryPath;
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set { _directoryPath = value; }
        }

        public List<Cabinet> GetAll()
        {
            return _constructionSite;
        }

        public void SetConstructionSite(List<Cabinet> cabinets)
        {
            _constructionSite = cabinets;
        }

        public void Add(Cabinet cabinets)
        {
            _constructionSite.Add(cabinets);
        }
    }
}
