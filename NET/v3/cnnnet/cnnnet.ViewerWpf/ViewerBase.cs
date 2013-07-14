using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace cnnnet.ViewerWpf
{
    public abstract class ViewerBase
    {
        public abstract byte[,] GetData();
    }
}
