using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace intsis
{
    public static class GlobalDATA
    {

        public static bool recvadmin { get; set; }
        public static int IdSisForCREATE { get; set; }
        public static int SelectRULEID { get; set; }
        public static int SelectANSID { get; set; }
        public static int SelectFACTID { get; set; }
        public static bool IsFirst { get; set; }
        public static string ConnectToDB { get; set; }
        public static Brush Accent { get;set; }

        public static bool weigth { get; set; }

    //      {
    //        Database.SetInitializer(new CreateDatabaseIfNotExists<ExpertSystemEntities>());
    //        Database.Log = log => System.Diagnostics.Debug.WriteLine(log);
    //    }
    //private static ExpertSystemEntities context;
    //public static ExpertSystemEntities GetContext()
    //{
    //    if (context == null)
    //        context = new ExpertSystemEntities();
    //    return context;
    //}
}
}
