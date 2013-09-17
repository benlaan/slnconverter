using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Laan.SolutionConverter.Xml
{
    [Serializable]
    public abstract class SolutionItem
    {
        /// <summary>
        /// Initializes a new instance of the SolutionItem class.
        /// </summary>
        public SolutionItem()
        {
        }

        public void AddConfiguration(string config, string value)
        {
            if (Configurations == null)
                Configurations = new List<NameValue>();

            Configurations.Add(new NameValue { Name = config, Value = value });
        }
        
        [XmlIgnore]//[XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("configurations"), XmlArrayItem("configuration")]
        public List<NameValue> Configurations { get; set; }
    }
}
