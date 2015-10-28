using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;


namespace ImageToGCode
{
    class GCodeGenerator
    {
        private readonly double _Width;
        private readonly double _Height;
        private readonly double _LineStep;
        private readonly double _PointStep = 0.1;
        private readonly double _Freezone;
        private readonly double _Feed;
        private readonly bool _EngraveBothDirection;

        private const int MAX_Intencity = 80;

        public GCodeGenerator(double width, double height, double lineStep, double freezone, double feed, bool engraveBothDirection)
        {
            _Width = width;
            _Height = height;
            _LineStep = lineStep;
            _Freezone = freezone;
            _Feed = feed;
            _EngraveBothDirection = engraveBothDirection;
        }
        const string feedMove = "G1 X{0:0.###} S{1:0.###}";
        //const string engraveMove = "G1 X{0:0.###} S{1:0.###}";
        //const string freeMove = "G1 X{0:0.###} M5";
        const string fastMove = "G0 X{0:0.###} Y{1:0.###} S{2:0.###}";
        public IEnumerable<string> Generate(Bitmap image)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            
            int iWidth = image.Width;   //Ширина изображения
            int iHeight = image.Height; //Высота изображения
            double pixelWidth = _Width / iWidth;    //Ширина пикселя
            if (pixelWidth < _PointStep)
                pixelWidth = _PointStep;

            int xPixelSteps = 1;    //Шаг выборки пикселей из изображения
            if (pixelWidth < _LineStep)
                xPixelSteps = (int)(_LineStep / pixelWidth);

            int linesCount = (int)(_Height / _LineStep);
            double linesPerPixel = (double)linesCount / (double)iHeight;

            List<string> gCode = new List<string>();
            gCode.Add(string.Format(nfi, "F{0:0.###}", _Feed));
            for (int y = 0; y < linesCount; y++)    //Шагаем по строкам
            {
                int yPixelCursor = (int)(y / linesPerPixel);    //Вычисляем положение в картинке

                int lastLaserState = 0;     //Сбрасываем яркость лазера на последнем шаге
                double lastX = 0.0;         //Последнее положение

                int firstDarkX = -1;        //Поиск первого темного пикселя
                for (int x = 0; x < iWidth; x++)
                {
                    if (image.GetPixel(x, yPixelCursor).GetBrightness() < 1.0)
                    {
                        firstDarkX = x;
                        break;
                    }
                }
                if (firstDarkX == -1)
                    continue;   //Пропускаем строки если они белые


                //Холостой ход в начало строки с темными пикселями
                gCode.Add(string.Format(nfi, fastMove, firstDarkX * pixelWidth - _Freezone, -y * _LineStep, 0.0));

                //Подъезжаем на рабочей скорости к первой темной точке
                //gCode.Add(string.Format(nfi, feedMove, firstDarkX * pixelWidth, 0));

                float brightnessSum = 0;
                //Перебор пикселей в строке
                for (int x = firstDarkX; x < iWidth; x++)
                {
                    if (x % xPixelSteps != 0)
                    {
                        brightnessSum += image.GetPixel(x, yPixelCursor).GetBrightness();   //суммируем яркости пикселей, между шагами головки
                        continue;
                    }

                    brightnessSum += image.GetPixel(x, yPixelCursor).GetBrightness(); //прибавляем текущий пиксель

                    int curLaserState = (int)(MAX_Intencity * (1.0 - brightnessSum / (double)xPixelSteps)); //вычисляем среднюю яркость
                    brightnessSum = 0;  //Обнуляем яркость

                    if (lastLaserState == curLaserState) //не добавляем команд если строки не закрашены
                        continue;

                    lastX = x * pixelWidth; //координата точки с изменением цвета

                    gCode.Add(string.Format(nfi, feedMove, lastX, lastLaserState));

                    lastLaserState = curLaserState;
                }

                if (lastLaserState > 0.0)
                {
                    lastX = _Width;
                    gCode.Add(string.Format(nfi, feedMove, _Width, lastLaserState)); //Завершаем строку и выключаем лазер
                }
                
                if (_Freezone > 0.0)
                    gCode.Add(string.Format(nfi, feedMove, lastX + _Freezone, 0)); //Холостой ход для торможения
                
            }
            return gCode;
        }
    }
}
