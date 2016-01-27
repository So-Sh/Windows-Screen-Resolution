using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenResolution
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<Resolution> availableResolutions = Resolution.GetAvailableScreenResolutions();
                availableResolutions.Sort();

                int count = 1;
                foreach (var resolution in availableResolutions)
                {
                    Console.WriteLine("{0}. Width: {1}, Height: {2}", count, resolution.Width, resolution.Height);
                    count++;
                }

                Console.WriteLine("");
                Console.WriteLine("Select the Screen Resolution (1-{0})", count - 1);
                Console.WriteLine("");

                int answer = Convert.ToInt32(Console.ReadLine());

                ScreenResolution.Resolution.SetScreenResolution(availableResolutions.ElementAt(answer - 1).Width, availableResolutions.ElementAt(answer - 1).Height);           


            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
           
        }
    }
}
