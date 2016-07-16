using System;
using System.IO;
using Toxon.Fonts.Rendering;

namespace Toxon.Fonts.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var file = File.OpenRead("ARIAL.TTF");
            var reader = new FontReader();

            Font font;
            try
            {
                font = reader.Read(file);
            }
            catch (Exception ex)
            {
                throw;
            }

            var renderer = new FontRenderer(font);

            FontImage img;
            try
            {
                img = renderer.Render('b');
            }
            catch (Exception ex)
            {
                throw;
            }

            try
            {
                var ppm = img.Render();
                File.WriteAllBytes("out.ppm", ppm);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
