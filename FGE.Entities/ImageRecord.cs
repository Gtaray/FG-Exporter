using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Entities
{
    public enum ImageRecordSource
    {
        Data,
        Campaign
    }
    public enum ImageType
    {
        Image,
        Token,
        RefImage
    }
    public class ImageRecord
    {
        public readonly string GraphicFile;
        public readonly string GraphicPath;
        public readonly ImageType Type;
        public readonly ImageRecordSource Source;

        public ImageRecord(string bitmap, ImageType type)
        {
            if (bitmap == null)
                throw new ArgumentException($"Bitmap was null. File: {bitmap}");

            this.GraphicFile = Path.GetFileName(bitmap);
            this.GraphicPath = Path.GetDirectoryName(bitmap).Replace("\\", "/");
            this.Type = type;


            Source = ImageRecordSource.Data;
            if (bitmap.StartsWith("campaign/"))
            {
                Source = ImageRecordSource.Campaign;
            }
        }

        // Removes the leading directories in front of the image path
        // so that all that remains is the actual file path, regardless of it's
        // from the data or campaign folder
        public string NormalizedPath
        {
            get
            {
                string path = GraphicPath;

                // First we strip out the 'campaign/' at the start of the path
                if (Source == ImageRecordSource.Campaign)
                    path = Regex.Replace(path, @"(campaign\/*)", "");

                // Now we strip out the 'image/'
                path = Regex.Replace(path, @"(images\/*)", "");
                return path;
            }
        }

        public string ImageTypePath
        {
            get
            {
                if (Type == ImageType.Image)
                    return "image";
                else if (Type == ImageType.Token)
                    return "tokens";
                else if (Type == ImageType.RefImage)
                    return "referenceimages";
                else
                    return "";
            }
        }

        public string SourceGraphic 
        {
            get
            {
                string path = "";
                if (Type == ImageType.Image || Type == ImageType.RefImage)
                    path = "images/";
                if (!string.IsNullOrEmpty(NormalizedPath))
                    path = $"{path}{NormalizedPath}";

                return $"{path}/{GraphicFile}"; ;
            }
        }

        public string ModuleGraphic
        { 
            get
            {
                string path = "";
                // Have to use string concat here because Path.Combine uses backslash
                // but the FG exporter uses forward slashes.
                if (Type == ImageType.Image)
                    path = $"images";
                // Kind of jank, but exporting tokens gets rid of all hierarchical folders
                // so we just want everything under a 'tokens' folder
                else if (Type == ImageType.Token)
                    path = "tokens";
                else if (Type == ImageType.RefImage)
                    path = "referenceimages";
                else
                    return "";

                // for non-token images, add the normalized path back to the beginning
                if (!string.IsNullOrEmpty(NormalizedPath) && Type != ImageType.Token)
                    path = $"{path}/{NormalizedPath}";

                return $"{path}/{GraphicFile}";
            } 
        }

        public override string ToString()
        {
            return $"{GraphicPath}/{GraphicFile}";
        }
    }
}
