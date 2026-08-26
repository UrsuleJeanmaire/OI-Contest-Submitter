using Submitter.Code.Component;
using Submitter.Code.Manager;

namespace Submitter
{
    class Submitter
    {
        static MainManager mainThread = new();
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Log.init();
                mainThread.start();
            }
            catch (Exception e)
            {
                Log.error(e.Message);
            }
            finally
            {
                Log.information("main function quit");
                Log.information("");
                Log.quit();
            }
        }
    }
}