using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WINCCPictrueTree.Models
{
    public class CatalogueItemClass
    {
        public string Name { get; set; }
        public string PictrueName { get; set; }
        public ObservableCollection<CatalogueItemClass> Children { get; set; }

        //public override string ToString()
        //{
        //    return Name;
        //}

        public static ObservableCollection<CatalogueItemClass> Init()
        {
            ObservableCollection<CatalogueItemClass> nodes = new ObservableCollection<CatalogueItemClass>();
            var Catalogue = Environment.CurrentDirectory;
            var file = "CatalogueFile.xml";
            DirectoryInfo dir = new DirectoryInfo(Catalogue);
            nodes.Add(new CatalogueItemClass()
            {
                Name = "未分类",
                PictrueName = "未分类",
                Children = new ObservableCollection<CatalogueItemClass>()
            });
            var fileInfos = dir.GetFiles("*.pdl", SearchOption.TopDirectoryOnly);
            if (fileInfos.Length == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    nodes[0].Children.Add(new CatalogueItemClass() { Name = $"NewPDL{i}.pdl", PictrueName = $"NewPDL{i}.pdl", Children = new ObservableCollection<CatalogueItemClass>() }
                }

            }
            else
            {
                foreach (FileInfo fileInfo in fileInfos)
                {
                    nodes[0].Children.Add(new CatalogueItemClass()
                    {
                        Name = fileInfo.Name,
                        PictrueName = fileInfo.Name
                    });
                }
            }
            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<CatalogueItemClass>));
                    var children = nodes[0].Children;
                    foreach (var item in (ObservableCollection<CatalogueItemClass>)serializer.Deserialize(stream))
                    {
                        nodes.Add(item);
                    }
                    for (int i = 1; i < nodes.Count; i++)
                    {
                        foreach (var item in nodes[i].Children)
                        {
                            children.Remove(children.First(n => n.PictrueName == item.PictrueName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nodes;
        }

        public static void Save(ObservableCollection<CatalogueItemClass> Nodes)
        {
            var file = "CatalogueFile.xml";
            ObservableCollection<CatalogueItemClass> node = new ObservableCollection<CatalogueItemClass>();
            foreach (var item in Nodes)
            {
                node.Add(item);
            }
            using (FileStream stream = File.Create(file))
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<CatalogueItemClass>));
                var temp = node[0];
                node.Remove(temp);
                serializer.Serialize(stream, node);
            }
        }
    }
}
