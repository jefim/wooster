using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wooster.Utils
{
    public static class ImageSourceHelper
    {
        public static ImageSource StreamToImageSource(Stream stream)
        {
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = stream;
            imageSource.EndInit();

            // Assign the Source property of your image
            return imageSource;
        }
    }
}
