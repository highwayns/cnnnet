using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace cnnnet.ViewerWpf
{
    public static class Utils
    {
        public static WriteableBitmap LoadBitmap(string path)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.CreateOptions = BitmapCreateOptions.None;
            var streamResourceInfo = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            if (streamResourceInfo != null)
            {
                var stream = streamResourceInfo.Stream;
                img.StreamSource = stream;
            }
            img.EndInit();
            return BitmapFactory.ConvertToPbgra32Format(img);
        }
    }
}
