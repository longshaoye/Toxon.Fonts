using System;
using System.IO;

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
            catch(Exception ex)
            {
                throw;
            }

        }
    }
}
