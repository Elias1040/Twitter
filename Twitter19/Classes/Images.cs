using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter19.Classes
{
    public class Images
    {
        public byte[] ConvertToBytes(IFormFile img)
        {
            try
            {
                MemoryStream ms = new();
                img.CopyTo(ms);
                ms.Close();
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Image Resize(Image img, Size size)
        {
            try
            {
                return (Image)new Bitmap(img, size);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string ConvertToB64(Image img)
        {
            try
            {
                MemoryStream ms = new();
                img.Save(ms, ImageFormat.Png);
                ms.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Image ConvertToImage(byte[] bytes)
        {
            MemoryStream ms = new(bytes);
            Image img = Image.FromStream(ms);
            ms.Close();
            return img;
        }
    }
}
