using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE
{
    public enum ImageRecordSource
    {
        Data,
        Campaign
    }
    public enum ImageType
    {
        Bitmap,
        Token
    }
    public class ImageRecord
    {
        public readonly string Graphic;
        public readonly ImageType Type;
        public readonly ImageRecordSource Source;

        public ImageRecord(string bitmap, ImageType type)
        {
            this.Graphic = bitmap;
            this.Type = type;

            if (Graphic == null)
                throw new ArgumentException($"Bitmap was null. File: {bitmap}");

            Source = ImageRecordSource.Data;
            if (bitmap.StartsWith("campaign/"))
            {
                Source = ImageRecordSource.Campaign;
            }
        }

        public string SourceGraphic 
        {
            get
            {
                string filepath = Source == ImageRecordSource.Campaign
                    ? Graphic.Substring(Graphic.IndexOf('/') + 1)
                    : Graphic;
                return filepath;
            }
        }

        public string ModuleGraphic
        { 
            get
            {
                string moduleGraphic = Source == ImageRecordSource.Campaign
                    ? Graphic.Substring(Graphic.IndexOf('/') + 1)
                    : Graphic;
                // Kind of jank, but exporting tokens gets rid of all hierarchical folders
                // so we just want everything under a 'tokens' folder
                // Also, have to use string concat here because Path.Combine uses backslash
                // but the FG exporter uses forward slashes.
                if (Type == ImageType.Token)
                    moduleGraphic = "tokens/" + Path.GetFileName(moduleGraphic);
                return moduleGraphic;
            } 
        }
    }
}
