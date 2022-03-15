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
    public class ImageRecord
    {
        public readonly string Id;
        public readonly string Bitmap;
        public readonly ImageRecordSource Source;

        public ImageRecord(string id, string bitmap)
        {
            this.Id = id;
            this.Bitmap = bitmap;

            if (Bitmap == null)
                throw new ArgumentException($"Bitmap was null for image record {Id}");

            Source = ImageRecordSource.Data;
            if (bitmap.StartsWith("campaign/"))
            {
                Source = ImageRecordSource.Campaign;
            }
        }

        public string ModuleBitmap
        { 
            get
            {
                if (Source == ImageRecordSource.Campaign)
                {
                    return Bitmap.Substring(Bitmap.IndexOf('/') + 1);
                }
                return Bitmap;
            } 
        }
    }
}
