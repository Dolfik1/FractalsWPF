using FractalsWPF.Commands;
using System;
using System.Collections.Generic;
//using System.Drawing;
using Drawing = System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;

namespace FractalsWPF.ViewModels
{
    class MainWindowViewModel : ViewModelBase, IMonoGameTarget
    {
        private double _maxR = 2;
        public double maxR
        {
            get 
            {
                return _maxR;
            }
            set 
            {
                if (value != _maxR)
                {
                    _maxR = value;
                    base.RaisePropertyChanged("maxR");
                }
            }
        }

        private double _minR = -2;
        public double minR
        {
            get
            {
                return _minR;
            }
            set
            {
                if (value != _minR)
                {
                    _minR = value;
                    base.RaisePropertyChanged("minR");
                }
            }
        }

        private double _maxI = 2;
        public double maxI
        {
            get
            {
                return _maxI;
            }
            set
            {
                if (value != _maxI)
                {
                    _maxI = value;
                    base.RaisePropertyChanged("maxI");
                }
            }
        }

        private double _minI = -2;
        public double minI
        {
            get
            {
                return _minI;
            }
            set
            {
                if (value != _minI)
                {
                    _minI = value;
                    base.RaisePropertyChanged("minI");
                }
            }
        }

        private DelegateCommand _updateBtnClick;
        public ICommand updateBtnClick
        {
            get
            {
                if(_updateBtnClick == null)
                {
                    _updateBtnClick = new DelegateCommand(updateBtnClickFunc);
                }
                return _updateBtnClick;
            }
        }

        private DelegateCommand _saveBtnClick;
        public ICommand saveBtnClick
        {
            get
            {
                if (_saveBtnClick == null)
                {
                    _saveBtnClick = new DelegateCommand(saveBtnClickFunc);
                }
                return _saveBtnClick;
            }
        }

        private Media.ImageSource _fractalImage = new BitmapImage();
        public Media.ImageSource fractalImage
        {
            get
            {
                return _fractalImage;
            }
            set
            {
                if (value != _fractalImage)
                {
                    _fractalImage = value;
                    base.RaisePropertyChanged("fractalImage");
                }
            }
        }

        private int _imageWidth = 800;
        public int imageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                if (value != _imageWidth)
                {
                    _imageWidth = value;
                    base.RaisePropertyChanged("imageWidth");
                }
            }
        }

        private int _imageHeight = 600;
        public int imageHeight
        {
            get
            {
                return _imageHeight;
            }
            set
            {
                if (value != _imageHeight)
                {
                    _imageHeight = value;
                    base.RaisePropertyChanged("imageHeight");
                }
            }
        }

        private Media.Brush _busyForeground = new Media.SolidColorBrush(System.Windows.Media.Colors.Green);
        public Media.Brush busyForeground
        {
            get 
            {
                return _busyForeground;
            }
            set 
            {
                if (value != _busyForeground)
                {
                    _busyForeground = value;
                    base.RaisePropertyChanged("busyForeground");
                }
            }
        }

        private string _busyText = "Free";
        public string busyText
        {
            get
            {
                return _busyText;
            }
            set 
            {
                if (value != _busyText)
                {
                    _busyText = value;
                    base.RaisePropertyChanged("busyText");
                }
            }
        }

        private bool _updateFractal = true;

        private DrawManager _drawManager;
        private SpriteBatch _MandelBrotFractal;

        private GraphicsDeviceControl _graphicsDeviceControl;

        public MainWindowViewModel(WindowsFormsHost winFormsHost)
        {
            _graphicsDeviceControl = new GraphicsDeviceControl();

            _drawManager = new DrawManager(this);

            _graphicsDeviceControl.Initialise(_drawManager);
            winFormsHost.Child = _graphicsDeviceControl;
        }

        private void updateBtnClickFunc()
        {
            _updateFractal = true;
        }

        private void saveBtnClickFunc()
        {

        }

        public void LoadContent()
        {
            _MandelBrotFractal = new SpriteBatch(_graphicsDeviceControl.GraphicsDevice);
        }


        public void Update(float deltaTime)
        {

        }
        

        Texture2D canvas;
        UInt32[] pixels;
        public void Draw()
        {
            _graphicsDeviceControl.GraphicsDevice.Clear(Color.CornflowerBlue);
            _graphicsDeviceControl.GraphicsDevice.Textures[0] = null;

            int width = _graphicsDeviceControl.GraphicsDevice.Viewport.Width;
            int height = _graphicsDeviceControl.GraphicsDevice.Viewport.Height;

            if (_updateFractal)
            {
                canvas = new Texture2D(_graphicsDeviceControl.GraphicsDevice, width, height);

                double zx = 0;
                double zy = 0;
                double cx = 0;
                double cy = 0;
                double xjump = ((maxR - minR) / width);
                double yjump = ((maxI - minI) / height);
                double tempzx = 0;

                int loopmax = 1000;
                int loopgo = 0;

                pixels = new UInt32[width * height];

                for (int x = 0; x < width; x++)
                {
                    cx = (xjump * x) - Math.Abs(minR);
                    for (int y = 0; y < height; y++)
                    {
                        zx = 0;
                        zy = 0;
                        cy = (yjump * y) - Math.Abs(minI);
                        loopgo = 0;
                        while (zx * zx + zy * zy <= 4 && loopgo < loopmax)
                        {
                            loopgo++;
                            tempzx = zx;
                            zx = (zx * zx) - (zy * zy) + cx;
                            zy = (2 * tempzx * zy) + cy;
                        }

                        //img.SetPixel(x, y, System.Drawing.Color.FromArgb(loopgo % 128 * 2, loopgo % 32 * 7, loopgo % 16 * 14));
                        //img.SetPixel(x, y, System.Drawing.Color.Black);

                        if (loopgo != loopmax)
                            pixels[y * width + x] = new Color(loopgo % 128 * 2, loopgo % 32 * 7, loopgo % 16 * 14).PackedValue;
                        else
                            pixels[y * width + x] = Color.Black.PackedValue;
                    }
                }

                canvas.SetData<UInt32>(pixels, 0, width * height);
                _updateFractal = false;
            }

            _MandelBrotFractal.Begin();
            _MandelBrotFractal.Draw(canvas, new Rectangle(0, 0, width, height), Color.White);
            _MandelBrotFractal.End();
        }
    }
}
